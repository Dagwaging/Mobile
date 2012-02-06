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

namespace Rhit.Applications.ViewModel.Controllers {
    public class ServicesController : DependencyObject {
        private static ServicesController _instance;

        private ServicesController() {
            CampusServices = new ObservableCollection<CampusService>();
            DataCollector.Instance.CampusServicesReturned += new Model.Events.CampusServicesEventHandler(CampusServicesReturned);
            DataCollector.Instance.GetCampusServices();
        }

        private void CampusServicesReturned(object sender, Model.Events.CampusServicesEventArgs e) {
            Root = new CampusService() { Label = "Campus Services", };
            foreach(CampusServicesCategory_DC service in e.Categories)
                Root.Children.Add(new CampusService(service));
            CurrentService = Root;
        }

        private CampusService Root { get; set; }

        #region Singleton Instance
        public static ServicesController Instance {
            get {
                if(_instance == null)
                    _instance = new ServicesController();
                return _instance;
            }
        }
        #endregion

        #region CurrentService
        public CampusService CurrentService {
            get { return (CampusService) GetValue(CurrentServiceProperty); }
            set { SetValue(CurrentServiceProperty, value); }
        }

        public static readonly DependencyProperty CurrentServiceProperty =
           DependencyProperty.Register("CurrentService", typeof(CampusService), typeof(ServicesController), new PropertyMetadata(null));
        #endregion

        #region Parent
        public CampusService Parent {
            get { return (CampusService)GetValue(ParentProperty); }
            set { SetValue(ParentProperty, value); }
        }

        public static readonly DependencyProperty ParentProperty =
           DependencyProperty.Register("Parent", typeof(CampusService), typeof(ServicesController), new PropertyMetadata(null));
        #endregion

        public ObservableCollection<CampusService> CampusServices { get; private set; }
    }

    public class CampusService : DependencyObject {
        public CampusService() {
            Links = new ObservableCollection<Link>();
            Children = new ObservableCollection<CampusService>();
        }

        public CampusService(CampusServicesCategory_DC model) {
            Links = new ObservableCollection<Link>();
            Children = new ObservableCollection<CampusService>();

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
           DependencyProperty.Register("Label", typeof(string), typeof(ServicesController), new PropertyMetadata(""));
        #endregion

        public ObservableCollection<Link> Links { get; private set; }

        public ObservableCollection<CampusService> Children { get; private set; }
    }
}