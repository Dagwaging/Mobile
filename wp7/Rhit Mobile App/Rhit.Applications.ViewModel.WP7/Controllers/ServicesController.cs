using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Utilities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Rhit.Applications.Model.Events;

namespace Rhit.Applications.ViewModel.Controllers {
    public class ServicesController : DependencyObject {
        private static ServicesController _instance;

        private ServicesController() {
            All = new ObservableCollection<CampusService>();
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
    }

    public class CampusService : DependencyObject {
        private static int _lastId = 0;

        public CampusService() {
            Id = ++_lastId;
            Links = new ObservableCollection<Link>();
            Children = new ObservableCollection<CampusService>();
        }

        public CampusService(CampusServicesCategory_DC model) : this() {
            Label = model.Name;
            foreach(CampusServicesCategory_DC service in model.Children)
                Children.Add(new CampusService(service));
            foreach(Link_DC link in model.Links)
                Links.Add(new Link() { Name = link.Name, Address = link.Address, });

        }

        #region Label
        public string Label {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(CampusService), new PropertyMetadata(""));
        #endregion

        public int Id { get; private set; }

        public ObservableCollection<Link> Links { get; private set; }

        public ObservableCollection<CampusService> Children { get; private set; }
    }
}