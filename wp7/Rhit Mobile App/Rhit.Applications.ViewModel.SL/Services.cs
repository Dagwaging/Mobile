using System.Windows.Threading;
using Rhit.Applications.Models.Services;

namespace Rhit.Applications.ViewModels {
    public static class Services {
        public static void Start(Dispatcher dispatcher) {
            DataCollector.Instance.BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600";
            Connection.SetDispatcher(dispatcher);
        }
    }
}
