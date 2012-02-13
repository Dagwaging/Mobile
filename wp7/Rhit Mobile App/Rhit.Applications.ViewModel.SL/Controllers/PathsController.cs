using System;
using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.ViewModel.Utilities;
using System.Collections.Generic;

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
            Nodes = new ObservableCollection<PathNode>();
            DataCollector.Instance.DirectionsReturned += new DirectionsEventHandler(DirectionsReturned);
            DataCollector.Instance.ToursReturned += new DirectionsEventHandler(DirectionsReturned);
        }

        internal void GetTestDirections() {
            DataCollector.Instance.GetTestDirections();
        }

        internal void GetDirections(int to) {
            DataCollector.Instance.GetDirections(18, to);
        }

        internal void GetTestTour() {
            DataCollector.Instance.GetTestTour();
        }

        internal void GetDirections(int from, IList<int> tagIds) {
            DataCollector.Instance.GetTour(from, tagIds);
        }

        #region internal event NodesUpdated
        internal event EventHandler NodesUpdated;
        protected virtual void OnNodesUpdated() {
            if(NodesUpdated != null) NodesUpdated(this, new EventArgs());
        }
        #endregion

        private void DirectionsReturned(object sender, DirectionsEventArgs e) {
            PathNode.Restart();
            Coordinates.Clear();
            Nodes.Clear();
            Start = null;
            PathNode lastNode = null;
            foreach(DirectionPath_DC path in e.Paths) {
                string action = path.ConvertAction();
                if(string.IsNullOrWhiteSpace(action))
                    Coordinates.Add(new GeoCoordinate(path.Latitude, path.Longitude));
                else {
                    PathNode node = new PathNode(path.Latitude, path.Longitude) {
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
        public PathNode Start {
            get { return (PathNode) GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public static readonly DependencyProperty StartProperty =
           DependencyProperty.Register("Start", typeof(PathNode), typeof(PathsController), new PropertyMetadata(null));
        #endregion

        #region End
        public PathNode End {
            get { return (PathNode) GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public static readonly DependencyProperty EndProperty =
           DependencyProperty.Register("End", typeof(PathNode), typeof(PathsController), new PropertyMetadata(null));
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

        public ObservableCollection<PathNode> Nodes { get; protected set; }
    }
}
