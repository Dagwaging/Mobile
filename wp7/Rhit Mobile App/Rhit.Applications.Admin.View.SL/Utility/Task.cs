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
using Rhit.Applications.Mvvm.Commands;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Rhit.Applications.View.Utility {
    public class Task : DependencyObject {
        public Task() {
            Visibility = Visibility.Visible;
            ActivateCommand = new RelayCommand(p => Activate());
            Steps = new ObservableCollection<TaskStep>();
            InternalRequirements = new ObservableCollection<string>();
            Requirements = new ObservableCollection<TaskRequirement>();
            Requirements.CollectionChanged += new NotifyCollectionChangedEventHandler(Requirements_CollectionChanged);
        }

        public TaskContainer Parent { get; set; }

        private void Requirements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if(e.NewItems == null || e.NewItems.Count <= 0) return;
            foreach(TaskRequirement req in e.NewItems)
                if(req != null) req.PropertyChanged += new PropertyChangedEventHandler(Requirements_PropertyChanged);
            CalculateVisibility();
        }

        private void Requirements_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            CalculateVisibility();
        }

        public void CalculateVisibility() {
            foreach(TaskRequirement req in Requirements)
                if(!req.Value) {
                    Visibility = Visibility.Collapsed;
                    return;
                }
            if(Parent != null && Parent.CurrentTask != null) {
                if(InternalRequirements.Contains(Parent.CurrentTask.Label))
                    Visibility = Visibility.Visible;
                else Visibility = Visibility.Collapsed;
                return;
            }
            Visibility = Visibility.Visible;
        }

        #region Task Activation
        public event EventHandler Activated;

        private void OnAvtivate(EventArgs e) {
            if(Activated != null) Activated(this, e);
        }

        public ICommand ActivateCommand { get; set; }

        private void Activate() {
            if(Steps.Count > 0) CurrentStep = Steps[0];
            else CurrentStep = null;
            OnAvtivate(new EventArgs());
            if(StartCommand != null) StartCommand.Execute(null);
        }
        #endregion

        #region Dependency Properties
        #region StartCommand
        public ICommand StartCommand {
            get { return (ICommand) GetValue(StartCommandProperty); }
            set { SetValue(StartCommandProperty, value); }
        }

        public static readonly DependencyProperty StartCommandProperty =
           DependencyProperty.Register("StartCommand", typeof(ICommand), typeof(Task), new PropertyMetadata(null));
        #endregion

        #region Label
        public string Label {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(Task), new PropertyMetadata(""));
        #endregion

        #region ToolTip
        public string ToolTip {
            get { return (string) GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        public static readonly DependencyProperty ToolTipProperty =
           DependencyProperty.Register("ToolTip", typeof(string), typeof(Task), new PropertyMetadata(""));
        #endregion

        #region CurrentStep
        public TaskStep CurrentStep {
            get { return (TaskStep) GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }

        public static readonly DependencyProperty CurrentStepProperty =
           DependencyProperty.Register("CurrentStep", typeof(TaskStep), typeof(Task), new PropertyMetadata(null));
        #endregion

        #region Visibility
        public Visibility Visibility {
            get { return (Visibility) GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly DependencyProperty VisibilityProperty =
           DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Task), new PropertyMetadata(null));
        #endregion
        #endregion

        public ObservableCollection<TaskStep> Steps { get; private set; }

        public ObservableCollection<TaskRequirement> Requirements { get; set; }

        public ObservableCollection<string> InternalRequirements { get; set; }

        public void AddStep(TaskStep step) {
            Steps.Add(step);
        }

        public void AddStep(string label, string instructions) {
            Steps.Add(new TaskStep(label, instructions));
        }

        public void AddSteps(IEnumerable<TaskStep> steps) {
            foreach(TaskStep step in steps)
                AddStep(step);
        }
    }
}
