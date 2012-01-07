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
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    public partial class LocationIdWindow : ChildWindow {
        public LocationIdWindow() {
            InitializeComponent();
            DataContext = this;
            IdNumber = "0";
            this.UpdateLayout();
            this.MyGrid.UpdateLayout();
            this.Tree.UpdateLayout();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            ViewModel.SetTempLocation((sender as TreeView).SelectedValue);
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

        public string NewName { get; set; }

        public int GetIdNumber() {
            int intValue = 0;
            int.TryParse(IdNumber, out intValue);
            return intValue;
        }

        public int GetParentId() {
            return ViewModel.GetTempId();
        }
    }
}
