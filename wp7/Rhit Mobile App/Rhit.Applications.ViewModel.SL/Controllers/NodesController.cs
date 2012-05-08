using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Models.Services;
using Rhit.Applications.ViewModels.Models;

namespace Rhit.Applications.ViewModels.Controllers {
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

            AllNodes = new List<Node>();
            SelectedNodes = new List<Node>();
            Nodes = new ObservableCollection<Node>();

            AllPaths = new List<Path>();
            SelectedPaths = new List<Path>();
            Paths = new ObservableCollection<Path>();

            AllDirections = new List<Direction>();

            AllMessages = new ObservableCollection<DirectionMessage>();
            Messages = new ObservableCollection<DirectionMessage>();
        }

        private void InitializeDictionaries() {
            NodeDictionary = new Dictionary<int, Node>();
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

            foreach(Node node in AllNodes)
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
            //Direction direction = new Direction(e.Direction);

            //DirectionDictionary[direction.Id] = direction;
            //AllDirections.Add(direction);

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
            Node node = NodeDictionary[e.Id];
            if(node.CurrentState.HasFlag(Node.State.Selected)) {
                RestoreToDefault();
            }
            NodeDictionary.Remove(node.Id);
            AllNodes.Remove(node);
            Nodes.Remove(node);
            foreach(Node _node in node.GetAdjacentNodes())
                _node.RemoveAdjacentNode(node);
            RestoreToDefault();
        }

        private void Instance_NodeUpdated(object sender, NodeEventArgs e) {
            if(!NodeDictionary.ContainsKey(e.Node.Id)) return;
            Node node = NodeDictionary[e.Node.Id];
            node.UpdateModel(e.Node);

            if(node.IsOutside && !Nodes.Contains(node)) Nodes.Add(node);
            if(!node.IsOutside && Nodes.Contains(node)) Nodes.Remove(node);
        }

        private void Instance_NodeCreated(object sender, NodeEventArgs e) {
            Node node = new Node(e.Node);
            node.Selected += new EventHandler(Node_Selected);
            node.Unselected += new EventHandler(Node_Unselected);
            NodeDictionary[node.Id] = node;
            AllNodes.Add(node);
            if(node.IsOutside) Nodes.Add(node);
            RestoreToDefault();
        }
        #endregion

        #region Dictionaries
        private Dictionary<int, Node> NodeDictionary { get; set; }

        private Dictionary<int, Path> PathDictionary { get; set; }

        private Dictionary<int, Direction> DirectionDictionary { get; set; }

        private Dictionary<int, DirectionMessage> DirectionMessageDictionary { get; set; }
        #endregion

        #region Collections
        public List<string> AllActions { get; set; }

        private List<Node> AllNodes { get; set; }
        
        private List<Node> SelectedNodes { get; set; }

        public ObservableCollection<Node> Nodes { get; protected set; }

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
                Node node = new Node(model);
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



        internal bool DeletePath() {
            if(SelectedNodes.Count < 2) return false;
            Path path = SelectedNodes[0].GetPath(SelectedNodes[1]);
            if(path == null) return false;
            DataCollector.Instance.DeletePath(path.Id);
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

        internal void SavePath(int id) {
            if(State != BehaviorState.Default) return;
            if(!PathDictionary.ContainsKey(id)) return;
            Path path = PathDictionary[id];
            DataCollector.Instance.UpdatePath(id, path.Node1.Id, path.Node2.Id, path.IsElevator, path.StairCount, path.Partition);
            RestoreToDefault();
        }

        internal bool CreatePath() {
            if(SelectedNodes.Count < 2) return false;
            DataCollector.Instance.CreatePath(SelectedNodes[0].Id, SelectedNodes[1].Id, false, 0, 100);
            RestoreToDefault();
            return true;
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




        internal bool DeleteNode() {
            if(SelectedNodes.Count < 1) return false;
            Node node = SelectedNodes[0];
            DataCollector.Instance.DeleteNode(node.Id);
            RestoreToDefault();
            return true;
        }

        private void Node_Moved(object sender, EventArgs e) {
            Node node = sender as Node;
            if(State == BehaviorState.MovingNodes) {
                if(!SelectedNodes.Contains(node)) SelectedNodes.Add(node);
                if(!node.CurrentState.HasFlag(Node.State.Selected))
                    node.CurrentState |= Node.State.Selected;
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
        public Node LastNode {
            get { return (Node) GetValue(LastNodeProperty); }
            set { SetValue(LastNodeProperty, value); }
        }

        public static readonly DependencyProperty LastNodeProperty =
           DependencyProperty.Register("LastNode", typeof(Node), typeof(NodesController), new PropertyMetadata(null));
        #endregion

        internal Direction GetBlankDirection() {
            Direction direction = new Direction(BlankMessage);
            CurrentDirection = direction;
            return direction;
        }



        

        private void Node_Unselected(object sender, EventArgs e) {
            Node node = sender as Node;

        }

        private void ProccessSelectedNode(Node node) {
            ProccessSelectedNode(node, false);
        }

        private void ProccessSelectedNode(Node node, bool multipleEndNodes) {
            SelectedNodes.Add(node);
            node.CurrentState = Node.State.Selected | Node.State.EndNode;

            if(LastNode != null) {
                Path path = LastNode.GetPath(node);
                if(path != null) {
                    SelectedPaths.Add(path);
                    path.IsSelected = true;
                }
                if(!multipleEndNodes && LastNode.CurrentState.HasFlag(Node.State.EndNode))
                    LastNode.CurrentState ^= Node.State.EndNode;
            }
            LastNode = node;
            CurrentDirection = DirectionSearch();
            CurrentMessage = CurrentDirection.Message;
        }

        private void CalculateSelectableNodes() {
            foreach(Node other in AllNodes) {
                if(other.CurrentState.HasFlag(Node.State.CanSelect))
                    other.CurrentState ^= Node.State.CanSelect;
            }
            foreach(Node other in LastNode.GetAdjacentNodes()) {
                if(!other.CurrentState.HasFlag(Node.State.CanSelect))
                    other.CurrentState ^= Node.State.CanSelect;
            }

        }

        private void Node_Selected(object sender, EventArgs e) {
            Node node = sender as Node;
            switch(State) {
                case BehaviorState.MovingNodes:
                    if(!SelectedNodes.Contains(node)) SelectedNodes.Add(node);
                    if(!node.CurrentState.HasFlag(Node.State.Selected))
                        node.CurrentState ^= Node.State.Selected;
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
            
            foreach(Node node in AllNodes) {
                node.Revert();
                if(!node.CurrentState.HasFlag(Node.State.CanSelect))
                    node.CurrentState ^= Node.State.CanSelect;
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
                foreach(Node node in Nodes) {
                    node.CanMove = true;
                    if(node.CurrentState.HasFlag(Node.State.CanSelect))
                        node.CurrentState ^= Node.State.CanSelect;
                }
                State = BehaviorState.MovingNodes;
            } else RestoreToDefault();
        }

        internal void SaveNodes() {
            foreach(Node node in Nodes) {
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
