using System;
using System.ComponentModel;
using System.Windows;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels {
    public class ServicesViewModel : DependencyObject {
        public ServicesViewModel() {
            //TODO: Use test data instead of just disabling it
            if (DesignerProperties.IsInDesignTool) return;

            CurrentHeading = "Select a Service";

            Services = ServicesController.Instance;
            Services.Start();
            ServicesController.Instance.CampusServicesUpdated += new EventHandler(CampusServicesUpdated);
        }

        private void CampusServicesUpdated(object sender, EventArgs e) {
            Update();
        }

        private int Index { get; set; }

        private void Update() {
            CampusService campusService = ServicesController.Instance.GetCampusService(Index);
            if(campusService == null) return;

            CurrentService = ServicesController.Instance.GetCampusService(Index);
            Parent = ServicesController.Instance.GetParentCategory(CurrentService.Id);
        }

        public void SetIndex(int id) {
            Index = id;
            Update();
        }

        #region CurrentService
        public CampusService CurrentService {
            get { return (CampusService) GetValue(CurrentServiceProperty); }
            set { SetValue(CurrentServiceProperty, value); }
        }

        public static readonly DependencyProperty CurrentServiceProperty =
           DependencyProperty.Register("CurrentService", typeof(CampusService), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region Parent
        public CampusService Parent {
            get { return (CampusService) GetValue(ParentProperty); }
            set { SetValue(ParentProperty, value); }
        }

        public static readonly DependencyProperty ParentProperty =
           DependencyProperty.Register("Parent", typeof(CampusService), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region CurrentHeading
        public String CurrentHeading
        {
            get { return (String)GetValue(CurrentHeadingProperty); }
            private set { SetValue(CurrentHeadingProperty, value); }
        }

        private static readonly DependencyProperty CurrentHeadingProperty = DependencyProperty.Register("CurrentHeading", typeof(String), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        #region LinkFieldsVisibility
        public Visibility LinkFieldsVisibility
        {
            get { return (Visibility)GetValue(LinkFieldsVisibilityProperty); }
            private set { SetValue(LinkFieldsVisibilityProperty, value); }
        }

        private static readonly DependencyProperty LinkFieldsVisibilityProperty = DependencyProperty.Register("LinkFieldsVisibility", typeof(Visibility), typeof(ServicesViewModel), new PropertyMetadata(null));
        #endregion

        public ServicesController Services { get; private set; }

        public void SelectServiceNode(object serviceNode)
        {
            try { SelectServiceNode((ServiceNode)serviceNode); }
            catch { }
        }

        public void SelectServiceNode(ServiceNode serviceNode)
        {
            if (serviceNode is ServiceCategoryNode)
            {
                CurrentHeading = "Category";
                LinkFieldsVisibility = Visibility.Collapsed;
            }
            else if (serviceNode is ServiceLinkNode)
            {
                CurrentHeading = "Service Link";
                LinkFieldsVisibility = Visibility.Visible;
            }
        }
    }
}
