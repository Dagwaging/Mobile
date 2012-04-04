using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels.Controllers {
    public class ServicesController : DependencyObject {
        private static ServicesController _instance;

        private ServicesController() {
            ServicesTree = new ObservableCollection<ServiceNode>();

            DataCollector.Instance.CampusServicesReturned += new CampusServicesEventHandler(CampusServicesReturned);
            DataCollector.Instance.GetCampusServices();
        }

        #region Singleton Instance
        public static ServicesController Instance {
            get {
                if(_instance == null)
                    _instance = new ServicesController();
                return _instance;
            }
        }
        #endregion

        private void CampusServicesReturned(object sender, CampusServicesEventArgs e) {
            ServicesTree.Clear();

            Root = new CampusService(e.Root);
            Root.Label = "Campus Services";

            foreach (CampusService child in Root.Children)
            {
                ServicesTree.Add(new ServiceCategoryNode(child));
            }

            foreach (Link link in Root.Links)
            {
                ServicesTree.Add(new ServiceLinkNode(link));
            }
        }

        private void VersionReturned(object sender, VersionEventArgs e)
        {
            CurrentVersion = e.ServicesVersion.ToString();
        }

        private CampusService Root { get; set; }

        public ServiceNode CurrentServiceNode { get; set; }

        internal void ReloadServices()
        {
            DataCollector.Instance.GetCampusServices();
        }

        public ObservableCollection<ServiceNode> ServicesTree { get; private set; }

        public String CurrentVersion { get; private set; }
    }
}