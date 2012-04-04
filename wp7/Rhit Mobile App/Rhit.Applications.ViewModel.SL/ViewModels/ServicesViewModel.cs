using System;
using System.ComponentModel;
using System.Windows;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Utilities;
using System.Windows.Input;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.Models.Services;

namespace Rhit.Applications.ViewModels {
    public class ServicesViewModel : DependencyObject {
        public ServicesViewModel() {
            AddCategoryCommand = new RelayCommand(p => AddCategory());
            AddServiceCommand = new RelayCommand(p => AddService());
            AddRootCategoryCommand = new RelayCommand(p => AddRootCategory());
            SaveCurrentCommand = new RelayCommand(p => SaveCurrent());
            DeleteCurrentCommand = new RelayCommand(p => DeleteCurrent());
            RefreshCommand = new RelayCommand(p => ReloadServices());
            IncrementVersionCommand = new RelayCommand(p => IncrementVersion());

            DataCollector.Instance.CampusServicesUpdateReturned += new EventHandler(Instance_CampusServicesUpdateReturned);
            DataCollector.Instance.VersionUpdate += new Models.Events.VersionEventHandler(Instance_VersionUpdate);
            DataCollector.Instance.CampusServicesReturned += new Models.Events.CampusServicesEventHandler(Instance_CampusServicesReturned);

            AllFieldsVisibility = Visibility.Collapsed;

            Services = ServicesController.Instance;
        }

        void Instance_CampusServicesReturned(object sender, Models.Events.CampusServicesEventArgs e)
        {
            Services.CampusServicesReturned(sender, e);

            if (Services.CurrentServiceNode != null)
            {
                OnUpdateSelectedServiceNode(Services.CurrentServiceNode);
            }
        }

        void Instance_VersionUpdate(object sender, Models.Events.VersionEventArgs e)
        {
            if (e.ServicesVersion == 0) return;

            ServicesVersionStatus = String.Format("Increment services version (currently {0})", e.ServicesVersion);
        }

        void Instance_CampusServicesUpdateReturned(object sender, EventArgs e)
        {
            ReloadServices();
        }

        #region Command Implementations
        private void AddCategory()
        {
            String name = "New Category " + DateTime.Now.ToFileTimeUtc();

            if (Services.CurrentServiceNode is ServiceCategoryNode)
            {
                DataCollector.Instance.AddCampusServiceCategory(name, ((ServiceCategoryNode)Services.CurrentServiceNode).Category.ToLightWeightDataContract());
            }

            Services.Creating = true;
            Services.CreatedOrUpdatedItem = name;
        }

        private void AddService()
        {
            String name = "New Service " + DateTime.Now.ToFileTimeUtc();

            if (Services.CurrentServiceNode is ServiceCategoryNode)
            {
                DataCollector.Instance.AddCampusServiceLink(name, ((ServiceCategoryNode)Services.CurrentServiceNode).Category.ToLightWeightDataContract());
            }

            Services.Creating = true;
            Services.CreatedOrUpdatedItem = name;
        }

        private void AddRootCategory()
        {
            String name = "New Category " + DateTime.Now.ToFileTimeUtc();
            DataCollector.Instance.AddCampusServiceCategory(name, null);

            Services.Creating = true;
            Services.CreatedOrUpdatedItem = name;
        }

        private void SaveCurrent()
        {
            Services.Updating = true;
            Services.CreatedOrUpdatedItem = CurrentName;

            if (Services.CurrentServiceNode is ServiceCategoryNode)
            {
                ServiceCategoryNode node = (ServiceCategoryNode)Services.CurrentServiceNode;
                DataCollector.Instance.SaveCampusServiceCategory(node.Category.ToLightWeightDataContract(), CurrentName, node.Parent == null ? null : node.Parent.Name);
            }

            else if (Services.CurrentServiceNode is ServiceLinkNode)
            {
                ServiceLinkNode node = (ServiceLinkNode)Services.CurrentServiceNode;
                DataCollector.Instance.SaveCampusServiceLink(node.Link.ToLightWeightDataContract(), CurrentName, CurrentURL, node.Parent == null ? null : node.Parent.Name);
            }
        }

        private void DeleteCurrent()
        {
            if (Services.CurrentServiceNode is ServiceCategoryNode)
            {
                ServiceCategoryNode node = (ServiceCategoryNode)Services.CurrentServiceNode;
                DataCollector.Instance.DeleteCampusServiceCategory(node.Category.ToLightWeightDataContract(), node.Parent == null ? null : node.Parent.Name);
            }

            else if (Services.CurrentServiceNode is ServiceLinkNode)
            {
                ServiceLinkNode node = (ServiceLinkNode)Services.CurrentServiceNode;
                DataCollector.Instance.DeleteCampuServiceLink(node.Link.ToLightWeightDataContract(), node.Parent == null ? null : node.Parent.Name);
            }

            if (Services.CurrentServiceNode != null) Services.CurrentServiceNode = Services.CurrentServiceNode.Parent;
        }

