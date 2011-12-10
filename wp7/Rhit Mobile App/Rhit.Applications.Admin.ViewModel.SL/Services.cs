using System.Windows.Threading;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.ViewModel {
    public static class Services {
        public static void Start(Dispatcher dispatcher) {
            DataCollector.Instance.BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600";
            DataCollector.Instance.GetAllLocations(dispatcher);
        }
    }
}
