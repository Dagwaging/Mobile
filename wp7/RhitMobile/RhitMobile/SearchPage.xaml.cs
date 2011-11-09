using System.Windows.Input;
using Microsoft.Phone.Controls;
using RhitMobile.Services;
using RhitMobile.Events;
using System.Windows.Controls;
using RhitMobile.ObjectModel;
using System.Windows.Media;
using System;

namespace RhitMobile {
    public partial class SearchPage : PhoneApplicationPage {
        public SearchPage() {
            InitializeComponent();
            this.IsTabStop = true;
            DataCollector.Instance.SearchResultsAvailable += new SearchEventHandler(SearchResultsAvailable);
            PivotControl.SelectionChanged += new SelectionChangedEventHandler(PivotControl_SelectionChanged);
        }

        void PivotControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.Focus();
        }

        void SearchResultsAvailable(object sender, SearchEventArgs e) {
            if(e.Places != null && e.Places.Count > 0) {
                if(SearchAllTextBox.Text != "" && AllResults.ItemsSource == null)
                    AllResults.ItemsSource = e.Places;
                if(SearchPlacesTextBox.Text != "" && PlacesResults.ItemsSource == null)
                    PlacesResults.ItemsSource = e.Places;
            }
        }

        private void SearchAll_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                DataCollector.Instance.SearchLocations(Dispatcher, SearchAllTextBox.Text);
                AllResults.ItemsSource = null;
                //DataCollector.Instance.SearchPeople(Dispatcher, SearchPeopleTextBox.Text);
                this.Focus();
            }
        }

        private void SearchPlaces_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                DataCollector.Instance.SearchLocations(Dispatcher, SearchPlacesTextBox.Text);
                PeopleResults.ItemsSource = null;
                this.Focus();
            }
        }
        
        private void SearchPeople_KeyUp(object sender, KeyEventArgs e) {
            //if(e.Key == Key.Enter) {
            //    DataCollector.Instance.SearchPeople(Dispatcher, SearchPeopleTextBox.Text);
            //    this.Focus();
            //}
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListBox listbox = (ListBox) sender;
            RhitLocation selected = (RhitLocation) listbox.SelectedItem;
            if(selected == null) return;
            RhitMap.Instance.Select(selected);
            listbox.SelectedItem = null;
            NavigationService.Navigate(new Uri("/InfoPage.xaml?Id=" + selected.Id.ToString(), UriKind.Relative));
        }
    }
}