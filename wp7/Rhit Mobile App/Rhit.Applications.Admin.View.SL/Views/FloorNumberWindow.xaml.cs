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
    public partial class FloorNumberWindow : ChildWindow {
        public FloorNumberWindow() {
            InitializeComponent();
            DataContext = this;
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
                if(!int.TryParse(value, out intValue))
                    throw new Exception("Only enter numbers");
                else
                    floorNumber = value;
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
    }
}
