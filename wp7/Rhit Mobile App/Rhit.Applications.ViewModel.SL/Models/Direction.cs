using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;
using Rhit.Applications.ViewModels.Controllers;

namespace Rhit.Applications.ViewModels.Models {
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
}
