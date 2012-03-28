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
using System.Collections.ObjectModel;
using Rhit.Applications.Models;

namespace Rhit.Applications.ViewModels.Utilities
{
    public class ServiceCategoryNode : ServiceNode
    {
        public ServiceCategoryNode(CampusService serviceCategory)
        {
            Children = new ObservableCollection<ServiceNode>();
            Category = serviceCategory;
            Name = Category.Label;

            foreach (CampusService service in Category.Children)
            {
                ServiceCategoryNode node = new ServiceCategoryNode(service);
                node.Parent = this;
                Children.Add(node);
            }

            foreach (Link link in Category.Links)
            {
                ServiceLinkNode node = new ServiceLinkNode(link);
                node.Parent = this;
                Children.Add(node);
            }
        }

        public CampusService Category { get; private set; }
    }
}
