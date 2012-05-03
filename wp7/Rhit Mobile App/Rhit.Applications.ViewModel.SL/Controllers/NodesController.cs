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
using Rhit.Applications.ViewModels.Utilities;
#endif

namespace Rhit.Applications.ViewModels.Controllers {
    public class Path : DependencyObject {
        //Note: Always assumes nodes are constant
        //Note: IsOutside is only calculated once (it cant be calculate again since it makes actual inside nodes, outside nodes)
        public Path(Path_DC model, SimpleNode node1, SimpleNode node2) {
            Nodes = new LocationCollection();
            
            Node1 = node1;
            Node2 = node2;
            Nodes.Add(Node1.Center);
            Nodes.Add(Node2.Center);

            Node1.Moved += new EventHandler(Node_CenterChanged);
            Node2.Moved += new EventHandler(Node_CenterChanged);

            TryUpdateModel(model);

            ComputeIsOutside();

            Node1.AddPathBinding(this);
            Node2.AddPathBinding(this);
        }

        internal bool TryUpdateModel(Path_DC model) {
            if(Model != null) {
                if(model.Id != Model.Id) return false;
                if(model.Node1 != Model.Node1) return false;
                if(model.Node2 != Model.Node2) return false;
            }

            Model = model;
            Revert();

            return true;
        }

        internal void Revert() {
            Id = Model.Id;
            IsElevator = Model.Elevator;
            StairCount = Model.Stairs;
            Partition = Model.Partition;
            IsSelected = false;
        }

        internal void ComputeIsOutside() {
            if(Node1.IsOutside || Node2.IsOutside) {
                IsOutside = true;
                Node1.IsOutside = true;
                Node2.IsOutside = true;
            } else IsOutside = false;
        }

        private void Node_CenterChanged(object sender, EventArgs e) {
            Nodes.Clear();
            Nodes.Add(Node1.Center);
            Nodes.Add(Node2.Center);
        }

        public LocationCollection Nodes { get; private set; }

        public int Id { get; private set; }

        private Path_DC Model { get; set; }

        #region Partition
        public int Partition {
            get { return (int) GetValue(PartitionProperty); }
            set { SetValue(PartitionProperty, value); }
        }

        public static readonly DependencyProperty PartitionProperty =
           DependencyProperty.Register("Partition", typeof(int), typeof(Path), new PropertyMetadata(0));
        #endregion

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
            Location = LocationsController.Instance.GetLocation(model.Location);
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

        #region Moved Event
        public event EventHandler Moved;
        protected virtual void OnMove() {
            if(Moved != null) Moved(this, new EventArgs());
        }
        #endregion

        internal void Revert() {
            CanSelect = false;
            IsEndNode = false;
            IsSelected = false;
            CanMove = false;
            Center = new Location(Model.Latitude, Model.Longitude, Model.Altitude);
            IsOutside = Model.Outside;
            OnMove();
        }

        internal bool UpdateModel(Node_DC model) {
            if(model.Id != Model.Id) return false;

            Model = model;
            Revert();

            return true;
        }

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
            SimpleNode node = d as SimpleNode;
            if(node.Center.Latitude == node.Model.Latitude)
                if(node.Center.Longitude == node.Model.Longitude)
                    if(node.Center.Altitude == node.Model.Altitude)
                        return;
            node.OnMove();
        }
        #endregion

        #region Location
        public LocationData Location {
            get { return (LocationData) GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty LocationProperty =
           DependencyProperty.Register("Location", typeof(LocationData), typeof(SimpleNode), new PropertyMetadata(null));
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
            if(PathBindingDictionary.ContainsKey(node))
                return PathBindingDictionary[node];
            return null;
        }

        protected Node_DC Model { get; private set; }

        internal bool HasChanges() {
            if(IsOutside != Model.Outside) return true;
            if(Center.Latitude != Model.Latitude) return true;
            if(Center.Longitude != Model.Longitude) return true;
            if(Center.Altitude != Model.Altitude) return true;
            if(Location.Id != Model.Location) return true;
            return false;
        }

        internal void RemoveAdjacentNode(SimpleNode node) {
            if(PathBindingDictionary.ContainsKey(node))
                PathBindingDictionary.Remove(node);
        }
    }

    public class DirectionMessage : DependencyObject {
        public DirectionMessage(DirectionMessage_DC model) {
            TryUpdateModel(model);
        }

        public DirectionMessage() {
            Id = -1;
            Message = "No Message Found";
            ReverseMessage = "No Message Found";
            Offset = 0;
            Action = null;
            ReverseAction = null;
        }

