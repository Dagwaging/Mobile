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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Rhit.Applications.View.Utility {
    public class TaskContainer : DependencyObject {
        public TaskContainer() {
            Tasks = new ObservableCollection<Task>();
            Tasks.CollectionChanged += new NotifyCollectionChangedEventHandler(Tasks_CollectionChanged);
        }

        private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if(e.Action != NotifyCollectionChangedAction.Add) return;
            foreach(Task task in e.NewItems)
                task.Activated += new EventHandler(Task_Activated);
        }

        private void Task_Activated(object sender, EventArgs e) {
            CurrentTask = (sender as Task);
        }

        public ObservableCollection<Task> Tasks { get; private set; }

        #region Dependency Properties
        #region CurrentTask
        public Task CurrentTask {
            get { return (Task) GetValue(CurrentTaskProperty); }
            set { SetValue(CurrentTaskProperty, value); }
        }

        public static readonly DependencyProperty CurrentTaskProperty =
           DependencyProperty.Register("CurrentTask", typeof(Task), typeof(TaskContainer), new PropertyMetadata(null));
        #endregion
        #endregion

        public void AddTasks(IEnumerable<Task> tasks) {
            foreach(Task task in tasks)
                Tasks.Add(task);
        }
    }
}
