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
using Rhit.Applications.Model;
using System.Collections.ObjectModel;

namespace Rhit.Applications.ViewModel
{
    public class LocationNode
    {
        public LocationNode(RhitLocation location)
        {
            ChildLocations = new ObservableCollection<LocationNode>();
            Location = location;
            Name = Location.Label;
            Id = Location.Id;
        }

        public RhitLocation Location { get; private set; }

        public ObservableCollection<LocationNode> ChildLocations { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}
