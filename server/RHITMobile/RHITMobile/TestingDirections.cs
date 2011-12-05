using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RHITMobile
{
    public class TestingDirectionsHandler : PathHandler
    {
        public TestingDirectionsHandler()
        {
            Redirects.Add("directions", new TestingDirectionsDirectionsHandler());
            Redirects.Add("tour", new TestingDirectionsTourHandler());
            Redirects.Add("schedule", new TestingDirectionsScheduleHandler());
        }
    }

    public class TestingDirectionsDirectionsHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(new DirectionsResponse(100, 0)
            {
                Result = new Directions(new LatLong(39.4829356448704,-87.3240112828813),
                    new List<Path>()
                    {
                        new Path(39.4830085089037,-87.3240387806918, "Go down the stairs", false),
                        new Path(39.4830037513645,-87.3240594201352, null, false),
                        new Path(39.4829310360043,-87.3240312773421, "Turn left in the hallway", false),
                        new Path(39.4829595714821,-87.3239144478958, "Turn right and head outside", false),
                        new Path(39.4829201855182,-87.3238995842144, null, false),
                        new Path(39.4828827044101,-87.32387560499762, "Go around the circle", false),
                        new Path(39.48269224869126,-87.32380184424972, null, false),
                        new Path(39.482547336381884,-87.32394534243201, null, false),
                        new Path(39.48244900285711,-87.32391852034186, null, false),
                        new Path(39.48243968704229,-87.32385817063903, null, false),
                        new Path(39.482352739379756,-87.32380720866774, "Cross the street", false),
                        new Path(39.482256475766555,-87.32372137797927, "Follow the path", false),
                        new Path(39.48215193126149,-87.32370528472518, null, false),
                        new Path(39.48205670246667,-87.32352691782569, null, false),
                        new Path(39.48196354373688,-87.3234759558544, null, false),
                        new Path(39.48196871922512,-87.32338878406142, null, false),
                        new Path(39.48189936765145,-87.32332575214957, "Travel around the Flame of the Millennium", false),
                        new Path(39.48189315705929,-87.3232037116394, null, false),
                        new Path(39.482012193312364,-87.32314202083205, null, false),
                        new Path(39.481946982172865,-87.32293683184241, "Take the right path", false),
                        new Path(39.48197492981158,-87.32283088458632, "Cross the street", false),
                        new Path(39.48196664903091,-87.3227370072708, null, false),
                        new Path(39.48198942117538,-87.32248085630988, null, true),
                    })
            }));
        }
    }

    public class TestingDirectionsTourHandler : PathHandler
    {

    }

    public class TestingDirectionsScheduleHandler : PathHandler
    {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state)
        {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse(new DirectionsResponse(100, 0)
            {
                Result = new Directions(new LatLong(39.482605301347846,-87.32549565924262),
                    new List<Path>()
                    {
                        new Path(39.48271605569064,-87.32544067395781, "Turn left out of the building", false),
                        new Path(39.48276159948134,-87.32544201506232, null, false),
                        new Path(39.48279782747537,-87.32531058682059, "Cross the street", false),
                        new Path(39.4828233768118,-87.3251946430617, "Enter the building", false),
                        new Path(39.4827981277893,-87.3251275442943, "Travel up the stairs", false),
                        new Path(39.4828267736495,-87.3251036056461, "Turn left", false),
                        new Path(39.4828483492916,-87.3250270327099, "Turn left into O259", true),
                        new Path(39.4828936261573,-87.3248265602336, "Turn left in the hallway", false),
                        new Path(39.4829038329553,-87.3247379356305, "Turn right", false),
                        new Path(39.4826674518561,-87.3246589327903, "Follow the hallway", false),
                        new Path(39.4827460528963,-87.3243069813, "Turn right into O203", true),
                        new Path(39.4827539048826,-87.3242836147281, "Turn right in the hallway", false),
                        new Path(39.4828300704665,-87.3243038373829, null, false),
                        new Path(39.4828504540649,-87.3242135523541, "Turn right", false),
                        new Path(39.4828612209994,-87.3241694058094, "Follow the hallway", false),
                        new Path(39.4828314701146,-87.3241520087641, null, false),
                        new Path(39.4828936203629,-87.3238719836538, null, false),
                        new Path(39.4829198685854,-87.3238828534351, null, false),
                        new Path(39.4829284921556,-87.3238468040853, null, false),
                        new Path(39.4829719528795,-87.3238645568296, null, false),
                        new Path(39.482976303729,-87.3238451260226, null, false),
                        new Path(39.4829635205691,-87.3237997253878, null, false),
                        new Path(39.4830580175096,-87.3234086319921, "Continue straight", false),
                        new Path(39.4830788474827,-87.3234159824097, "Turn left into A202", true),
                        new Path(39.4830580175096,-87.3234086319921, null, false),
                        new Path(39.4830138156114,-87.3235937824256, "Turn right", false),
                        new Path(39.4830932852368,-87.3236203915097, "Turn right and go up the stairs", false),
                        new Path(39.4830974840139,-87.3236075339996, null, false),
                        new Path(39.4835935530998,-87.3237811514371, "Go down the hallway", false),
                        new Path(39.4835979732896,-87.3237626363938, "Turn right into D219", true),
                        new Path(39.4835935530998,-87.3237811514371, null, false),
                        new Path(39.4837042676847,-87.3238187861477, "Turn right in the hallway", false),
                        new Path(39.4836460794462,-87.3240577916873, "Go down all the stairs", false),
                        new Path(39.483619680332296,-87.32420685781096, "Go outside", false),
                        new Path(39.4835741371045,-87.32441607011413, "Continue straight", false),
                        new Path(39.48338471835569,-87.32457566155051, "Make a slight left", false),
                        new Path(39.48325533366785,-87.32483449472045, "Pass the library", false),
                        new Path(39.48315286082586,-87.32491093767737, "Continue straight", false),
                        new Path(39.48312284349966,-87.32503566039657, "Make a slight right", false),
                        new Path(39.48308040518877,-87.32510808003997, null, false),
                        new Path(39.48299552848933,-87.32537361873244, "Continue along the sidewalk", false),
                        new Path(39.48289409034682,-87.32541251076316, null, false),
                        new Path(39.48279782747537,-87.32531058682059, null, false),
                        new Path(39.48276159948134,-87.32544201506232, "Cross the street", false),
                        new Path(39.48271605569064,-87.32544067395781, null, true),
                    })
            }));
        }
    }
}
