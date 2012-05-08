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
using Rhit.Applications.Models;

namespace Rhit.Applications.ViewModels.Models {
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
}
