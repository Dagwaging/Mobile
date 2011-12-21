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

namespace Rhit.Applications.View.Utility {
    public class Task : DependencyObject {
        public Task() {
            Steps = new ObservableCollection<TaskStep>();
            ActivateCommand = new RelayCommand(p => Activate());
        }

        public event EventHandler Activated;

        private void OnAvtivate(EventArgs e) {
            if(Activated != null) Activated(this, e);
        }

        public ICommand ActivateCommand { get; set; }

        private void Activate() {
            if(Steps.Count > 0)
                CurrentStep = Steps[0];
            OnAvtivate(new EventArgs());
            if(StartCommand != null) StartCommand.Execute(null);
        }

        #region Dependency Properties
        #region StartCommand
        public ICommand StartCommand {
            get { return (ICommand) GetValue(StartCommandProperty); }
            set { SetValue(StartCommandProperty, value); }
        }

        public static readonly DependencyProperty StartCommandProperty =
           DependencyProperty.Register("StartCommand", typeof(ICommand), typeof(Task), new PropertyMetadata(null));
        #endregion

        #region CompletedCommand
        public ICommand CompletedCommand {
            get { return (ICommand) GetValue(CompletedCommandProperty); }
            set { SetValue(CompletedCommandProperty, value); }
        }

        public static readonly DependencyProperty CompletedCommandProperty =
           DependencyProperty.Register("CompletedCommand", typeof(ICommand), typeof(Task), new PropertyMetadata(null));
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
