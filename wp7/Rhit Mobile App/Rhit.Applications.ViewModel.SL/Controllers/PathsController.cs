using System;
using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;

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
            Coordinates = new LocationCollection();
            Nodes = new ObservableCollection<Node>();
            DataCollector.Instance.DirectionsReturned += new Model.Events.DirectionsEventHandler(DirectionsReturned);
        }

        internal void GetTestDirections() {
            DataCollector.Instance.GetTestDirections();
        }

        internal void GetDirections(int to) {
            DataCollector.Instance.GetDirections(18, to);
        }

        #region internal event NodesUpdated
        internal event EventHandler NodesUpdated;
        protected virtual void OnNodesUpdated() {
            if(NodesUpdated != null) NodesUpdated(this, new EventArgs());
        }
        #endregion

        private void DirectionsReturned(object sender, DirectionsEventArgs e) {
            Node.Restart();
            Coordinates.Clear();
            Nodes.Clear();
            Start = null;
            Node lastNode = null;
            foreach(DirectionPath_DC path in e.Paths) {
                string action = path.ConvertAction();
                if(string.IsNullOrWhiteSpace(action))
                    Coordinates.Add(new GeoCoordinate(path.Latitude, path.Longitude));
                else {
                    Node node = new Node(path.Latitude, path.Longitude) {
                        Action = action,
                    };
                    Coordinates.Add(node.Center);
                    Nodes.Add(node);
                    if(lastNode != null) {
                        lastNode.Next = node;
                        node.Previous = lastNode;
                    }
                    lastNode = node;
                    if(Start == null) Start = node;
                }
            }
            End = lastNode;
            OnNodesUpdated();
        }

        #region Start
        public Node Start {
            get { return (Node) GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public static readonly DependencyProperty StartProperty =
           DependencyProperty.Register("Start", typeof(Node), typeof(PathsController), new PropertyMetadata(null));
        #endregion

        #region End
        public Node End {
            get { return (Node) GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public static readonly DependencyProperty EndProperty =
           DependencyProperty.Register("End", typeof(Node), typeof(PathsController), new PropertyMetadata(null));
        #endregion

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

        public LocationCollection Coordinates { get; protected set; }

        public ObservableCollection<Node> Nodes { get; protected set; }
    }

    public class Node : DependencyObject {
        private static int LastNumber = 0;
        public Node(double latitude, double longitude) {
            Number = ++LastNumber;
            Center = new GeoCoordinate(latitude, longitude);
        }

        internal static void Restart() {
            LastNumber = 0;
        }

        public int Number { get; set; }

        public string Action { get; set; }

        internal Node Next { get; set; }

        internal Node Previous { get; set; }

        public GeoCoordinate Center { get; private set; }

        #region IsSelected
        public bool IsSelected {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(Node), new PropertyMetadata(false));
        #endregion
    }
}
