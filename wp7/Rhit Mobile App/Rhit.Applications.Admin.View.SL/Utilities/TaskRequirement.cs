using System.ComponentModel;
using System.Windows;

namespace Rhit.Applications.Views.Utilities {
    public class TaskRequirement : DependencyObject, INotifyPropertyChanged {
        public TaskRequirement() { }

        #region Value
        public bool Value {
            get { return (bool) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register("Value", typeof(bool), typeof(TaskRequirement), new PropertyMetadata(true, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as TaskRequirement).OnChanged(new PropertyChangedEventArgs("Value"));
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnChanged(PropertyChangedEventArgs e) {
            if(PropertyChanged != null) PropertyChanged(this, e);
        }
    }
}
