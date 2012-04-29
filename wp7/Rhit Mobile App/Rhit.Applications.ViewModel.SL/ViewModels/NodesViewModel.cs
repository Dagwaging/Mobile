using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels {
    public class NodesViewModel : TaskedViewModel {
        private enum BehaviorState { Default, AddingNode, MovingNodes, DeletingNode, CreatingPath, DeletingPath, };

         protected override void Initialize() {
             base.Initialize();
             if(DesignerProperties.IsInDesignTool) return;

             State = BehaviorState.Default;
             Paths = NodesController.Instance;
         }

        protected override void InitializeCommands() {
            base.InitializeCommands();

            
            ClearSelectedCommand = new RelayCommand(p => ClearSelected());

            SaveDirectionCommand = new RelayCommand(p => SaveDirection());
            DeleteDirectionCommand = new RelayCommand(p => DeleteDirection());

            AddNodeCommand = new RelayCommand(p => AddNode());
            MoveNodesCommand = new RelayCommand(p => MoveNodes());
            DeleteNodeCommand = new RelayCommand(p => DeleteNode());

            PathClickCommand = new RelayCommand(p => HandlePathClick(p));
            PathSaveCommand = new RelayCommand(p => HandlePathSave(p));
            AddPathCommand = new RelayCommand(p => AddPath());
            DeletePathCommand = new RelayCommand(p => DeletePath());
            
            AddMessageCommand = new RelayCommand(p => AddMessage());
            SaveMessageCommand = new RelayCommand(p => SaveMessage(p));
            DeleteMessageCommand = new RelayCommand(p => DeleteMessage(p));
        }

        protected override void Save() {
            switch(State) {
                case BehaviorState.DeletingNode:
                    Paths.DeleteNode();
                    break;
                case BehaviorState.CreatingPath:
                    Paths.CreatePath();
                    break;
                case BehaviorState.DeletingPath:
                    Paths.DeletePath();
                    break;
                case BehaviorState.AddingNode:
                    CreateNode();
                    break;
                case BehaviorState.MovingNodes:
                    Paths.SaveNodes();
                    Cancel();
                    break;
                case BehaviorState.Default:
                default:
                    break;
            }
            Cancel();
        }

        private void CreateNode() {
            IList<Location> locations = LocationsProvider.GetLocations();
            Location newLocation = null;
            if(locations.Count > 0) newLocation = locations[0];
            if(newLocation != null) {
                DataCollector.Instance.CreateNode(newLocation.Latitude, newLocation.Longitude, newLocation.Altitude, true, null);
            }
            Cancel();
        }

        protected override void Cancel() {
            Paths.RestoreToDefault();
            State = BehaviorState.Default;
        }

        #region ClearSelected Command/Method
        public ICommand ClearSelectedCommand { get; private set; }

        private void ClearSelected() {
            Paths.RestoreToDefault();
        }
        #endregion


        #region SaveDirection Command/Method
        public ICommand SaveDirectionCommand { get; private set; }

        private void SaveDirection() {
            Paths.SaveDirection();
        }
        #endregion

        #region DeleteDirection Command/Method
        public ICommand DeleteDirectionCommand { get; private set; }

        private void DeleteDirection() {
            Paths.DeleteDirection();
        }
        #endregion


        #region Add Node Command/Method
        public ICommand AddNodeCommand { get; private set; }

        private void AddNode() {
            State = BehaviorState.AddingNode;
            Paths.RestoreToDefault();
            LocationsProvider.CreateNewLocations(1);
            ShowSave = true;
        }
        #endregion

        #region Move Nodes Command/Method
        public ICommand MoveNodesCommand { get; private set; }

        private void MoveNodes() {
            State = BehaviorState.MovingNodes;
            Paths.RestoreToDefault();
            ShowSave = true;
            Paths.AllowMovement();
        }
        #endregion

        #region Delete Node Command/Method
        public ICommand DeleteNodeCommand { get; private set; }

        private void DeleteNode() {
            State = BehaviorState.DeletingNode;
            ShowSave = false;
            Paths.DeleteNextNode();
        }
        #endregion


        #region Add Path Command/Method
        public ICommand AddPathCommand { get; private set; }

        private void AddPath() {
            State = BehaviorState.CreatingPath;
            ShowSave = false;
            Paths.CreateNewPath();
        }
        #endregion

        #region Delete Path Command/Method
        public ICommand DeletePathCommand { get; private set; }

        private void DeletePath() {
            State = BehaviorState.DeletingPath;
            ShowSave = false;
            Paths.DeleteNextPath();
        }
        #endregion

        #region Path Click Command/Method
        public ICommand PathClickCommand { get; private set; }

        private void HandlePathClick(object p) {
            if(!(p is int)) return;
            int id = (int) p;
            Paths.SelectPath(id);
        }
        #endregion

        #region Path Save Command/Method
        public ICommand PathSaveCommand { get; private set; }

        private void HandlePathSave(object p) {
            if(!(p is int)) return;
            int id = (int) p;
            Paths.SavePath(id);
        }
        #endregion


        #region AddMessage Command/Method
        public ICommand AddMessageCommand { get; private set; }

        private void AddMessage() {
            DataCollector.Instance.AddBlankDirectionMessage();
        }
        #endregion

        #region SaveMessage Command/Method
        public ICommand SaveMessageCommand { get; private set; }

        private void SaveMessage(object p) {
            if(!(p is int)) return;
            int id = (int) p;
            Paths.SaveDirectionMessage(id);
        }
        #endregion

        #region DeleteMessage Command/Method
        public ICommand DeleteMessageCommand { get; private set; }

        private void DeleteMessage(object p) {
            if(!(p is int)) return;
            int id = (int) p;
            DataCollector.Instance.DeleteDirectionMessage(id);
        }
        #endregion

        public NodesController Paths { get; set; }

        private BehaviorState State { get; set; }
    }
}
