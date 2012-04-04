using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Rhit.Applications.ViewModels;
using Rhit.Applications.ViewModels.Utilities;
using System.Collections.Generic;

namespace Rhit.Applications.Views.Views
{
    public partial class ServicesPage : Page
    {
        private ServicesViewModel ViewModel { get; set; }
        private TreeView ServicesTreeView { get; set; }

        public ServicesPage()
        {
            ViewModel = new ServicesViewModel();
            DataContext = ViewModel;

            ViewModel.UpdateSelectedServiceNode += new ServicesViewModel.UpdateSelectedServiceNodeEventHandler(ViewModel_UpdateSelectedServiceNode);

            InitializeComponent();
        }

        void ViewModel_UpdateSelectedServiceNode(object sender, ServicesViewModel.SelectedServiceNodeArgs args)
        {
            ServicesTreeView.UpdateLayout();

            TreeViewItem item = ServicesTreeView.ItemContainerGenerator.ContainerFromItem(args.Selected) as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
            }
            else
            {
                List<ServiceNode> parentPath = new List<ServiceNode>();
                ServiceNode targetNode = args.Selected;
                ServiceNode currentNode = targetNode.Parent;

                // Generate a list of parents to expand in the treeview
                while (currentNode != null)
                {
                    parentPath.Add(currentNode);
                    currentNode = currentNode.Parent;
                }

                // Reverse the "path" so we can traverse it in the proper order
                parentPath.Reverse();
                TreeViewItem parentItem = null;

                foreach (ServiceNode node in parentPath)
                {
                    // If this is the first parent, we'll load it from the treeview directly
                    if (parentPath.IndexOf(node) == 0)
                    {
                        parentItem = ServicesTreeView.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
                    }

                    else
                    {
                        // Load the next item from the treeview
                        parentItem = parentItem.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
                    }


                    if (parentItem != null)
                    {
                        parentItem.IsExpanded = true;
                        ServicesTreeView.UpdateLayout();
                    }
                }

                TreeViewItem targetItem;

                if (parentItem == null)
                {
                    targetItem = ServicesTreeView.ItemContainerGenerator.ContainerFromItem(targetNode) as TreeViewItem;
                }

                else
                {
                    targetItem = parentItem.ItemContainerGenerator.ContainerFromItem(targetNode) as TreeViewItem;
                }



                if (targetItem != null)
                {
                    targetItem.IsSelected = true;
                }
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ServicesTreeView = sender as TreeView;
            ViewModel.SelectServiceNode(ServicesTreeView.SelectedItem);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