        internal void Revert() {
            if(Model == null) return;

            Id = Model.Id;
            Message = Model.Message1;
            ReverseMessage = Model.Message2;
            Offset = Model.Offset;
            Action = Model.Action1;
            ReverseAction = Model.Action2;
        }

        internal bool TryUpdateModel(DirectionMessage_DC model) {
            if(Model != null)
                if(model.Id != Model.Id) return false;
            if(Id < 0) return false;

            Model = model;
            Revert();

            return true;
        }

        private DirectionMessage_DC Model { get; set; }

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

        #region Action
        public string Action {
            get { return (string) GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public static readonly DependencyProperty ActionProperty =
           DependencyProperty.Register("Action", typeof(string), typeof(DirectionMessage),
           new PropertyMetadata("", new PropertyChangedCallback(OnActionChanged)));

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            string value = e.NewValue as string;
            if(value == "Null" || value == "None") {
                (d as DirectionMessage).Action = null;
            }
        }
        #endregion

        #region ReverseAction
        public string ReverseAction {
            get { return (string) GetValue(ReverseActionProperty); }
            set { SetValue(ReverseActionProperty, value); }
        }

        public static readonly DependencyProperty ReverseActionProperty =
           DependencyProperty.Register("ReverseAction", typeof(string), typeof(DirectionMessage),
           new PropertyMetadata("", new PropertyChangedCallback(OnReverseActionChanged)));

        private static void OnReverseActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            string value = e.NewValue as string;
            if(value == "Null" || value == "None") {
                (d as DirectionMessage).ReverseAction = null;
            }
        }
        #endregion

    }

    public class Direction : DependencyObject {
        public Direction(Direction_DC model) {
            Paths = new ObservableCollection<Path>();
            TryUpdateModel(model);
            
        }

        public Direction(DirectionMessage directionMessage) {
            Paths = new ObservableCollection<Path>();
            Id = -1;
            Message = directionMessage;
            Within = null;
        }

        public int Id { get; private set; }

        internal void Revert() {
            if(Model == null) return;

            Id = Model.Id;
            Paths.Clear();
            if(Model.Paths != null) {
                foreach(int id in Model.Paths)
                    Paths.Add(NodesController.Instance.GetPath(id));
            }
            Message = NodesController.Instance.GetMessage(Model.MessageId);
            Within = Model.Within;
        }

        internal bool TryUpdateModel(Direction_DC model) {
            if(Model != null)
                if(model.Id != Model.Id) return false;
            if(Id < 0) return false;

            Model = model;
            Revert();

            return true;
        }

        private Direction_DC Model { get; set; }

        public ObservableCollection<Path> Paths { get; private set; }

        public bool IsBlank() {
            if(Id < 0) return true;
            return false;
        }

