using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Rhit.Applications.Models;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels.Controllers {
    public class ServicesController : DependencyObject {
        private static ServicesController _instance;

        private ServicesController() {
            ServicesTree = new ObservableCollection<ServiceNode>();

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

        public void CampusServicesReturned(object sender, CampusServicesEventArgs e) {
            String currentName;
            String currentParent;

            if (Creating)
            {
                currentName = CreatedOrUpdatedItem;
                currentParent = CurrentServiceNode == null ? null : CurrentServiceNode.Name;

                CreatedOrUpdatedItem = null;
                Creating = false;
            }
            else if (Updating)
            {
                currentName = CreatedOrUpdatedItem;
                currentParent = CurrentServiceNode == null ? null : (CurrentServiceNode.Parent == null ? null : CurrentServiceNode.Parent.Name);

                CreatedOrUpdatedItem = null;
                Updating = false;
            }
            else
            {
                currentName = CurrentServiceNode == null ? null : CurrentServiceNode.Name;
                currentParent = CurrentServiceNode == null ? null : (CurrentServiceNode.Parent == null ? null : CurrentServiceNode.Parent.Name);
            }

            ServicesTree.Clear();
            CurrentServiceNode = null;

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

            foreach (ServiceNode node in ServicesTree)
            {
                if (node.Name == currentName && ((node.Parent == null && currentParent == null) || (node.Parent != null && node.Parent.Name == currentParent)))
                {
                    CurrentServiceNode = node;
                }

                foreach (ServiceNode childNode in node.GetRecursiveChildren())
                {
                    if (childNode.Name == currentName && (childNode.Parent == null && currentParent == null || childNode.Parent.Name == currentParent))
                    {
                        CurrentServiceNode = childNode;
                    }
                }
            }
        }

        public Boolean Creating { get; set; }

        public Boolean Updating { get; set; }

        public String CreatedOrUpdatedItem { get; set; }

        private CampusService Root { get; set; }

        public ServiceNode CurrentServiceNode { get; set; }

        internal void ReloadServices()
        {
            DataCollector.Instance.GetCampusServices();
        }

        public ObservableCollection<ServiceNode> ServicesTree { get; private set; }

        #region ServicesVersionStatus
        public String ServicesVersionStatus
        {
            get { return (String)GetValue(ServicesVersionStatusProperty); }
            set { SetValue(ServicesVersionStatusProperty, value); }
        }

        private static readonly DependencyProperty ServicesVersionStatusProperty = DependencyProperty.Register("ServicesVersionStatus", typeof(String), typeof(ServicesController), new PropertyMetadata(null));
        #endregion
    }
}