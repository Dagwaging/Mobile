using System;
using System.Windows;
using System.Windows.Controls;

namespace Rhit.Applications.Views {
    public partial class ErrorWindow : ChildWindow {
        public ErrorWindow(Exception e) {
            InitializeComponent();
            if(e == null) return;
            
            ErrorTextBox.Text = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
        }

        public ErrorWindow(Uri uri) {
            InitializeComponent();
            if(uri == null) return;
            
            ErrorTextBox.Text = "Page not found: \"" + uri.ToString() + "\"";
        }

        public ErrorWindow(string message, string details) {
            InitializeComponent();
            
            ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}