using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using RhitMobile.Events;
using RhitMobile.ObjectModel;
using RhitMobile.Services;

namespace RhitMobile {
    public partial class POIPage : PhoneApplicationPage {
        public POIPage() {
            InitializeComponent();

            DataCollector.Instance.UpdateAvailable += new ServerEventHandler(OnUpdateAvailable);

            List<RhitLocation> dataSource = DataCollector.Instance.GetLocations(Dispatcher);
            if (dataSource == null) {
                //TODO: Show Loading Screen
                //TODO: Show a waiting text + symbol
                //TODO: Add progress?
                return;
            }

            listBox.ItemsSource = dataSource;
        }

        private void OnUpdateAvailable(object sender, ServerEventArgs e) {
            listBox.ItemsSource = e.ResponseObject.GetLocations();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RhitLocation selected = (RhitLocation) listBox.SelectedItem;

            ListBoxItem lbi = (ListBoxItem) (listBox.ItemContainerGenerator.ContainerFromItem(selected));
            if(lbi == null) return;
            lbi.Foreground = new SolidColorBrush(Colors.Green);

            RhitMapView.Instance.Select(selected);

            if(NavigationService.CanGoBack) NavigationService.GoBack();
            else NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }
    }
}