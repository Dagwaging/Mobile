using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using System.Collections.Generic;
using Rhit.Applications.Mvvm.Commands;
using System.Windows.Input;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
using System;
#endif

namespace Rhit.Applications.ViewModels.Controllers {
    public class Path : DependencyObject {
        public Path(Path_DC model, SimpleNode node1, SimpleNode node2) {
            
            Nodes = new LocationCollection();
            
            Node1 = node1;
            Node2 = node2;
            Nodes.Add(Node1.Center);
            Nodes.Add(Node2.Center);
            Node1.CenterChanged += new EventHandler(Node_CenterChanged);
            Node2.CenterChanged += new EventHandler(Node_CenterChanged);
            Id = model.Id;
            IsElevator = model.Elevator;
            StairCount = model.Stairs;
            if(Node1.IsOutside || Node2.IsOutside)
                IsOutside = true;
            else IsOutside = false;

            Node1.AddPathBinding(this);
            Node2.AddPathBinding(this);
        }

        private void Node_CenterChanged(object sender, EventArgs e) {
            Nodes.Clear();
            Nodes.Add(Node1.Center);
            Nodes.Add(Node2.Center);
        }

        public LocationCollection Nodes { get; private set; }

        public int Id { get; private set; }

        #region IsOutside
        public bool IsOutside {
            get { return (bool) GetValue(IsOutsideProperty); }
            set { SetValue(IsOutsideProperty, value); }
        }

        public static readonly DependencyProperty IsOutsideProperty =
           DependencyProperty.Register("IsOutside", typeof(bool), typeof(Path), new PropertyMetadata(true));
        #endregion

        #region StairCount
        public int StairCount {
            get { return (int) GetValue(StairCountProperty); }
            set { SetValue(StairCountProperty, value); }
        }

        public static readonly DependencyProperty StairCountProperty =
           DependencyProperty.Register("StairCount", typeof(int), typeof(Path), new PropertyMetadata(0));
        #endregion

        #region IsElevator
        public bool IsElevator {
            get { return (bool) GetValue(IsElevatorProperty); }
            set { SetValue(IsElevatorProperty, value); }
        }

        public static readonly DependencyProperty IsElevatorProperty =
           DependencyProperty.Register("IsElevator", typeof(bool), typeof(Path), new PropertyMetadata(false));
        #endregion

        #region IsSelected
        public bool IsSelected {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(Path), new PropertyMetadata(false));
        #endregion

        #region CanSelect
        public bool CanSelect {
            get { return (bool) GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }

        public static readonly DependencyProperty CanSelectProperty =
           DependencyProperty.Register("CanSelect", typeof(bool), typeof(Path), new PropertyMetadata(false));
        #endregion

        #region Node1
        public SimpleNode Node1 {
            get { return (SimpleNode) GetValue(Node1Property); }
            set { SetValue(Node1Property, value); }
        }

        public static readonly DependencyProperty Node1Property =
           DependencyProperty.Register("Node1", typeof(SimpleNode), typeof(Path), new PropertyMetadata(null));
        #endregion

        #region Node2
        public SimpleNode Node2 {
            get { return (SimpleNode) GetValue(Node2Property); }
            set { SetValue(Node2Property, value); }
        }

        public static readonly DependencyProperty Node2Property =
           DependencyProperty.Register("Node2", typeof(SimpleNode), typeof(Path), new PropertyMetadata(null));
        #endregion

        public bool Contains(SimpleNode node) {
            if(node == Node1 || node == Node2) return true;
            return false;
        }
    }

    public class SimpleNode : DependencyObject {
        public SimpleNode(Node_DC model) {
            Model = model;
            PathBindingDictionary = new Dictionary<SimpleNode, Path>();
            InitializeCommands();
            Center = new Location(Model.Latitude, Model.Longitude, Model.Altitude);
            IsOutside = Model.Outside;
            Id = Model.Id;
        }

        #region Selected Event
        public event EventHandler Selected;
        protected virtual void OnSelected() {
            if(Selected != null) Selected(this, new EventArgs());
        }
        #endregion

