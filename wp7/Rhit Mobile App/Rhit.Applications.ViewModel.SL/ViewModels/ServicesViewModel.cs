using System;
using System.ComponentModel;
using System.Windows;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Utilities;
using System.Windows.Input;
using Rhit.Applications.Mvvm.Commands;

namespace Rhit.Applications.ViewModels {
    public class ServicesViewModel : DependencyObject {
        public ServicesViewModel() {
            AddCategoryCommand = new RelayCommand(p => AddCategory());
            AddServiceCommand = new RelayCommand(p => AddService());
            SaveCurrentCommand = new RelayCommand(p => SaveCurrent());
            DeleteCurrentCommand = new RelayCommand(p => DeleteCurrent());
            CheckURLCommand = new RelayCommand(p => CheckURL());

            AllFieldsVisibility = Visibility.Collapsed;

            Services = ServicesController.Instance;
        }

        #region Command Implementations
        private void AddCategory()
        {

        }

        private void AddService()
        {

        }

        private void SaveCurrent()
        {

            ReloadServices();
        }

        private void DeleteCurrent()
        {

            ReloadServices();
        }

        private void CheckURL()
        {

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

        #region AllFieldsVisibility
        public Visibility AllFieldsVisibility
        {
            get { return (Visibility)GetValue(AllFieldsVisibilityProperty); }
            private set { SetValue(AllFieldsVisibilityProperty, value); }
        }

        private static readonly DependencyProperty AllFieldsVisibilityProperty = DependencyProperty.Register("AllFieldsVisibility", typeof(Visibility), typeof(ServicesViewModel), new PropertyMetadata(null));
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
            }
            else if (serviceNode is ServiceLinkNode)
            {
                AllFieldsVisibility = Visibility.Visible;
                CurrentHeading = "Service Link";
                CurrentName = serviceNode.Name;
                CurrentURL = ((ServiceLinkNode)serviceNode).Link.Address;
                LinkFieldsVisibility = Visibility.Visible;
            }
        }

        #region Commands
        public ICommand AddServiceCommand { get; private set; }

        public ICommand AddCategoryCommand { get; private set; }

        public ICommand SaveCurrentCommand { get; private set; }

        public ICommand DeleteCurrentCommand { get; private set; }

        public ICommand CheckURLCommand { get; private set; }
        #endregion

    }
}
