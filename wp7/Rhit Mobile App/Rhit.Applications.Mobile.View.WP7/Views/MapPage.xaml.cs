using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Input;
using Rhit.Applications.Mvvm.Commands;

namespace Rhit.Applications.View.Views {
    /// \ingroup pages
    public partial class MapPage : PhoneApplicationPage {

        public MapPage() {
            DataContext = this;
            InitializeComponent();

            //TODO: Try not to have to do this
            ViewModel.SetMode(MyMap);
            MyMap.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Map_Tap);

            DirectionsCommand = new RelayCommand(p => GotoDirections());
            CampusServicesCommand = new RelayCommand(p => GotoCampusServices());
            QuickListCommand = new RelayCommand(p => GotoQuickList());
            SearchCommand = new RelayCommand(p => GotoSearch());
            SettingsCommand = new RelayCommand(p => GotoSettings());
        }

        #region Directions Command
        public ICommand DirectionsCommand { get; private set; }

        private void GotoDirections() {
            NavigationService.Navigate(new Uri("/Views/DirectionsPage.xaml", UriKind.Relative));
        }
        #endregion

        #region Campus Services Command
        public ICommand CampusServicesCommand { get; private set; }

        private void GotoCampusServices() {
            NavigationService.Navigate(new Uri("/Views/CampusServicesPage.xaml", UriKind.Relative));
        }
        #endregion

        #region QuickList Command
        public ICommand QuickListCommand { get; private set; }

        private void GotoQuickList() {
            NavigationService.Navigate(new Uri("/Views/QuickListPage.xaml", UriKind.Relative));
        }
        #endregion

        #region Search Command
        public ICommand SearchCommand { get; private set; }

        private void GotoSearch() {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml", UriKind.Relative));
        }
        #endregion

        #region Settings Command
        public ICommand SettingsCommand { get; private set; }

        private void GotoSettings() {
            NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
        }
        #endregion

        private Point LastEventCoordinate { get; set; }

        private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            if(LastEventCoordinate == e.GetPosition(MyMap)) return;
            ViewModel.Locations.UnSelect();
        }

        private void MapPolygon_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as MapPolygon).Tag);
        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            NavigationService.Navigate(new Uri("/Views/InfoPage.xaml", UriKind.Relative));
        }
    }
}