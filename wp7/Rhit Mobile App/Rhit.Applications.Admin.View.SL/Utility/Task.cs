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

namespace Rhit.Applications.View.Utility {
    public class Task : DependencyObject {
        public Task() {
            Steps = new ObservableCollection<TaskStep>();
        }

        public Task(IEnumerable<TaskStep> collection) {
            Steps = new ObservableCollection<TaskStep>();
            AddSteps(collection);
        }

        //public ICommand Command { get; set; }

        #region Dependency Properties
        #region Command
        public ICommand Command {
            get {
                return (ICommand) GetValue(CommandProperty);
            }
            set {
                SetValue(CommandProperty, value);
            }
        }

        public static readonly DependencyProperty CommandProperty =
           DependencyProperty.Register("Command", typeof(ICommand), typeof(Task), new PropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var a = d;
            var b = e;
            return;
        }
        #endregion

        #region Label
        public string Label {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(Task), new PropertyMetadata(""));
        #endregion

        #region CurrentStep
        public TaskStep CurrentStep {
            get { return (TaskStep) GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }

        public static readonly DependencyProperty CurrentStepProperty =
           DependencyProperty.Register("CurrentStep", typeof(TaskStep), typeof(Task), new PropertyMetadata(null));
        #endregion
        #endregion

        public ObservableCollection<TaskStep> Steps { get; private set; }

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
