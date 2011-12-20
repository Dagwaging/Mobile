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

namespace Rhit.Applications.View.Utility {
    public class TaskStep : DependencyObject {
        public TaskStep() { }

        public TaskStep(string label, string instructions) {
            Label = label;
            Instructions = instructions;
        }

        #region Dependency Properties
        #region Label
        public string Label {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(TaskStep), new PropertyMetadata(""));
        #endregion

        #region Instructions
        public string Instructions {
            get { return (string) GetValue(InstructionsProperty); }
            set { SetValue(InstructionsProperty, value); }
        }

        public static readonly DependencyProperty InstructionsProperty =
           DependencyProperty.Register("Instructions", typeof(string), typeof(TaskStep), new PropertyMetadata(""));
        #endregion

        #region Number
        public int Number {
            get { return (int) GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty NumberProperty =
           DependencyProperty.Register("Number", typeof(int), typeof(TaskStep), new PropertyMetadata(0));
        #endregion
        #endregion
    }
}
