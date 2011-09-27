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
        public uint NumExecutions { get; set; }

        private ulong _lastThread = 0;
        private Dictionary<ulong, ThreadInfo> _threads = new Dictionary<ulong, ThreadInfo>();

        private object _result;
        public T GetResult<T>()
        {
            if (_result is Exception)
            {
                throw _result as Exception;
            }
            return (T)_result;
        }

        public bool GetResult<T1, T2>(out T1 a1, out T2 a2)
        {
            if (_result is Exception)
            {
                throw _result as Exception;
            }
            else if (_result is T1)
            {
                a1 = (T1)_result;
                a2 = default(T2);
                return true;
            }
            else
            {
                a1 = default(T1);
                a2 = (T2)_result;
                return false;
            }
        }

        public ulong CurrentThread { get; set; }

        public void Enqueue(IEnumerable<ulong> method)
        {
            IncreaseExecutions();
            var continueThread = Await(0, method);
            ContinueThread(continueThread);
        }

        public ulong Await(ulong currentThread, IEnumerable<ulong> method)
        {
            CurrentThread = GetNextThreadId();

            var thread = method.GetEnumerator();
            _threads[CurrentThread] = new ThreadInfo(currentThread, thread);
            try
            {
                thread.MoveNext();
            }
            catch (Exception ex)
            {
                _result = ex;
                return currentThread;
            }
            return thread.Current;
        }

        public ulong Return(ulong currentThread, object result)
        {
            _result = result;

            return Return(currentThread);
        }

        public ulong Return(ulong currentThread)
        {
            ulong caller = _threads[currentThread].Caller;
            _threads.Remove(currentThread);

            return caller;
        }

        private ulong GetNextThreadId()
        {
            do
            {
                if (_lastThread == ulong.MaxValue)
                    _lastThread = 1;
                else
                    _lastThread++;
            } while (_threads.ContainsKey(_lastThread) || _lastThread == 0);
            return _lastThread;
        }

        private Queue<Continuation> _queue = new Queue<Continuation>();

        private void Enqueue(ulong thread, object result)
        {
            lock (_queue)
            {
                _queue.Enqueue(new Continuation(thread, result));
            }
        }

        private object _numExecutionsLock = new object();
        private void IncreaseExecutions()
        {
            lock (_numExecutionsLock)
            {
                NumExecutions++;
            }
        }

        private void DecreaseExecutions()
        {
            lock (_numExecutionsLock)
            {
                NumExecutions--;
            }
        }

        private void ContinueThread(ulong continueThread)
        {
            while (continueThread != 0)
            {
                var thread = _threads[continueThread];
                try
                {
                    thread.Iterator.MoveNext();
                }
                catch (Exception ex)
                {
                    _result = ex;
                    continueThread = thread.Caller;
                    continue;
                }
                continueThread = thread.Iterator.Current;
            }
            if (_result is Exception)
            {
                // Log exception
            }
            DecreaseExecutions();
        }

        public void Start(int processes)
        {
            Task[] tasks = new Task[processes];
            for (int i = 0; i < processes; i++)
            {
                tasks[i] = Task.Factory.StartNew(Run);
            }
            Task.WaitAll(tasks);
            Console.WriteLine("COMPLETED");
            Console.ReadLine();
        }

        private void Run()
        {
            while (NumExecutions > 0)
            {
                Monitor.Enter(_queue);
                while (!_queue.Any())
                {
                    Monitor.Exit(_queue);
                    Thread.Sleep(1);
                    if (NumExecutions == 0)
                        return;
                    Monitor.Enter(_queue);
                }
                var continuation = _queue.Dequeue();
                Monitor.Exit(_queue);

                _result = continuation.Result;
                ContinueThread(continuation.Thread);
            }
        }

        public ulong Requeue(ulong currentThread)
        {
            IncreaseExecutions();
            Enqueue(currentThread, "Requeued.");
            return 0;
        }

        public ulong Sleep(ulong currentThread, int milliseconds)
        {
            IncreaseExecutions();
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(milliseconds);
                return "Slept for " + milliseconds + " milliseconds.";
            }).ContinueWith((task) =>
                    Enqueue(currentThread, task.Result));

            return 0;
        }

        public ulong WaitForClient(ulong currentThread, HttpListener listener)
        {
            IncreaseExecutions();
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
                Enqueue(currentThread, task.Result));

            return 0;
        }

        public ulong MakeDbCall(ulong currentThread, string connectionString, string procedure, params SqlParameter[] parameters)
        {
            IncreaseExecutions();
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
                    Enqueue(currentThread, task.Result));

            return 0;
        }
    }

    public struct ThreadInfo
    {
        public ulong Caller;
        public IEnumerator<ulong> Iterator;

        public ThreadInfo(ulong caller, IEnumerator<ulong> iterator)
        {
            Caller = caller;
            Iterator = iterator;
        }
    }

    public struct Continuation
    {
        public ulong Thread;
        public object Result;

        public Continuation(ulong thread, object result)
        {
            Thread = thread;
            Result = result;
        }
    }
}
