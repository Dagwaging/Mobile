using System.Windows.Threading;
using Rhit.Admin.Model.Services;

namespace Rhit.Admin.ViewModel.ViewModels {
    public static class Services {
        public static void Start(Dispatcher dispatcher) {
            DataCollector.Instance.BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600";
            DataCollector.Instance.GetAllLocations(dispatcher);
        }
    }
}
