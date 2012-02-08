using System.Windows.Controls;
using Rhit.Applications.ViewModel.Models;
using System.Windows;
using System;

namespace Rhit.Applications.ServerTree.View.SL {
    public partial class MainPage : UserControl {
        public MainPage() {
            InitializeComponent();

            ViewModel = new MainViewModel();
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel { get; set; }

        private string PreviousId { get; set; }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            ViewModel.ChangeRequest((sender as TreeView).SelectedItem);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox box = (sender as TextBox);
            try {
                Int32.Parse(box.Text);
                return;
            } catch(Exception) {
                box.Text = ViewModel.Id.ToString();
            }

        }
    }
}
