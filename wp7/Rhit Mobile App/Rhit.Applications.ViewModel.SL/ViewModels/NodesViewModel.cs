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
         private enum BehaviorState { Default, AddingNode, MovingNodes, };

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
                case BehaviorState.AddingNode:
                    CreateNode();
                    break;
                case BehaviorState.MovingNodes:
                    NodesController.Instance.SaveNodes();
                    Cancel();
                    break;
                case BehaviorState.Default:
                default:
                    Cancel();
                    break;
            }
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
            NodesController.Instance.RestoreToDefault();
            State = BehaviorState.Default;
        }

        #region Add Node Command/Method
        public ICommand AddNodeCommand { get; private set; }

        private void AddNode() {
            NodesController.Instance.RestoreToDefault();
            LocationsProvider.CreateNewLocations(1);
            ShowSave = true;
            State = BehaviorState.AddingNode;
        }
        #endregion

        #region Move Nodes Command/Method
        public ICommand MoveNodesCommand { get; private set; }

        private void MoveNodes() {
            NodesController.Instance.RestoreToDefault();
            ShowSave = true;
            State = BehaviorState.MovingNodes;
            NodesController.Instance.AllowMovement();
        }
        #endregion

        #region Delete Node Command/Method
        public ICommand DeleteNodeCommand { get; private set; }

        private void DeleteNode() {
            ShowSave = false;
            NodesController.Instance.DeleteNextNode();
        }
        #endregion

        #region Add Path Command/Method
        public ICommand AddPathCommand { get; private set; }

        private void AddPath() {
            ShowSave = false;
            NodesController.Instance.CreateNewPath();
        }
        #endregion

        #region Delete Path Command/Method
        public ICommand DeletePathCommand { get; private set; }

        private void DeletePath() {
            ShowSave = false;
            NodesController.Instance.DeleteNextPath();
        }
        #endregion

        public NodesController Paths { get; set; }

        private BehaviorState State { get; set; }


    }
}
