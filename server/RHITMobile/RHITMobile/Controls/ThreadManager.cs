using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;

namespace RHITMobile {
    /// <summary>
    /// Manages synchronization within the application to minimize multi-threading
    /// </summary>
    public class ThreadManager {
        #region Constructor
        public ThreadManager() {
            Enqueue(_threads.LookForNewIndex(this), ThreadPriority.Low);
            Enqueue(_threads.CheckForIncreaseSize(this), ThreadPriority.Low);
        }
        #endregion

        #region Status requests
        /// <summary>
        /// Write the number of executions
        /// </summary>
        public void WriteExecutionStatus() {
            Console.WriteLine("Low priority executions: " + _numExecutions[ThreadPriority.Low]);
            Console.WriteLine("Normal priority executions: " + _numExecutions[ThreadPriority.Normal]);
        }

        /// <summary>
        /// Write the number of items in the execution queues
        /// </summary>
        public void WriteQueueStatus() {
            Console.WriteLine("Low priority queue items: " + _queues[ThreadPriority.Low].Count());
            Console.WriteLine("Normal priority queue items: " + _queues[ThreadPriority.Normal].Count());
        }

        /// <summary>
        /// Write the number of threads in progress
        /// </summary>
        public void WriteThreadStatus() {
            Console.Write("Threads in progress: ");
            _threads.WriteStatus();
        }
        #endregion

        #region Fields and Properties
        /// <summary>
        /// Number of executions for each priority
        /// </summary>
        private Dictionary<ThreadPriority, int> _numExecutions = new Dictionary<ThreadPriority, int>()
        {
            { ThreadPriority.Low, 0 },
            { ThreadPriority.Normal, 0 },
        };

        /// <summary>
        /// Safely increase the number of executions for a given priority
        /// </summary>
        /// <param name="priority">Priority of execution</param>
        private void IncreaseExecutions(ThreadPriority priority) {
            lock (_numExecutions) {
                _numExecutions[priority]++;
            }
        }

        /// <summary>
        /// Safely decrease the number of executions for a given priority
        /// </summary>
        /// <param name="priority">Priority of execution</param>
        private void DecreaseExecutions(ThreadPriority priority) {
            lock (_numExecutions) {
                _numExecutions[priority]--;
            }
        }

