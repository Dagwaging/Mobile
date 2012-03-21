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
            All = new ObservableCollection<CampusService>();
            ServicesTree = new ObservableCollection<ServiceNode>();
            CampusServiceDictionary = new Dictionary<int, CampusService>();
            CampusServiceParentDictionary = new Dictionary<int, int>();

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

        #region internal event CampusServicesUpdated
        internal event EventHandler CampusServicesUpdated;
        protected virtual void OnCampusServicesUpdated() {
            if(CampusServicesUpdated != null) CampusServicesUpdated(this, new EventArgs());
        }
        #endregion

        private void CampusServicesReturned(object sender, CampusServicesEventArgs e) {
            Root = new CampusService(e.Root);
            Root.Label = "Campus Services";
            AddCampusService(Root, -1);
            OnCampusServicesUpdated();

            foreach (CampusService child in Root.Children)
            {
                ServicesTree.Add(new ServiceCategoryNode(child));
            }

            foreach (Link link in Root.Links)
            {
                ServicesTree.Add(new ServiceLinkNode(link));
            }
        }

        private void AddCampusService(CampusService campusService, int parentId) {
            All.Add(campusService);
            CampusServiceDictionary[campusService.Id] = campusService;
            if(parentId >= 0)
                CampusServiceParentDictionary[campusService.Id] = parentId;
            foreach(CampusService child in campusService.Children)
                AddCampusService(child, campusService.Id);
        }

        private CampusService Root { get; set; }

        private Dictionary<int, CampusService> CampusServiceDictionary { get; set; }

        private Dictionary<int, int> CampusServiceParentDictionary { get; set; }

        internal void Start() { }

        internal CampusService GetCampusService(int id) {
            CampusService campusService;
            if(CampusServiceDictionary.TryGetValue(id, out campusService))
                return campusService;
            return Root;
        }

        internal CampusService GetParentCategory(int childId) {
            int parentId;
            if(!CampusServiceParentDictionary.TryGetValue(childId, out parentId))
                return null;
            return GetCampusService(parentId);
        }

        public ObservableCollection<CampusService> All { get; private set; }

        public ObservableCollection<ServiceNode> ServicesTree { get; private set; }
    }
}