        #region Unselected Event
        public event EventHandler Unselected;
        protected virtual void OnUnselected() {
            if(Unselected != null) Unselected(this, new EventArgs());
        }
        #endregion

        #region CenterChanged Event
        public event EventHandler CenterChanged;
        protected virtual void OnCenterChanged() {
            if(CenterChanged != null) CenterChanged(this, new EventArgs());
        }
        #endregion

        private Dictionary<SimpleNode, Path> PathBindingDictionary { get; set; }

        public void AddPathBinding(Path path) {
            SimpleNode other;
            if(path.Node1 == this) other = path.Node2;
            else if(path.Node2 == this) other = path.Node1;
            else return;

            PathBindingDictionary[other] = path;
        }

        public int Id { get; private set; }

        private void InitializeCommands() {
            ClickCommand = new RelayCommand(p => OnClick());
        }

        #region Click Command/Method
        public ICommand ClickCommand { get; private set; }

        private void OnClick() {
            if(CanSelect) {
                //The node is currently not selected and the user can select it
                IsSelected = true;
                OnSelected();
            }
            //TODO: Implement unselect functionality
        }
        #endregion

        #region Center
        public Location Center {
            get { return (Location) GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
           DependencyProperty.Register("Center", typeof(Location), typeof(SimpleNode), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnCenterPropertyChanged)));

        private static void OnCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as SimpleNode).OnCenterChanged();
        }
        #endregion

        #region IsSelected
        public bool IsSelected {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(SimpleNode), new PropertyMetadata(false));
        #endregion

