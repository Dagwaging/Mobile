using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rhit.Applications.View.Views {
    public partial class LocationIdWindow : ChildWindow {
        public LocationIdWindow() {
            InitializeComponent();
            DataContext = this;
            IdNumberBox.Focus();
        }

        private string idNumber;
        public string IdNumber {
            get { return idNumber; }
            set {
                int intValue;
                if(value == string.Empty) {
                    idNumber = value;
                    return;
                }
                if(int.TryParse(value, out intValue)) idNumber = value;
                else throw new Exception("Only enter integers");

            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        public int GetIdNumber() {
            int intValue = 0;
            int.TryParse(idNumber, out intValue);
            return intValue;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) this.DialogResult = true;
        }
    }
}
