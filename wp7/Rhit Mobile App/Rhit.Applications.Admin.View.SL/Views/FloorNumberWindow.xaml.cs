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

namespace Rhit.Applications.Views {
    public partial class FloorNumberWindow : ChildWindow {
        public FloorNumberWindow() {
            InitializeComponent();
            DataContext = this;
            FloorNumberBox.Focus();
        }

        private string floorNumber;
        public string FloorNumber {
            get { return floorNumber; }
            set {
                int intValue;
                if(value == string.Empty) {
                    floorNumber = value;
                    return;
                }
                if(int.TryParse(value, out intValue)) floorNumber = value;
                else throw new Exception("Only enter integers");
                
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        public int GetFloorNumber() {
            int intValue = 0;
            int.TryParse(floorNumber, out intValue);
            return intValue;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) this.DialogResult = true;
        }
    }
}
