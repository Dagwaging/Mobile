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
using Rhit.Applications.Models;
using System.Collections.ObjectModel;

namespace Rhit.Applications.ViewModels.Utilities
{
    public class ServiceLinkNode : ServiceNode
    {
        public ServiceLinkNode(Link serviceLink)
        {
            Children = new ObservableCollection<ServiceNode>();
            Link = serviceLink;
            Name = Link.Name;
        }

        public Link Link { get; private set; }

        public override ObservableCollection<ServiceNode> GetRecursiveChildren()
        {
            return new ObservableCollection<ServiceNode>();
        }
    }
}
