using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.ViewModel.Controllers;
using System.Collections.Generic;
using Microsoft.Phone.Controls.Maps;
using System.ComponentModel;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Utilities;

namespace Rhit.Applications.ViewModel.Models {
    public class DirectionsViewModel : BaseMapViewModel {
        protected override void Initialize() {
            base.Initialize();
            if(DesignerProperties.IsInDesignTool) return;

            ZoomLevel = 19.5;

            Directions = PathsController.Instance;
            Directions.NodesUpdated += new EventHandler(NodesUpdated);
            NextCommand = new RelayCommand(p => Next());
            PreviousCommand = new RelayCommand(p => Previous());
        }

        public PathsController Directions { get; private set; }

        private void NodesUpdated(object sender, EventArgs e) {
            CurrentNode = Directions.Start;
        }

        private void ChangeNode(PathNode oldNode, PathNode newNode) {
            if(oldNode != null) {
                oldNode.IsSelected = false;
            }
            if(newNode != null) {
                newNode.IsSelected = true;
                if(newNode.Next != null) HasNext = true;
                else HasNext = false;
                if(newNode.Previous != null) HasPrevious = true;
                else HasPrevious = false;
                Center = newNode.Center;
            }
        }

        public void SetLocation(int id, bool isTour) {
            if(isTour) {
                Directions.GetTestTour();
                
            } else {
                if(id < 0) Directions.GetTestDirections();
                else Directions.GetDirections(id);
            }
        }

        #region Next Command
        public ICommand NextCommand { get; private set; }

        private void Next() {
            CurrentNode = CurrentNode.Next;
        }
        #endregion

        #region Previous Command
        public ICommand PreviousCommand { get; private set; }

        private void Previous() {
            CurrentNode = CurrentNode.Previous;
        }
        #endregion

        #region HasNext
        public bool HasNext {
            get { return (bool) GetValue(HasNextProperty); }
            set { SetValue(HasNextProperty, value); }
        }

        public static readonly DependencyProperty HasNextProperty =
           DependencyProperty.Register("HasNext", typeof(bool), typeof(DirectionsViewModel), new PropertyMetadata(false));
        #endregion

        #region HasPrevious
        public bool HasPrevious {
            get { return (bool) GetValue(HasPreviousProperty); }
            set { SetValue(HasPreviousProperty, value); }
        }

        public static readonly DependencyProperty HasPreviousProperty =
           DependencyProperty.Register("HasPrevious", typeof(bool), typeof(DirectionsViewModel), new PropertyMetadata(false));
        #endregion

        #region CurrentNode
        public PathNode CurrentNode {
            get { return (PathNode) GetValue(CurrentNodeProperty); }
            set { SetValue(CurrentNodeProperty, value); }
        }

        public static readonly DependencyProperty CurrentNodeProperty =
           DependencyProperty.Register("CurrentNode", typeof(PathNode), typeof(DirectionsViewModel),
           new PropertyMetadata(null, new PropertyChangedCallback(OnSurrentNodeChanged)));

        private static void OnSurrentNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DirectionsViewModel instance = d as DirectionsViewModel;
            instance.ChangeNode(e.OldValue as PathNode, e.NewValue as PathNode);
            
        }
        #endregion
    }
}
