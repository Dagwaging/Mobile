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

namespace Rhit.Applications.ViewModels.Utilities
{
    public abstract class ServiceNode
    {
        public static ServiceNode New(CampusService service)
        {
            return new ServiceCategoryNode(service);
        }

        public ObservableCollection<ServiceNode> Children { get; set; }
        public string Name { get; set; }
    }
}