        /// <summary>
        /// Execution queues for each priority
        /// </summary>
        private Dictionary<ThreadPriority, Queue<ThreadInfo>> _queues = new Dictionary<ThreadPriority, Queue<ThreadInfo>>()
        {
        private Dictionary<ThreadPriority, Queue<ThreadInfo>> _queues = new Dictionary<ThreadPriority, Queue<ThreadInfo>>() {
            { ThreadPriority.Low, new Queue<ThreadInfo>() },
            { ThreadPriority.Normal, new Queue<ThreadInfo>() },
        };

        /// <summary>
        /// Threads that have not returned
        /// </summary>
        private Hash<IteratorInfo> _threads = new Hash<IteratorInfo>(64);

        /// <summary>
        /// Thread that is currently being executed
        /// </summary>
        public ThreadInfo CurrentThread { get; set; }

        /// <summary>
        /// Results from each thread being executed
        /// </summary>
        private ThreadInfo _result;

        /// <summary>
        /// Gets the result from the last async call
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <returns>Result as an "object"</returns>
        public object GetResult(ThreadInfo currentThread) {
            if (_result.Thread == currentThread.Thread) {
                if (_result.Result is ServerHandledException) {
                    throw (_result.Result as ServerHandledException).WithThread(currentThread);
                } else if (_result.Result is Exception) {
                    throw new ExceptionThrownException<Exception>(currentThread, _result.Result as Exception);
                }
                return _result.Result;
            }
            return null;
        }

        /// <summary>
        /// Gets the result from the last async call, but does not throw exceptions
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <returns>Result as an "object"</returns>
        public object GetResultNoException(ThreadInfo currentThread) {
            if (_result.Thread == currentThread.Thread) {
                return _result.Result;
            }
            return null;
        }

        /// <summary>
        /// Gets the result from the last async call
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="currentThread">Current thread</param>
        /// <returns>Result as type T</returns>
        public T GetResult<T>(ThreadInfo currentThread) {
            return (T)GetResult(currentThread);
        }
        #endregion

        #region Enqueue, Await, and Return
        /// <summary>
        /// Starts an async execution of a method with normal priority
        /// </summary>
        /// <param name="method">Method to execute</param>
        public void Enqueue(IEnumerable<ThreadInfo> method) {
            Enqueue(method, ThreadPriority.Normal);
        }

        /// <summary>
        /// Starts an async execution of a method
        /// </summary>
        /// <param name="method">Method to execute</param>
        /// <param name="priority">Priority of execution</param>
        public void Enqueue(IEnumerable<ThreadInfo> method, ThreadPriority priority) {
            IncreaseExecutions(priority);
            var continueThread = Await(new ThreadInfo(priority), method);
            Enqueue(continueThread);
        }

        /// <summary>
        /// Makes a synchronous call to an async method - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <param name="method">Method to call</param>
        /// <returns>void</returns>
        public ThreadInfo Await(ThreadInfo currentThread, IEnumerable<ThreadInfo> method) {
            var thread = method.GetEnumerator();
            var threadId = _threads.Insert(new IteratorInfo(currentThread.Thread, thread));
            CurrentThread = new ThreadInfo(threadId, currentThread.Priority);
            try {
                thread.MoveNext();
            } catch (ServerHandledException ex) {
                Return(ex.CurrentThread);
                return currentThread.WithResult(ex);
            } catch (Exception ex) {
                return currentThread.WithResult(ex);
            }
            return thread.Current;
        }

        /// <summary>
        /// Returns from an async method with a result - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <param name="result">Return value of method</param>
        /// <returns>void</returns>
        public ThreadInfo Return(ThreadInfo currentThread, object result) {
            return Return(currentThread).WithResult(result);
        }

        /// <summary>
        /// Returns from an async method with whatever was last returned - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <returns>void</returns>
        public ThreadInfo Return(ThreadInfo currentThread) {
            var thread = _threads[currentThread.Thread];
            _threads.Remove(currentThread.Thread);

            return new ThreadInfo(thread.Caller, currentThread.Priority, GetResultNoException(currentThread));
        }
        #endregion

        #region Control
        /// <summary>
        /// Starts the ThreadManager loop with the given number of processes
        /// </summary>
        public void Start() {
            Run();
            Console.WriteLine("ThreadManager ran out of items to process.  Please restart the service.");
            Console.ReadLine();
        }

        /// <summary>
        /// Continually takes from the execution queues and executes the next item
        /// </summary>
        private void Run() {
            while (true) {
                ThreadInfo continuation = null;
                while (true) {
                    // Check the queues in order of priority
                    lock (_queues[ThreadPriority.Normal])
                        if (_queues[ThreadPriority.Normal].Any()) {
                            continuation = _queues[ThreadPriority.Normal].Dequeue();
                            break;
                        }
                    lock (_queues[ThreadPriority.Low])
                        if (_queues[ThreadPriority.Low].Any()) {
                            continuation = _queues[ThreadPriority.Low].Dequeue();
                            break;
                        }
                    Thread.Sleep(1);
                }

                // Continue the next item in the queue
                _result = continuation;
                ContinueThread(continuation);
            }
        }

        /// <summary>
        /// Adds a continuation to a queue
        /// </summary>
        /// <param name="thread">Thread to add to queue</param>
        private void Enqueue(ThreadInfo thread) {
            lock (_queues[thread.Priority]) {
                _queues[thread.Priority].Enqueue(thread);
            }
        }

        /// <summary>
        /// Runs a thread until a blocking call must be made
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="continueThread"></param>
        private void ContinueThread(ThreadInfo continueThread) {
            int threadId = continueThread.Thread;
            while (threadId >= 0) {
                var thread = _threads[threadId];
                try {
                    // Advance the thread
                    thread.Iterator.MoveNext();
                } catch (ServerHandledException ex) {
                    Return(ex.CurrentThread);
                    _result = new ThreadInfo(thread.Caller, continueThread.Priority, ex);
                    threadId = thread.Caller;
                    continue;
                } catch (Exception ex) {
                    // If an exception was thrown, return to the caller
                    _result = new ThreadInfo(thread.Caller, continueThread.Priority, ex);
                    threadId = thread.Caller;
                    continue;
                }
                // If no exception was thrown, store the result and continue down the call stack
                var threadResult = thread.Iterator.Current;
                _result = threadResult;
                threadId = threadResult.Thread;
            }
            if (_result.Result is Exception) {
                // Log exception
            }
            DecreaseExecutions(continueThread.Priority);
        }
        #endregion

        #region Waiting operations
        /// <summary>
        /// Sleeps for a specified amount of time - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <param name="milliseconds">Time to sleep in milliseconds</param>
        /// <returns>string - String specifying how long the thread slept</returns>
        public ThreadInfo Sleep(ThreadInfo currentThread, int milliseconds) {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew(() => {
                Thread.Sleep(milliseconds);
                return "Slept for " + milliseconds + " milliseconds.";
            }).ContinueWith((task) => {
                if (task.Exception == null)
                    Enqueue(currentThread.WithResult(task.Result));
                else
                    Enqueue(currentThread.WithResult(task.Exception.GetBaseException()));
            });

            return new ThreadInfo(currentThread.Priority);
        }

        /// <summary>
        /// Waits for a client using an HttpListener - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <param name="listener">HTTPListener</param>
        /// <returns>HttpListenerContext - Client found</returns>
        public ThreadInfo WaitForClient(ThreadInfo currentThread, HttpListener listener) {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(() => {
                try {
                    return listener.GetContext();
                } catch (Exception ex) {
                    return ex;
                }
            }).ContinueWith((task) => {
                if (task.Exception == null)
                    Enqueue(currentThread.WithResult(task.Result));
                else
                    Enqueue(currentThread.WithResult(task.Exception.GetBaseException()));
            });

            return new ThreadInfo(currentThread.Priority);
        }

        /// <summary>
        /// Waits for input from the console - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <returns>string - Input from the console</returns>
        public ThreadInfo WaitForConsole(ThreadInfo currentThread) {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(() => {
                return Console.ReadLine();
            }).ContinueWith((task) => {
                if (task.Exception == null)
                    Enqueue(currentThread.WithResult(task.Result));
                else
                    Enqueue(currentThread.WithResult(task.Exception.GetBaseException()));
            });

            return new ThreadInfo(currentThread.Priority);
        }

        /// <summary>
        /// Waits for a SQL stored procedure to execute - Should be within a "yield return"
        /// </summary>
        /// <param name="currentThread">Current thread</param>
        /// <param name="connectionString">String to connect to the database</param>
        /// <param name="procedure">Name of stored procedure</param>
        /// <param name="parameters">List of parameters to the procedure</param>
        /// <returns>DataTable - Result from the stored procedure</returns>
        public ThreadInfo MakeDbCall(ThreadInfo currentThread, string connectionString, string procedure, params SqlParameter[] parameters) {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(() => {
                try {
                    using (var connection = new SqlConnection(connectionString)) {
                        connection.Open();
                        var table = new DataTable();
                        using (var command = connection.CreateCommand()) {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = procedure;
                            command.Parameters.AddRange(parameters);
                            using (var reader = command.ExecuteReader()) {
                                table.Load(reader);
                            }
                            return table;
                        }
                    }
                } catch (Exception ex) {
                    return ex;
                }
            }).ContinueWith((task) => {
                if (task.Exception == null)
                    Enqueue(currentThread.WithResult(task.Result));
                else
                    Enqueue(currentThread.WithResult(task.Exception.GetBaseException()));
            });

            return new ThreadInfo(currentThread.Priority);
        }

        public ThreadInfo StartNewThread(ThreadInfo currentThread, Func<object> thread) {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(thread).ContinueWith((task) => {
                if (task.Exception == null)
                    Enqueue(currentThread.WithResult(task.Result));
                else
                    Enqueue(currentThread.WithResult(task.Exception.GetBaseException()));
            });

            return new ThreadInfo(currentThread.Priority);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class IteratorInfo {
        public int Caller { get; private set; }
        public IEnumerator<ThreadInfo> Iterator { get; private set; }

        public IteratorInfo(int caller, IEnumerator<ThreadInfo> iterator) {
            Caller = caller;
            Iterator = iterator;
        }
    }

    public class ThreadInfo {
        public int Thread { get; private set; }
        public ThreadPriority Priority { get; private set; }
        public object Result { get; private set; }

        public ThreadInfo(int thread, ThreadPriority priority, object result) {
            Thread = thread;
            Priority = priority;
            Result = result;
        }

        public ThreadInfo(int thread, ThreadPriority priority) : this(thread, priority, null) { }

        public ThreadInfo(ThreadPriority priority) : this(-1, priority) { }

        public ThreadInfo WithThread(int thread) {
            Thread = thread;
            return this;
        }

        public ThreadInfo WithResult(object result) {
            Result = result;
            return this;
        }
    }

    /*public class Continuation
    {
        public int Thread { get; private set; }
        public object Result { get; private set; }

        public Continuation(int thread, object result)
        {
            Thread = thread;
            Result = result;
        }
    }*/

    public enum ThreadPriority {
        Low,
        Normal
    }
}
