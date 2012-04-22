using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.ViewModels.Providers;
using Rhit.Applications.ViewModels.Utilities;

namespace Rhit.Applications.ViewModels {
    public class NodesViewModel : TaskedViewModel {
        private enum BehaviorState { Default, AddingNode, MovingNodes, DeletingNode, CreatingPath, DeletingPath, };

         protected override void Initialize() {
             base.Initialize();

             State = BehaviorState.Default;
             Paths = NodesController.Instance;
             Paths.GetPathData();
         }

        protected override void InitializeCommands() {
            base.InitializeCommands();

            AddNodeCommand = new RelayCommand(p => AddNode());
            DeleteNodeCommand = new RelayCommand(p => DeleteNode());
            AddPathCommand = new RelayCommand(p => AddPath());
            DeletePathCommand = new RelayCommand(p => DeletePath());
            MoveNodesCommand = new RelayCommand(p => MoveNodes());
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

        public NodesController Paths { get; set; }

        private BehaviorState State { get; set; }
    }
}
