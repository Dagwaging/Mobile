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

namespace RHITMobile
{
    public class ThreadManager
    {
        #region Constructor
        public ThreadManager()
        {
            Enqueue(_threads.LookForNewIndex(this), ThreadPriority.Low);
            Enqueue(_threads.CheckForIncreaseSize(this), ThreadPriority.Low);
        }
        #endregion

        #region Status requests
        public void WriteExecutionStatus()
        {
            Console.WriteLine("Low priority executions: " + _numExecutions[ThreadPriority.Low]);
            Console.WriteLine("Normal priority executions: " + _numExecutions[ThreadPriority.Normal]);
        }

        public void WriteQueueStatus()
        {
            Console.WriteLine("Low priority queue items: " + _queues[ThreadPriority.Low].Count());
            Console.WriteLine("Normal priority queue items: " + _queues[ThreadPriority.Normal].Count());
        }

        public void WriteThreadStatus()
        {
            Console.Write("Threads in progress: ");
            _threads.WriteStatus();
        }
        #endregion

        #region Fields and Properties
        private Dictionary<ThreadPriority, int> _numExecutions = new Dictionary<ThreadPriority, int>()
        {
            { ThreadPriority.Low, 0 },
            { ThreadPriority.Normal, 0 },
        };

        private void IncreaseExecutions(ThreadPriority priority)
        {
            lock (_numExecutions)
            {
                _numExecutions[priority]++;
            }
        }

        private void DecreaseExecutions(ThreadPriority priority)
        {
            lock (_numExecutions)
            {
                _numExecutions[priority]--;
            }
        }

        private Dictionary<ThreadPriority, Queue<ThreadInfo>> _queues = new Dictionary<ThreadPriority, Queue<ThreadInfo>>()
        {
            { ThreadPriority.Low, new Queue<ThreadInfo>() },
            { ThreadPriority.Normal, new Queue<ThreadInfo>() },
        };

        private Hash<IteratorInfo> _threads = new Hash<IteratorInfo>(64);

        public ThreadInfo CurrentThread { get; set; }

        private ThreadInfo[] _results;
        public object GetResult(ThreadInfo currentThread)
        {
            foreach (var continuation in _results)
            {
                if (continuation.Thread == currentThread.Thread)
                {
                    if (continuation.Result is Exception)
                    {
                        throw continuation.Result as Exception;
                    }
                    return continuation.Result;
                }
            }
            return null;
        }

        public T GetResult<T>(ThreadInfo currentThread)
        {
            return (T)GetResult(currentThread);
        }

        public bool GetResult<T1, T2>(ThreadInfo currentThread, out T1 a1, out T2 a2)
        {
            foreach (var continuation in _results)
            {
                if (continuation.Thread == currentThread.Thread)
                {
                    if (continuation.Result is Exception)
                    {
                        throw continuation.Result as Exception;
                    }
                    else if (continuation.Result is T1)
                    {
                        a1 = (T1)continuation.Result;
                        a2 = default(T2);
                        return true;
                    }
                    else
                    {
                        a1 = default(T1);
                        a2 = (T2)continuation.Result;
                        return false;
                    }
                }
            }
            throw new ArgumentException("Thread does not have a result.");
        }
        #endregion

        #region Enqueue, Await, and Return
        public void Enqueue(IEnumerable<ThreadInfo> method)
        {
            Enqueue(method, ThreadPriority.Normal);
        }

        public void Enqueue(IEnumerable<ThreadInfo> method, ThreadPriority priority)
        {
            IncreaseExecutions(priority);
            var continueThread = Await(new ThreadInfo(priority), method);
            Enqueue(continueThread);
        }

        public ThreadInfo Await(ThreadInfo currentThread, IEnumerable<ThreadInfo> method)
        {
            var thread = method.GetEnumerator();
            var threadId = _threads.Insert(new IteratorInfo(currentThread.Thread, thread));
            CurrentThread = new ThreadInfo(threadId, currentThread.Priority);
            try
            {
                thread.MoveNext();
            }
            catch (Exception ex)
            {
                return currentThread.WithResult(ex);
            }
            return thread.Current;
        }

        public ThreadInfo Return(ThreadInfo currentThread, object result)
        {
            return Return(currentThread).WithResult(result);
        }

        public ThreadInfo Return(ThreadInfo currentThread)
        {
            var thread = _threads[currentThread.Thread];
            _threads.Remove(currentThread.Thread);

            return new ThreadInfo(thread.Caller, currentThread.Priority, GetResult(currentThread));
        }
        #endregion

        #region Control
        public void Start(int processes)
        {
            _results = new ThreadInfo[processes];
            Task[] tasks = new Task[processes];
            for (int i = 0; i < processes; i++)
            {
                tasks[i] = Task.Factory.StartNew(Run, i);
            }
            Task.WaitAll(tasks);
            Console.WriteLine("ThreadManager ran out of items to process.  Please restart the service.");
            Console.ReadLine();
        }

