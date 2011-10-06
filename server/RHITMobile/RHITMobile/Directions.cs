using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RHITMobile
{
    public class Directions
    {
        private static Hash<Directions> _directions = new Hash<Directions>(16);

        public static void EnqueueMonitors(ThreadManager TM)
        {
            TM.Enqueue(_directions.LookForNewIndex(TM), ThreadPriority.Low);
            TM.Enqueue(_directions.CheckForIncreaseSize(TM), ThreadPriority.Low);
        }

        public static void WriteStatus()
        {
            Console.Write("Directions in progress: ");
            _directions.WriteStatus();
        }

        public static IEnumerable<ThreadInfo> HandleDirectionsRequest(ThreadManager TM, IEnumerable<string> path, Dictionary<string, string> query)
        {
            var currentThread = TM.CurrentThread;
            if (!path.Any())
            {
                if (query.ContainsKey("id"))
                {
                    int id;
                    if (Int32.TryParse(query["id"], out id))
                    {
                        if (_directions[id] != null)
                        {
                            yield return TM.Await(currentThread, _directions[id].ReportProgress(TM));
                            yield return TM.Return(currentThread);
                        }
                        else
                        {
                            yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.NoContent));
                        }
                    }
                    else
                    {
                        yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadRequest));
                    }
                }
                else
                {
                    var directions = new Directions();
                    int id = _directions.Insert(directions);
                    TM.Enqueue(directions.GetDirections(TM, id));
                    yield return TM.Await(currentThread, directions.ReportProgress(TM));
                    yield return TM.Return(currentThread);
                }
            }
            else
            {
                yield return TM.Return(currentThread, new JsonResponse(HttpStatusCode.BadGateway));
            }
        }

        private int _done = 0;
        private int _id;

        public IEnumerable<ThreadInfo> GetDirections(ThreadManager TM, int id)
        {
            var currentThread = TM.CurrentThread;
            _id = id;
            for (int i = 1; i <= 100; i++)
            {
                yield return TM.Sleep(currentThread, 300);
                _done = i;
            }
            yield return TM.Sleep(currentThread, 60000);
            _directions.Remove(_id);
            yield return TM.Return(currentThread);
        }

        public IEnumerable<ThreadInfo> ReportProgress(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.Sleep(currentThread, 1000);
            int done = _done;
            if (done < 100)
            {
                yield return TM.Return(currentThread, new JsonResponse(new DirectionsResponse(done, _id, "Not done")));
            }
            else
            {
                yield return TM.Return(currentThread, new JsonResponse(new DirectionsResponse(100, _id, "Done!")));
            }
        }
    }
}
