using System.Windows.Threading;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.ViewModel {
    public static class Services {
        public static void Start(Dispatcher dispatcher, bool fastLoad) {
            DataCollector.Instance.BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600";
            if(fastLoad) DataCollector.Instance.GetTopLocations(dispatcher);
            else DataCollector.Instance.GetAllLocations(dispatcher);
        }

        public static void Start(Dispatcher dispatcher) {
            Start(dispatcher, false);
        }
    }
}
