using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    /// \ingroup pages
    public partial class SettingsPage : PhoneApplicationPage {

        public SettingsPage() {
            InitializeComponent();
            ViewModel = new MapViewModel();
            DataContext = ViewModel;
        }

        public MapViewModel ViewModel { get; set; }
    }
}
