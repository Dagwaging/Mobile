using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels.Models {
    public class Node : DependencyObject {
        [Flags]
        public enum State {
            Default = 1 << 0,
            CanSelect = 1 << 1,
            Selected = 1 << 2,
            EndNode = 1 << 3,
        }

        public Node(Node_DC model) {
            PathBindingDictionary = new Dictionary<Node, Path>();
            InitializeCommands();

            Model = model;
            Id = Model.Id;
            Revert();
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
            CurrentState = State.Default | State.CanSelect;
            CanMove = false;
            Center = new Location(Model.Latitude, Model.Longitude, Model.Altitude);
            Location = LocationsController.Instance.GetLocation(Model.Location);
            IsOutside = Model.Outside;
            OnMove();
        }

        internal bool UpdateModel(Node_DC model) {
            if(model.Id != Model.Id) return false;

            Model = model;
            Revert();

            return true;
        }

        private Dictionary<Node, Path> PathBindingDictionary { get; set; }

        public void AddPathBinding(Path path) {
            Node other;
            if(path.Node1 == this) other = path.Node2;
            else if(path.Node2 == this) other = path.Node1;
            else return;

            PathBindingDictionary[other] = path;
        }

        public int Id { get; private set; }

        private void InitializeCommands() {
            ClickCommand = new RelayCommand(p => OnClick());
        }

        #region CurrentState
        public State CurrentState {
            get { return (State) GetValue(CurrentStateProperty); }
            set { SetValue(CurrentStateProperty, value); }
        }

        public static readonly DependencyProperty CurrentStateProperty =
           DependencyProperty.Register("CurrentState", typeof(State), typeof(Node), new PropertyMetadata(State.Default | State.CanSelect));
        #endregion

        #region Click Command/Method
        public ICommand ClickCommand { get; private set; }

        private void OnClick() {
            if(CurrentState.HasFlag(State.CanSelect)) {
                //The node is currently not selected and the user can select it
                CurrentState |= State.Selected;
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
           DependencyProperty.Register("Center", typeof(Location), typeof(Node), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnCenterPropertyChanged)));

        private static void OnCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Node node = d as Node;
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
           DependencyProperty.Register("Location", typeof(LocationData), typeof(Node), new PropertyMetadata(null));
        #endregion


        #region IsOutside
        public bool IsOutside {
            get { return (bool) GetValue(IsOutsideProperty); }
            set { SetValue(IsOutsideProperty, value); }
        }

        public static readonly DependencyProperty IsOutsideProperty =
           DependencyProperty.Register("IsOutside", typeof(bool), typeof(Node), new PropertyMetadata(true));
        #endregion

        #region CanMove
        public bool CanMove {
            get { return (bool) GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        public static readonly DependencyProperty CanMoveProperty =
           DependencyProperty.Register("CanMove", typeof(bool), typeof(Node), new PropertyMetadata(false));
        #endregion

        internal ICollection<Node> GetAdjacentNodes() {
            return PathBindingDictionary.Keys;
        }

        internal Path GetPath(Node node) {
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

        internal void RemoveAdjacentNode(Node node) {
            if(PathBindingDictionary.ContainsKey(node))
                PathBindingDictionary.Remove(node);
        }
    }

}
