using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rhit.Applications.Views {
    public partial class FloorNumberWindow : ChildWindow {
        public FloorNumberWindow() {
            InitializeComponent();

            DataContext = this;
        }

        #region public string FloorNumber
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
        #endregion

        public int GetFloorNumber() {
            int intValue = 0;
            int.TryParse(floorNumber, out intValue);
            return intValue;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
        
        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) DialogResult = true;
        }
    }
}
