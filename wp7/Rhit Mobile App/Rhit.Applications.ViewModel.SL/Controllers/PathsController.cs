using System.Collections.ObjectModel;
using System.Windows.Input;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Utilities;
using System.Windows;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.ViewModel.Controllers {
    public class PathsController : DependencyObject {
        
        private PathsController() {
            Nodes = new LocationCollection();
            DataCollector.Instance.DirectionsReturned += new Model.Events.DirectionsEventHandler(DirectionsReturned);
        }

        private void DirectionsReturned(object sender, Model.Events.DirectionsEventArgs e) {
            Nodes.Clear();
            foreach(DirectionPath_DC path in e.Paths) {
                Node node = new Node(path.Latitude, path.Longitude);
                node.Action = path.ConvertAction();
                Nodes.Add(node);
                if(CurrentNode != null) Select(node);
            }
        }

        public void Select(Node node) {
            CurrentNode = node;
            MapController.Instance.Center = node;
        }

        #region Singleton Instance
        private static PathsController _instance;
        public static PathsController Instance {
            get {
                if(_instance == null)
                    _instance = new PathsController();
                return _instance;
            }
        }
        #endregion

        public LocationCollection Nodes { get; protected set; }


        #region CurrentNode
        public Node CurrentNode {
            get { return (Node) GetValue(CurrentNodeProperty); }
            set { SetValue(CurrentNodeProperty, value); }
        }

        public static readonly DependencyProperty CurrentNodeProperty =
           DependencyProperty.Register("CurrentNode", typeof(Node), typeof(PathsController), new PropertyMetadata(null));
        #endregion
    }

    public class Node : GeoCoordinate {
        public static int LastNumber = 0;
        public Node(double latitude, double longitude) : base(latitude, longitude) {
            Number = ++LastNumber;
        }

        public int Number { get; set; }

        public string Action { get; set; }
    }
}
