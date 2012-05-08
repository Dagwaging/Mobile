using System;
using System.Windows;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models;

namespace Rhit.Applications.ViewModels.Models {
    public class Path : DependencyObject {
        //Note: Always assumes nodes are constant
        //Note: IsOutside is only calculated once (it cant be calculate again since it makes actual inside nodes, outside nodes)
        public Path(Path_DC model, Node node1, Node node2) {
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
        public Node Node1 {
            get { return (Node) GetValue(Node1Property); }
            set { SetValue(Node1Property, value); }
        }

        public static readonly DependencyProperty Node1Property =
           DependencyProperty.Register("Node1", typeof(Node), typeof(Path), new PropertyMetadata(null));
        #endregion

        #region Node2
        public Node Node2 {
            get { return (Node) GetValue(Node2Property); }
            set { SetValue(Node2Property, value); }
        }

        public static readonly DependencyProperty Node2Property =
           DependencyProperty.Register("Node2", typeof(Node), typeof(Path), new PropertyMetadata(null));
        #endregion

        public bool Contains(Node node) {
            if(node == Node1 || node == Node2) return true;
            return false;
        }
    }
}