        private void IncrementVersion()
        {
            DataCollector.Instance.IncreaseServicesVersion();
        }
        #endregion

        #region CurrentHeading
        public String CurrentHeading
        {
            get { return (String)GetValue(CurrentHeadingProperty); }
            private set { SetValue(CurrentHeadingProperty, value); }
        }

        private static readonly DependencyProperty CurrentHeadingProperty = DependencyProperty.Register("CurrentHeading", typeof(String), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region CurrentName
        public String CurrentName
        {
            get { return (String)GetValue(CurrentNameProperty); }
            private set { SetValue(CurrentNameProperty, value); }
        }

        private static readonly DependencyProperty CurrentNameProperty = DependencyProperty.Register("CurrentName", typeof(String), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region CurrentURL
        public String CurrentURL
        {
            get { return (String)GetValue(CurrentURLProperty); }
            private set { SetValue(CurrentURLProperty, value); }
        }

        private static readonly DependencyProperty CurrentURLProperty = DependencyProperty.Register("CurrentURL", typeof(String), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region LinkFieldsVisibility
        public Visibility LinkFieldsVisibility
        {
            get { return (Visibility)GetValue(LinkFieldsVisibilityProperty); }
            private set { SetValue(LinkFieldsVisibilityProperty, value); }
        }

        private static readonly DependencyProperty LinkFieldsVisibilityProperty = DependencyProperty.Register("LinkFieldsVisibility", typeof(Visibility), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region CanAddChildItems
        public Boolean CanAddChildItems
        {
            get { return (Boolean)GetValue(CanAddChildItemsProperty); }
            private set { SetValue(CanAddChildItemsProperty, value); }
        }

        private static readonly DependencyProperty CanAddChildItemsProperty = DependencyProperty.Register("CanAddChildItems", typeof(Boolean), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region AllFieldsVisibility
        public Visibility AllFieldsVisibility
        {
            get { return (Visibility)GetValue(AllFieldsVisibilityProperty); }
            private set { SetValue(AllFieldsVisibilityProperty, value); }
        }

        private static readonly DependencyProperty AllFieldsVisibilityProperty = DependencyProperty.Register("AllFieldsVisibility", typeof(Visibility), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region ServicesVersionStatus
        public String ServicesVersionStatus
        {
            get { return (String)GetValue(ServicesVersionStatusProperty); }
            private set { SetValue(ServicesVersionStatusProperty, value); }
        }

        private static readonly DependencyProperty ServicesVersionStatusProperty = DependencyProperty.Register("ServicesVersionStatus", typeof(String), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        public ServicesController Services { get; private set; }

        public void ReloadServices()
        {
            AllFieldsVisibility = Visibility.Collapsed;
            Services.ReloadServices();
        }

        public void SelectServiceNode(object serviceNode)
        {
            try { SelectServiceNode((ServiceNode)serviceNode); }
            catch { }
        }

        public void SelectServiceNode(ServiceNode serviceNode)
        {
            Services.CurrentServiceNode = serviceNode;

            if (serviceNode is ServiceCategoryNode)
            {
                AllFieldsVisibility = Visibility.Visible;
                CurrentHeading = "Category";
                CurrentName = serviceNode.Name;
                CurrentURL = "";
                LinkFieldsVisibility = Visibility.Collapsed;
                CanAddChildItems = true;
            }
            else if (serviceNode is ServiceLinkNode)
            {
                AllFieldsVisibility = Visibility.Visible;
                CurrentHeading = "Service Link";
                CurrentName = serviceNode.Name;
                CurrentURL = ((ServiceLinkNode)serviceNode).Link.Address;
                LinkFieldsVisibility = Visibility.Visible;
                CanAddChildItems = false;
            }
        }

        #region Commands
        public ICommand RefreshCommand { get; private set; }

        public ICommand AddServiceCommand { get; private set; }

        public ICommand AddCategoryCommand { get; private set; }

        public ICommand AddRootCategoryCommand { get; private set; }

        public ICommand SaveCurrentCommand { get; private set; }

        public ICommand DeleteCurrentCommand { get; private set; }

        public ICommand IncrementVersionCommand { get; private set; }
        #endregion

        #region UpdateSelectedServiceNode
        public delegate void UpdateSelectedServiceNodeEventHandler(Object sender, SelectedServiceNodeArgs args);

        public class SelectedServiceNodeArgs : EventArgs
        {
            public ServiceNode Selected { get; set; }
        }

        public event UpdateSelectedServiceNodeEventHandler UpdateSelectedServiceNode;
        protected virtual void OnUpdateSelectedServiceNode(ServiceNode selectedNode)
        {
            SelectedServiceNodeArgs args = new SelectedServiceNodeArgs() {
                Selected = selectedNode,
            };

            if (UpdateSelectedServiceNode != null) UpdateSelectedServiceNode(this, args);
        }
        #endregion
    }
}
