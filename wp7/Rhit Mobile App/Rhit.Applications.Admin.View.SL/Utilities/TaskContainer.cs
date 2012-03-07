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
using Rhit.Applications.Mvvm.Commands;

namespace Rhit.Applications.Views.Utilities {
    public class TaskContainer : DependencyObject {
        public TaskContainer() {
            TaskStack = new Stack<Task>();
            Tasks = new ObservableCollection<Task>();
            Tasks.CollectionChanged += new NotifyCollectionChangedEventHandler(Tasks_CollectionChanged);
            CompleteCommand = new RelayCommand(p => Complete());
            CancelCommand = new RelayCommand(p => Cancel());
        }

        public void Complete() {
            if(TaskStack.Count <= 0) CurrentTask = null;
            else CurrentTask = TaskStack.Pop();
            foreach(Task task in Tasks) task.CalculateVisibility();
            if(CompletedCommand != null) CompletedCommand.Execute(null);
        }

        public void Cancel() {
            if(TaskStack.Count <= 0) CurrentTask = null;
            else CurrentTask = TaskStack.Pop();
            foreach(Task task in Tasks) task.CalculateVisibility();
            if(CanceledCommand != null) CanceledCommand.Execute(null);
        }

        public ICommand CompleteCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private Stack<Task> TaskStack { get; set; }

        private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if(e.Action != NotifyCollectionChangedAction.Add) return;
            foreach(Task task in e.NewItems) {
                task.Activated += new EventHandler(Task_Activated);
                task.Parent = this;
            }
        }

        private void Task_Activated(object sender, EventArgs e) {
            TaskStack.Push(CurrentTask);
            CurrentTask = (sender as Task);
            foreach(Task task in Tasks) task.CalculateVisibility();
        }

        public ObservableCollection<Task> Tasks { get; set; }

        #region Dependency Properties
        #region CompleteCommand
        public ICommand CompletedCommand {
            get { return (ICommand) GetValue(CompletedCommandProperty); }
            set { SetValue(CompletedCommandProperty, value); }
        }

        public static readonly DependencyProperty CompletedCommandProperty =
           DependencyProperty.Register("CompletedCommand", typeof(ICommand), typeof(TaskContainer), new PropertyMetadata(null));
        #endregion

        #region CancelCommand
        public ICommand CanceledCommand {
            get { return (ICommand) GetValue(CanceledCommandProperty); }
            set { SetValue(CanceledCommandProperty, value); }
        }

        public static readonly DependencyProperty CanceledCommandProperty =
           DependencyProperty.Register("CanceledCommand", typeof(ICommand), typeof(TaskContainer), new PropertyMetadata(null));
        #endregion

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