        private void Run(object processObj)
        {
            int processor = (int)processObj;
            while (_numExecutions[ThreadPriority.Normal] > -100)
            {
                ThreadInfo continuation = null;
                while (true)
                {
                    lock (_queues[ThreadPriority.Normal])
                        if (_queues[ThreadPriority.Normal].Any())
                        {
                            continuation = _queues[ThreadPriority.Normal].Dequeue();
                            break;
                        }
                    lock (_queues[ThreadPriority.Low])
                        if (_queues[ThreadPriority.Low].Any())
                        {
                            continuation = _queues[ThreadPriority.Low].Dequeue();
                            break;
                        }
                    Thread.Sleep(1);
                }

                _results[processor] = continuation;
                ContinueThread(processor, continuation);
            }
        }

        private void Enqueue(ThreadInfo thread)
        {
            lock (_queues[thread.Priority])
            {
                _queues[thread.Priority].Enqueue(thread);
            }
        }

        private void ContinueThread(int processor, ThreadInfo continueThread)
        {
            int threadId = continueThread.Thread;
            while (threadId >= 0)
            {
                var thread = _threads[threadId];
                try
                {
                    thread.Iterator.MoveNext();
                }
                catch (Exception ex)
                {
                    _results[processor] = new ThreadInfo(thread.Caller, continueThread.Priority, ex);
                    threadId = thread.Caller;
                    continue;
                }
                var threadResult = thread.Iterator.Current;
                _results[processor] = threadResult;
                threadId = threadResult.Thread;
            }
            if (_results[processor].Result is Exception)
            {
                // Log exception
            }
            DecreaseExecutions(continueThread.Priority);
        }
        #endregion

        #region Waiting operations
        public ThreadInfo Requeue(ThreadInfo currentThread)
        {
            IncreaseExecutions(currentThread.Priority);
            Enqueue(currentThread.WithResult("Requeued."));
            return new ThreadInfo(currentThread.Priority);
        }

        public ThreadInfo Sleep(ThreadInfo currentThread, int milliseconds)
        {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(milliseconds);
                    return "Slept for " + milliseconds + " milliseconds.";
                }).ContinueWith((task) =>
                    Enqueue(currentThread.WithResult(task.Result)));

            return new ThreadInfo(currentThread.Priority);
        }

        public ThreadInfo WaitForClient(ThreadInfo currentThread, HttpListener listener)
        {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(() =>
                {
                    try
                    {
                        return listener.GetContext();
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                }).ContinueWith((task) =>
                    Enqueue(currentThread.WithResult(task.Result)));

            return new ThreadInfo(currentThread.Priority);
        }

        public ThreadInfo WaitForConsole(ThreadInfo currentThread)
        {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(() =>
                {
                    return Console.ReadLine();
                }).ContinueWith((task) =>
                    Enqueue(currentThread.WithResult(task.Result)));

            return new ThreadInfo(currentThread.Priority);
        }

        public ThreadInfo MakeDbCall(ThreadInfo currentThread, string connectionString, string procedure, params SqlParameter[] parameters)
        {
            IncreaseExecutions(currentThread.Priority);
            Task.Factory.StartNew<object>(() =>
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            var table = new DataTable();
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = procedure;
                                command.Parameters.AddRange(parameters);
                                using (var reader = command.ExecuteReader())
                                {
                                    table.Load(reader);
                                }
                                return table;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                }).ContinueWith((task) =>
                    Enqueue(currentThread.WithResult(task.Result)));

            return new ThreadInfo(currentThread.Priority);
        }
        #endregion
    }

    public class IteratorInfo
    {
        public int Caller { get; private set; }
        public IEnumerator<ThreadInfo> Iterator { get; private set; }

        public IteratorInfo(int caller, IEnumerator<ThreadInfo> iterator)
        {
            Caller = caller;
            Iterator = iterator;
        }
    }

    public class ThreadInfo
    {
        public int Thread { get; private set; }
        public ThreadPriority Priority { get; private set; }
        public object Result { get; private set; }

        public ThreadInfo(int thread, ThreadPriority priority, object result)
        {
            Thread = thread;
            Priority = priority;
            Result = result;
        }

        public ThreadInfo(int thread, ThreadPriority priority) : this(thread, priority, null) { }

        public ThreadInfo(ThreadPriority priority) : this(-1, priority) { }

        public ThreadInfo WithThread(int thread)
        {
            Thread = thread;
            return this;
        }

        public ThreadInfo WithResult(object result)
        {
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

    public enum ThreadPriority
    {
        Low,
        Normal
    }
}
