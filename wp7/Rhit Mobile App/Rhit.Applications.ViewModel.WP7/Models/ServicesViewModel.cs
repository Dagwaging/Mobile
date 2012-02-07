using System.Collections.Generic;
using Rhit.Applications.Model;
using Rhit.Applications.ViewModel.Controllers;
using System.ComponentModel;
using System.Windows;
using System;

namespace Rhit.Applications.ViewModel.Models {
    public class ServicesViewModel : DependencyObject {
        public ServicesViewModel() {
            //TODO: Use test data instead of just disabling it
            if (DesignerProperties.IsInDesignTool) return;

            ServicesController.Instance.Start();
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
    }
}