        #region Message
        public DirectionMessage Message {
            get { return (DirectionMessage) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
           DependencyProperty.Register("Message", typeof(DirectionMessage), typeof(Direction), new PropertyMetadata(null));
        #endregion

        #region Within
        public int? Within {
            get { return (int?) GetValue(WithinProperty); }
            set { SetValue(WithinProperty, value); }
        }

        public static readonly DependencyProperty WithinProperty =
           DependencyProperty.Register("Within", typeof(int?), typeof(Direction), new PropertyMetadata(null));
        #endregion
    }

    public class NodesController : DependencyObject {
        protected enum BehaviorState { Default, DeletingNode, CreatingPath, DeletingPath, MovingNodes, };

        private NodesController() {
            InitializeCollections();
            InitializeDictionaries();
            InitializeEventHandlers();
            InitializeProperties();

            GetPathData();
        }

        #region Initializers
        private void InitializeCollections() {
            AllActions = new List<string>() { "UE", "DE", "GS", "CS", "FP", "L1", "L2", "R1", "R2", "L3", "R3", "EN", "EX", "US", "DS", "None", };

            AllNodes = new List<SimpleNode>();
            SelectedNodes = new List<SimpleNode>();
            Nodes = new ObservableCollection<SimpleNode>();

            AllPaths = new List<Path>();
            SelectedPaths = new List<Path>();
            Paths = new ObservableCollection<Path>();

            AllDirections = new List<Direction>();

            AllMessages = new ObservableCollection<DirectionMessage>();
            Messages = new ObservableCollection<DirectionMessage>();
        }

        private void InitializeDictionaries() {
            NodeDictionary = new Dictionary<int, SimpleNode>();
            PathDictionary = new Dictionary<int, Path>();
            DirectionDictionary = new Dictionary<int, Direction>();
            DirectionMessageDictionary = new Dictionary<int, DirectionMessage>();
        }

        private void InitializeEventHandlers() {
            DataCollector.Instance.PathDataReturned += new PathDataEventHandler(PathDataReturned);

            DataCollector.Instance.NodeCreated += new NodeEventHandler(Instance_NodeCreated);
            DataCollector.Instance.NodeUpdated += new NodeEventHandler(Instance_NodeUpdated);
            DataCollector.Instance.NodeDeleted += new IdentificationEventHandler(Instance_NodeDeleted);

            DataCollector.Instance.PathCreated += new PathEventHandler(Instance_PathCreated);
            DataCollector.Instance.PathUpdated += new PathEventHandler(Instance_PathUpdated);
            DataCollector.Instance.PathDeleted += new IdentificationEventHandler(Instance_PathDeleted);

            DataCollector.Instance.DirectionCreated += new DirectionEventHandler(Instance_DirectionCreated);
            DataCollector.Instance.DirectionUpdated += new DirectionEventHandler(Instance_DirectionUpdated);
            DataCollector.Instance.DirectionDeleted += new IdentificationEventHandler(Instance_DirectionDeleted);

            DataCollector.Instance.DirectionMessageCreated += new DirectionMessageEventHandler(Instance_DirectionMessageCreated);
            DataCollector.Instance.DirectionMessageUpdated += new DirectionMessageEventHandler(Instance_DirectionMessageUpdated);
            DataCollector.Instance.DirectionMessageDeleted += new IdentificationEventHandler(Instance_DirectionMessageDeleted);
        }

        private void InitializeProperties() {
            State = BehaviorState.Default;

            CurrentDirection = GetBlankDirection();

            BlankMessage = new DirectionMessage();
            CurrentMessage = BlankMessage;
            DirectionMessageDictionary[BlankMessage.Id] = BlankMessage;
            AllMessages.Add(BlankMessage);
        }
        #endregion

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

        #region DataCollector Event Handlers
        private void PathDataReturned(object sender, PathDataEventArgs e) {
            ClearCollections();
            ClearDictionaries();
            RestoreToDefault();
            DirectionMessageDictionary[BlankMessage.Id] = BlankMessage;
            AllMessages.Add(BlankMessage);

            ProccessNodes(e.Nodes);
            ProccessPaths(e.Paths);

            foreach(SimpleNode node in AllNodes)
                if(node.IsOutside) Nodes.Add(node);

            ProccessDirectionMessages(e.Messages);
            ProccessDirections(e.Directions);
        }

        
        private void Instance_DirectionDeleted(object sender, IdentificationEventArgs e) {
            if(e.Id < 0) return;
            if(!DirectionDictionary.ContainsKey(e.Id)) return;
            Direction direction = DirectionDictionary[e.Id];

            DirectionDictionary.Remove(direction.Id);
            AllDirections.Remove(direction);
        }

        private void Instance_DirectionUpdated(object sender, DirectionEventArgs e) {
            if(!DirectionDictionary.ContainsKey(e.Direction.Id)) return;
            DirectionDictionary[e.Direction.Id].TryUpdateModel(e.Direction);

            GetPathData();
        }

        private void Instance_DirectionCreated(object sender, DirectionEventArgs e) {
            Direction direction = new Direction(e.Direction);

            DirectionDictionary[direction.Id] = direction;
            AllDirections.Add(direction);

            GetPathData();
        }


        private void Instance_DirectionMessageDeleted(object sender, IdentificationEventArgs e) {
            if(e.Id < 0) return;
            if(!DirectionMessageDictionary.ContainsKey(e.Id)) return;
            DirectionMessage msg = DirectionMessageDictionary[e.Id];
            
            DirectionMessageDictionary.Remove(msg.Id);
            AllMessages.Remove(msg);
            Messages.Remove(msg);
        }

        private void Instance_DirectionMessageUpdated(object sender, DirectionMessageEventArgs e) {
            if(!DirectionMessageDictionary.ContainsKey(e.DirectionMessage.Id)) return;
            DirectionMessageDictionary[e.DirectionMessage.Id].TryUpdateModel(e.DirectionMessage);
        }

        private void Instance_DirectionMessageCreated(object sender, DirectionMessageEventArgs e) {
            DirectionMessage msg = new DirectionMessage(e.DirectionMessage);

            DirectionMessageDictionary[msg.Id] = msg;
            AllMessages.Add(msg);
            Messages.Add(msg);
        }


        private void Instance_PathUpdated(object sender, PathEventArgs e) {
            if(!PathDictionary.ContainsKey(e.Path.Id)) return;
            PathDictionary[e.Path.Id].TryUpdateModel(e.Path);
            RestoreToDefault();
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
            RestoreToDefault();
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
            foreach(SimpleNode _node in node.GetAdjacentNodes())
                _node.RemoveAdjacentNode(node);
            RestoreToDefault();
        }

        private void Instance_NodeUpdated(object sender, NodeEventArgs e) {
            if(!NodeDictionary.ContainsKey(e.Node.Id)) return;
            SimpleNode node = NodeDictionary[e.Node.Id];
            node.UpdateModel(e.Node);

            if(node.IsOutside && !Nodes.Contains(node)) Nodes.Add(node);
            if(!node.IsOutside && Nodes.Contains(node)) Nodes.Remove(node);
        }

        private void Instance_NodeCreated(object sender, NodeEventArgs e) {
            SimpleNode node = new SimpleNode(e.Node);
            node.Selected += new EventHandler(Node_Selected);
            node.Unselected += new EventHandler(Node_Unselected);
            NodeDictionary[node.Id] = node;
            AllNodes.Add(node);
            if(node.IsOutside) Nodes.Add(node);
            RestoreToDefault();
        }
        #endregion

        #region Dictionaries
        private Dictionary<int, SimpleNode> NodeDictionary { get; set; }

        private Dictionary<int, Path> PathDictionary { get; set; }

        private Dictionary<int, Direction> DirectionDictionary { get; set; }

        private Dictionary<int, DirectionMessage> DirectionMessageDictionary { get; set; }
        #endregion

        #region Collections
        public List<string> AllActions { get; set; }

        private List<SimpleNode> AllNodes { get; set; }
        
        private List<SimpleNode> SelectedNodes { get; set; }

        public ObservableCollection<SimpleNode> Nodes { get; protected set; }

        private List<Path> AllPaths { get; set; }

        private List<Path> SelectedPaths { get; set; }

        public ObservableCollection<Path> Paths { get; protected set; }

        private List<Direction> AllDirections { get; set; }

        public ObservableCollection<DirectionMessage> AllMessages { get; protected set; }

        public ObservableCollection<DirectionMessage> Messages { get; protected set; }
        #endregion

        #region Data Proccessing Methods
        private void ProccessNodes(IEnumerable<Node_DC> nodes) {
            foreach(Node_DC model in nodes) {
                SimpleNode node = new SimpleNode(model);
                node.Selected += new EventHandler(Node_Selected);
                node.Unselected += new EventHandler(Node_Unselected);
                node.Moved += new EventHandler(Node_Moved);
                NodeDictionary[node.Id] = node;
                AllNodes.Add(node);
            }
        }

        private void ProccessPaths(IEnumerable<Path_DC> paths) {
            foreach(Path_DC model in paths) {
                if(NodeDictionary.ContainsKey(model.Node1) && NodeDictionary.ContainsKey(model.Node2)) {
                    Path path = new Path(model, NodeDictionary[model.Node1], NodeDictionary[model.Node2]);
                    AllPaths.Add(path);
                    PathDictionary[path.Id] = path;
                    if(path.IsOutside) Paths.Add(path);
                } else {
                    //TODO: one of the nodes doesn't exist.
                    //  Should remove corrupted/bad path or throw error
                }
            }
        }

        private void ProccessDirections(IEnumerable<Direction_DC> directions) {
            foreach(Direction_DC model in directions) {
                Direction direction = new Direction(model);
                AllDirections.Add(direction);
                DirectionDictionary[direction.Id] = direction;
            }
        }

        private void ProccessDirectionMessages(IEnumerable<DirectionMessage_DC> directionMessages) {
            foreach(DirectionMessage_DC model in directionMessages) {
                DirectionMessage directionMessage = new DirectionMessage(model);
                DirectionMessageDictionary[directionMessage.Id] = directionMessage;
                AllMessages.Add(directionMessage);
                Messages.Add(directionMessage);
            }
        }
        #endregion

        private void ClearCollections() {
            AllNodes.Clear();
            SelectedNodes.Clear();
            Nodes.Clear();

            AllPaths.Clear();
            SelectedPaths.Clear();
            Paths.Clear();

            AllDirections.Clear();

            AllMessages.Clear();
            Messages.Clear();
        }

        private void ClearDictionaries() {
            NodeDictionary.Clear();
            PathDictionary.Clear();
            DirectionDictionary.Clear();
            DirectionMessageDictionary.Clear();
        }

        private DirectionMessage BlankMessage { get; set; }


        internal void SavePath(int id) {
            if(State != BehaviorState.Default) return;
            if(!PathDictionary.ContainsKey(id)) return;
            Path path = PathDictionary[id];
            DataCollector.Instance.UpdatePath(id, path.Node1.Id, path.Node2.Id, path.IsElevator, path.StairCount, path.Partition);
            RestoreToDefault();
        }

        internal void SaveDirection() {
            if(State != BehaviorState.Default) return;
            if(SelectedPaths.Count <= 0) return;
            if(CurrentDirection.IsBlank()) {
                IList<int> paths = new List<int>();
                foreach(Path path in SelectedPaths) paths.Add(path.Id);
                DataCollector.Instance.CreateDirection(CurrentMessage.Id, paths, CurrentDirection.Within);
            } else {
                DataCollector.Instance.UpdateDirection(CurrentDirection.Id, CurrentMessage.Id, CurrentDirection.Within);
            }
            RestoreToDefault();
        }

        internal void DeleteDirection() {
            if(State != BehaviorState.Default) return;
            if(CurrentDirection.IsBlank()) return;
            DataCollector.Instance.DeleteDirection(CurrentDirection.Id);
            RestoreToDefault();
        }

        internal bool DeletePath() {
            if(SelectedNodes.Count < 2) return false;
            Path path = SelectedNodes[0].GetPath(SelectedNodes[1]);
            if(path == null) return false;
            DataCollector.Instance.DeletePath(path.Id);
            RestoreToDefault();
            return true;
        }

        internal bool CreatePath() {
            if(SelectedNodes.Count < 2) return false;
            DataCollector.Instance.CreatePath(SelectedNodes[0].Id, SelectedNodes[1].Id, false, 0, 100);
            RestoreToDefault();
            return true;
        }

        internal bool DeleteNode() {
            if(SelectedNodes.Count < 1) return false;
            SimpleNode node = SelectedNodes[0];
            DataCollector.Instance.DeleteNode(node.Id);
            RestoreToDefault();
            return true;
        }




        internal void SelectPath(int id) {
            if(State != BehaviorState.Default) return;
            if(SelectedNodes.Count > 0) return;
            if(!PathDictionary.ContainsKey(id)) return;
            
            Path path = PathDictionary[id];
            if(SelectedPath != null) SelectedPath.IsSelected = false;
            SelectedPath = path;
            SelectedPath.IsSelected = true;
        }

        

        private void Node_Moved(object sender, EventArgs e) {
            SimpleNode node = sender as SimpleNode;
            if(State == BehaviorState.MovingNodes) {
                if(!SelectedNodes.Contains(node)) SelectedNodes.Add(node);
                if(!node.IsSelected) node.IsSelected = true;
            }
        }

        private void AddNodes(Path path) {
            if(!Nodes.Contains(path.Node1))
                Nodes.Add(path.Node1);
            if(!Nodes.Contains(path.Node2))
                Nodes.Add(path.Node2);
        }



        internal void GetPathData() {
            DataCollector.Instance.GetPathData();
        }


        #region SelectedPath
        public Path SelectedPath {
            get { return (Path) GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        public static readonly DependencyProperty SelectedPathProperty =
           DependencyProperty.Register("SelectedPath", typeof(Path), typeof(NodesController), new PropertyMetadata(null));
        #endregion

        #region CurrentDirection
        public Direction CurrentDirection {
            get { return (Direction) GetValue(CurrentDirectionProperty); }
            set { SetValue(CurrentDirectionProperty, value); }
        }

        public static readonly DependencyProperty CurrentDirectionProperty =
           DependencyProperty.Register("CurrentDirection", typeof(Direction), typeof(NodesController), new PropertyMetadata(null));
        #endregion

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

        internal Direction GetBlankDirection() {
            Direction direction = new Direction(BlankMessage);
            CurrentDirection = direction;
            return direction;
        }



        

        private void Node_Unselected(object sender, EventArgs e) {
            SimpleNode node = sender as SimpleNode;

        }

        private void ProccessSelectedNode(SimpleNode node) {
            ProccessSelectedNode(node, false);
        }

        private void ProccessSelectedNode(SimpleNode node, bool multipleEndNodes) {
            SelectedNodes.Add(node);
            node.IsSelected = true;
            node.IsEndNode = true;

            if(LastNode != null) {
                Path path = LastNode.GetPath(node);
                if(path != null) {
                    SelectedPaths.Add(path);
                    path.IsSelected = true;
                }
                LastNode.IsEndNode = multipleEndNodes;
            }
            LastNode = node;
            CurrentDirection = DirectionSearch();
            CurrentMessage = CurrentDirection.Message;
        }

        private void CalculateSelectableNodes() {
            foreach(SimpleNode other in AllNodes)
                other.CanSelect = false;
            foreach(SimpleNode other in LastNode.GetAdjacentNodes())
                other.CanSelect = true;
        }

        private void Node_Selected(object sender, EventArgs e) {
            SimpleNode node = sender as SimpleNode;
            switch(State) {
                case BehaviorState.MovingNodes:
                    if(!SelectedNodes.Contains(node)) SelectedNodes.Add(node);
                    if(!node.IsSelected) node.IsSelected = true;
                    break;

                case BehaviorState.DeletingNode:
                    if(SelectedNodes.Count >= 1) break;
                    ProccessSelectedNode(node);
                    break;

                case BehaviorState.DeletingPath:
                    if(SelectedNodes.Count >= 2 || SelectedNodes.Contains(node)) break;
                    ProccessSelectedNode(node, true);
                    if(SelectedNodes.Count >= 2) break;
                    CalculateSelectableNodes();
                    break;

                case BehaviorState.CreatingPath:
                    if(SelectedNodes.Count >= 2 || SelectedNodes.Contains(node)) break;
                    ProccessSelectedNode(node, true);
                    break;

                case BehaviorState.Default:
                default:
                    if(SelectedPath != null) {
                        SelectedPath.IsSelected = false;
                        SelectedPath = null;
                    }
                    ProccessSelectedNode(node);
                    CalculateSelectableNodes();
                    break;
            }
        }



        public Direction DirectionSearch() {
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
                if(isMatch) return direction;
                isMatch = true;
                for(int i = 0, j = SelectedPaths.Count - 1; i < SelectedPaths.Count; i++, j--) {
                    if(direction.Paths[i] != SelectedPaths[j]) {
                        isMatch = false;
                        break;
                    }
                }
                if(isMatch) return direction;
            }
            return GetBlankDirection();
        }

        

        protected BehaviorState State { get; set; }

        public void DeleteNextNode() {
            RestoreToDefault();
            State = BehaviorState.DeletingNode;
        }

        internal void RestoreToDefault() {
            State = BehaviorState.Default;
            SelectedPath = null;
            SelectedPaths.Clear();
            SelectedNodes.Clear();
            LastNode = null;
            
            foreach(SimpleNode node in AllNodes) {
                node.Revert();
                node.CanSelect = true;
            }
            foreach(Path path in AllPaths) {
                path.Revert();
                path.ComputeIsOutside();
            }


            foreach(Direction direction in AllDirections) direction.Revert();
            foreach(DirectionMessage directionMessage in AllMessages) directionMessage.Revert();

            CurrentDirection = GetBlankDirection();
            CurrentMessage = CurrentDirection.Message;

        }

        public Path GetPath(int id) {
            return PathDictionary[id];
        }

        public DirectionMessage GetMessage(int id) {
            return DirectionMessageDictionary[id];
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
                foreach(SimpleNode node in Nodes) {
                    node.CanMove = true;
                    node.CanSelect = false;
                }
                State = BehaviorState.MovingNodes;
            } else RestoreToDefault();
        }

        internal void SaveNodes() {
            foreach(SimpleNode node in Nodes) {
                if(node.HasChanges()) {
                    int? locationId;
                    if(node.Location == null) locationId = null;
                    else locationId = node.Location.Id;
                    DataCollector.Instance.UpdateNode(node.Id, node.Center.Latitude, node.Center.Longitude, node.Center.Altitude, node.IsOutside, locationId);
                }
            }
            RestoreToDefault();
        }

        internal void SaveDirectionMessage(int id) {
            if(!DirectionMessageDictionary.ContainsKey(id)) return;
            DirectionMessage msg = DirectionMessageDictionary[id];
            DataCollector.Instance.UpdateDirectionMessage(msg.Id, msg.Offset, msg.Message, msg.ReverseMessage, msg.Action, msg.ReverseAction);
            RestoreToDefault();
        }
    }
}
