using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using RhitMobile.Events;
using RhitMobile.ObjectModel;
using RhitMobile.Services;

namespace RhitMobile {
    /// \ingroup pages
    public partial class QuiuckListPage : PhoneApplicationPage {
        public QuiuckListPage() {
            InitializeComponent();

            DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(OnUpdateAvailable);

            List<RhitLocation> dataSource = DataCollector.Instance.GetQuikList(Dispatcher);
            if (dataSource == null) {
                //TODO: Show Loading Screen
                //TODO: Show a waiting text + symbol
                //TODO: Add progress?
                return;
            }

            listBox.ItemsSource = dataSource;
        }

        private void OnUpdateAvailable(object sender, ServiceEventArgs e) {
            List<RhitLocation> locations = DataCollector.Instance.GetQuikList(Dispatcher);
            if(locations != null) listBox.ItemsSource = locations;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RhitLocation selected = (RhitLocation) listBox.SelectedItem;
            RhitMap.Instance.Select(selected);
            listBox.SelectedItem = null;
            NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }
    }
}