        #region CanSelect
        public bool CanSelect {
            get { return (bool) GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }

        public static readonly DependencyProperty CanSelectProperty =
           DependencyProperty.Register("CanSelect", typeof(bool), typeof(SimpleNode), new PropertyMetadata(true));
        #endregion

        #region IsEndNode
        public bool IsEndNode {
            get { return (bool) GetValue(IsEndNodeProperty); }
            set { SetValue(IsEndNodeProperty, value); }
        }

        public static readonly DependencyProperty IsEndNodeProperty =
           DependencyProperty.Register("IsEndNode", typeof(bool), typeof(SimpleNode), new PropertyMetadata(false));
        #endregion

        #region IsOutside
        public bool IsOutside {
            get { return (bool) GetValue(IsOutsideProperty); }
            set { SetValue(IsOutsideProperty, value); }
        }

        public static readonly DependencyProperty IsOutsideProperty =
           DependencyProperty.Register("IsOutside", typeof(bool), typeof(SimpleNode), new PropertyMetadata(true));
        #endregion

        #region CanMove
        public bool CanMove {
            get { return (bool) GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        public static readonly DependencyProperty CanMoveProperty =
           DependencyProperty.Register("CanMove", typeof(bool), typeof(SimpleNode), new PropertyMetadata(false));
        #endregion

        internal ICollection<SimpleNode> GetAdjacentNodes() {
            return PathBindingDictionary.Keys;
        }

        internal Path GetPath(SimpleNode node) {
            return PathBindingDictionary[node];
        }

        protected Node_DC Model { get; private set; }

        internal bool HasChanges() {
            if(IsOutside != Model.Outside) return true;
            if(Center.Latitude != Model.Latitude) return true;
            if(Center.Longitude != Model.Longitude) return true;
            if(Center.Altitude != Model.Altitude) return true;
            return false;
        }
    }

    public class DirectionMessage : DependencyObject {
        public DirectionMessage(DirectionMessage_DC model) {
            Id = model.Id;
            Message = model.Message1;
            ReverseMessage = model.Message2;
            Offset = model.Offset;
        }

        public DirectionMessage() {
            Id = -1;
            Message = ReverseMessage = "No Message Found";
            Offset = 0;
        }

        public int Id { get; private set; }

        #region Offset
        public int Offset {
            get { return (int) GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty =
           DependencyProperty.Register("Offset", typeof(int), typeof(DirectionMessage), new PropertyMetadata(0));
        #endregion

        #region Message
        public string Message {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
           DependencyProperty.Register("Message", typeof(string), typeof(DirectionMessage), new PropertyMetadata(""));
        #endregion

        #region ReverseMessage
        public string ReverseMessage {
            get { return (string) GetValue(ReverseMessageProperty); }
            set { SetValue(ReverseMessageProperty, value); }
        }

        public static readonly DependencyProperty ReverseMessageProperty =
           DependencyProperty.Register("ReverseMessage", typeof(string), typeof(DirectionMessage), new PropertyMetadata(""));
        #endregion
    }

    public class Direction : DependencyObject {
        public Direction(Direction_DC model) {
            Paths = new ObservableCollection<Path>();
            Id = model.Id;
            foreach(int id in model.Paths)
                Paths.Add(NodesController.Instance.GetPath(id));
            Message = NodesController.Instance.GetMessage(model.Message);
        }

        public int Id { get; private set; }

        public ObservableCollection<Path> Paths { get; private set; }

        #region Message
        public DirectionMessage Message {
            get { return (DirectionMessage) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
           DependencyProperty.Register("Message", typeof(DirectionMessage), typeof(Direction), new PropertyMetadata(null));
        #endregion
    }

    public class NodesController : DependencyObject {
        private NodesController() {
            AllNodes = new List<SimpleNode>();
            AllPaths = new List<Path>();
            AllDirections = new List<Direction>();
            Paths = new ObservableCollection<Path>();
            Nodes = new ObservableCollection<SimpleNode>();
            DataCollector.Instance.PathDataReturned += new PathDataEventHandler(PathDataReturned);
            NodeDictionary = new Dictionary<int, SimpleNode>();
            PathDictionary = new Dictionary<int, Path>();
            MessageDictionary = new Dictionary<int, DirectionMessage>();
            SelectedPaths = new List<Path>();
            NullMessage = new DirectionMessage();
            CurrentMessage = NullMessage;
            SelectedNodes = new List<SimpleNode>();

            DataCollector.Instance.NodeCreated += new NodeEventHandler(Instance_NodeCreated);
            DataCollector.Instance.NodeDeleted += new IdentificationEventHandler(Instance_NodeDeleted);
            DataCollector.Instance.PathCreated += new PathEventHandler(Instance_PathCreated);
            DataCollector.Instance.PathDeleted += new IdentificationEventHandler(Instance_PathDeleted);
            State = BehaviorState.Default;
        }

        private void Instance_PathDeleted(object sender, IdentificationEventArgs e) {
            if(!PathDictionary.ContainsKey(e.Id)) return;
            Path path = PathDictionary[e.Id];
            RestoreToDefault();
            
            PathDictionary.Remove(path.Id);
            AllPaths.Remove(path);
            Paths.Remove(path);
        }

        private void Instance_PathCreated(object sender, PathEventArgs e) {
            Path path = new Path(e.Path, NodeDictionary[e.Path.Node1], NodeDictionary[e.Path.Node2]);
            AllPaths.Add(path);
            PathDictionary[path.Id] = path;
            if(path.IsOutside) 
                Paths.Add(path);
        }

        private void Instance_NodeDeleted(object sender, IdentificationEventArgs e) {
            if(!NodeDictionary.ContainsKey(e.Id)) return;
            SimpleNode node = NodeDictionary[e.Id];
            if(node.IsSelected || node.IsEndNode) {
                RestoreToDefault();
            }
            NodeDictionary.Remove(node.Id);
            AllNodes.Remove(node);
            Nodes.Remove(node);
        }

        private void Instance_NodeCreated(object sender, NodeEventArgs e) {
            SimpleNode _node = new SimpleNode(e.Node);
            _node.Selected += new EventHandler(Node_Selected);
            _node.Unselected += new EventHandler(Node_Unselected);
            NodeDictionary[_node.Id] = _node;
            AllNodes.Add(_node);
            if(_node.IsOutside) Nodes.Add(_node);
        }

        #region Singleton Instance
        private static NodesController _instance;
        public static NodesController Instance {
            get {
                if(_instance == null)
                    _instance = new NodesController();
                return _instance;
            }
        }
        #endregion

        private void Clear() {
            AllNodes.Clear();
            AllPaths.Clear();
            AllDirections.Clear();
            Paths.Clear();
            Nodes.Clear();
            NodeDictionary.Clear();
            PathDictionary.Clear();
            MessageDictionary.Clear();
            SelectedNodes.Clear();
            State = BehaviorState.Default;
        }

        private void PathDataReturned(object sender, PathDataEventArgs e) {
            Clear();

            foreach(Node_DC node in e.Nodes) {
                SimpleNode _node = new SimpleNode(node);
                _node.Selected += new EventHandler(Node_Selected);
                _node.Unselected += new EventHandler(Node_Unselected);
                NodeDictionary[_node.Id] = _node;
                AllNodes.Add(_node);
            }
            foreach(Path_DC path in e.Paths) {
                if(NodeDictionary.ContainsKey(path.Node1) && NodeDictionary.ContainsKey(path.Node2)) {
                    Path _path = new Path(path, NodeDictionary[path.Node1], NodeDictionary[path.Node2]);
                    AllPaths.Add(_path);
                    PathDictionary[_path.Id] = _path;
                    if(_path.IsOutside) {
                        //addNodes(_path);
                        Paths.Add(_path);
                    }
                } else {
                    //TODO: one of the nodes doesn't exist.
                    //  Should remove corrupted/bad path or thorw error
                }
            }

            foreach(SimpleNode node in AllNodes)
                if(node.IsOutside) Nodes.Add(node);

            foreach(DirectionMessage_DC msg in e.Messages) {
                DirectionMessage _msg = new DirectionMessage(msg);
                MessageDictionary[_msg.Id] = _msg;
            }
            foreach(Direction_DC direction in e.Directions)
                AllDirections.Add(new Direction(direction));
        }

        private void AddNodes(Path path) {
            if(!Nodes.Contains(path.Node1))
                Nodes.Add(path.Node1);
            if(!Nodes.Contains(path.Node2))
                Nodes.Add(path.Node2);
        }

        private Dictionary<int, SimpleNode> NodeDictionary { get; set; }

        private Dictionary<int, Path> PathDictionary { get; set; }

        private Dictionary<int, DirectionMessage> MessageDictionary { get; set; }

        internal void GetPathData() {
            DataCollector.Instance.GetPathData();
        }

        private List<Path> AllPaths { get; set; }

        private List<Direction> AllDirections { get; set; }

        public ObservableCollection<Path> Paths { get; protected set; }

        private List<SimpleNode> AllNodes { get; set; }

        public ObservableCollection<SimpleNode> Nodes { get; protected set; }

        private List<Path> SelectedPaths { get; set; }

        private List<SimpleNode> SelectedNodes { get; set; }

        #region CurrentMessage
        public DirectionMessage CurrentMessage {
            get { return (DirectionMessage) GetValue(CurrentMessageProperty); }
            set { SetValue(CurrentMessageProperty, value); }
        }

        public static readonly DependencyProperty CurrentMessageProperty =
           DependencyProperty.Register("CurrentMessage", typeof(DirectionMessage), typeof(NodesController), new PropertyMetadata(null));
        #endregion

        #region LastNode
        public SimpleNode LastNode {
            get { return (SimpleNode) GetValue(LastNodeProperty); }
            set { SetValue(LastNodeProperty, value); }
        }

        public static readonly DependencyProperty LastNodeProperty =
           DependencyProperty.Register("LastNode", typeof(SimpleNode), typeof(NodesController), new PropertyMetadata(null));
        #endregion

        public DirectionMessage NullMessage { get; set; }

        private void Node_Unselected(object sender, EventArgs e) {
            SimpleNode node = sender as SimpleNode;

        }

        private void Node_Selected(object sender, EventArgs e) {
            SimpleNode node = sender as SimpleNode;
            switch(State) {
                case BehaviorState.DeletingNode:
                    DataCollector.Instance.DeleteNode(node.Id);
                    RestoreToDefault();
                    break;

                case BehaviorState.DeletingPath:
                    SelectedNodes.Add(node);
                    if(SelectedNodes.Count >= 2) {
                        Path path;
                        if(SelectedNodes[0] == node) path = node.GetPath(SelectedNodes[1]);
                        else path = node.GetPath(SelectedNodes[0]);
                        DataCollector.Instance.DeletePath(path.Id);
                        RestoreToDefault();
                    }
                    break;

                case BehaviorState.CreatingPath:
                    SelectedNodes.Add(node);
                    if(SelectedNodes.Count >= 2) {
                        CreatePath();
                        RestoreToDefault();
                    }
                    break;

                case BehaviorState.Default:
                default:
                    SelectedNodes.Add(node);
                    node.IsEndNode = true;

                    foreach(SimpleNode other in AllNodes)
                        other.CanSelect = false;

                    if(LastNode != null) {
                        LastNode.IsEndNode = false;
                        SelectedPaths.Add(LastNode.GetPath(node));
                    }
                    LastNode = node;
                    foreach(SimpleNode other in LastNode.GetAdjacentNodes())
                        other.CanSelect = true;

                    CurrentMessage = MessageSearch();
                    break;
            }
        }

        private void CreatePath() {
            DataCollector.Instance.CreatePath(SelectedNodes[0].Id, SelectedNodes[1].Id, false, 0, 100);
            RestoreToDefault();
        }

        public DirectionMessage MessageSearch() {
            bool isMatch;
            foreach(Direction direction in AllDirections) {
                isMatch = true;
                if(direction.Paths.Count != SelectedPaths.Count)
                    continue;
                for(int i = 0; i < SelectedPaths.Count; i++) {
                    if(direction.Paths[i] != SelectedPaths[i]) {
                        isMatch = false;
                        break;
                    }
                }
                if(isMatch) return direction.Message;
                isMatch = true;
                for(int i = 0, j = SelectedPaths.Count - 1; i < SelectedPaths.Count; i++, j--) {
                    if(direction.Paths[i] != SelectedPaths[j]) {
                        isMatch = false;
                        break;
                    }
                }
                if(isMatch) return direction.Message;
            }
            return NullMessage;
        }

        protected enum BehaviorState { Default, DeletingNode, CreatingPath, DeletingPath, };

        protected BehaviorState State { get; set; }

        public void DeleteNextNode() {
            RestoreToDefault();
            State = BehaviorState.DeletingNode;
        }

        public void RestoreToDefault() {
            State = BehaviorState.Default;
            SelectedPaths.Clear();
            SelectedNodes.Clear();
            LastNode = null;
            foreach(SimpleNode node in AllNodes) {
                node.CanSelect = true;
                node.IsEndNode = false;
                node.IsSelected = false;
            }
            foreach(Path path in AllPaths) {
                path.CanSelect = false;
                path.IsSelected = false;
            }
            AllowMovement(false);
        }

        public Path GetPath(int id) {
            return PathDictionary[id];
        }

        public DirectionMessage GetMessage(int id) {
            return MessageDictionary[id];
        }

        internal void CreateNewPath() {
            RestoreToDefault();
            State = BehaviorState.CreatingPath;
        }

        internal void DeleteNextPath() {
            RestoreToDefault();
            State = BehaviorState.DeletingPath;
        }

        internal void AllowMovement() {
            AllowMovement(true);
        }

        internal void AllowMovement(bool canMove) {
            if(canMove) {
                foreach(SimpleNode node in Nodes)
                    node.CanMove = true;
            } else {
                foreach(SimpleNode node in AllNodes)
                    node.CanMove = false;
            }
        }

        internal void SaveNodes() {
            foreach(SimpleNode node in Nodes) {
                if(node.HasChanges())
                    DataCollector.Instance.UpdateNode(node.Id, node.Center.Latitude, node.Center.Longitude, node.Center.Altitude, node.IsOutside, null);
            }
            RestoreToDefault();
        }
    }
}
