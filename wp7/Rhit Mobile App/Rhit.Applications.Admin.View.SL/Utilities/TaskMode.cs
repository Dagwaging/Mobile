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

namespace Rhit.Applications.Views.Utilities {
    public class TaskMode : DependencyObject {
        public TaskMode() {
            CurrentTaskMode = new TaskContainer();
        }

        #region CurrentTaskMode
        public TaskContainer CurrentTaskMode {
            get { return (TaskContainer) GetValue(CurrentTaskModeProperty); }
            set { SetValue(CurrentTaskModeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTaskModeProperty =
           DependencyProperty.Register("CurrentTaskMode", typeof(TaskContainer), typeof(TaskMode), new PropertyMetadata(null));
        #endregion
    }
}
