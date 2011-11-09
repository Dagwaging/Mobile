using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using RhitMobile.Events;
using RhitMobile.ObjectModel;
using RhitMobile.Services;

namespace RhitMobile {
    public partial class InfoPage : PhoneApplicationPage {
        public InfoPage() {
            InitializeComponent();
            DataCollector.Instance.UpdateAvailable += new ServiceEventHandler(OnUpdateAvailable);
        }

        private RhitLocation Location { get; set; }

        private void UpdateValues() {
            List<RhitLocation> children = DataCollector.Instance.GetChildLocations(Dispatcher, Location);
            PivotControl.Title = "Rhit Mobile - " + Location.Label;
            InfoPanel.Children.Clear();
            if(Location.Description == null || Location.Description == "")
                Description.Text = "No Description Loaded.\nTrying to load now...";
            else Description.Text = Location.Description;
            InfoPanel.Children.Add(Description);
            if(Location.AltNames != null && Location.AltNames.Count > 0) {
                TextBlock text = new TextBlock();
                text.Inlines.Add(new Run() {Text = "\n"});
                text.Inlines.Add(new Run() {Text = "Alternate Names:\n", FontWeight = FontWeights.Bold});
                string names = "";
                foreach(string name in Location.AltNames) names += name + ',';
                names = names.Remove(names.Length - 1, 1);
                text.Inlines.Add(new Run() { Text = names });
                InfoPanel.Children.Add(text);
            }
            if(Location.Links != null && Location.Links.Count > 0) {
                TextBlock text = new TextBlock();
                text.Inlines.Add(new Run() { Text = "\n" });
                text.Inlines.Add(new Run() { Text = "Links:", FontWeight = FontWeights.Bold });
                InfoPanel.Children.Add(text);
                foreach(KeyValuePair<String, String> link in Location.Links) {
                    HyperlinkButton button = new HyperlinkButton() {
                        NavigateUri = new Uri(link.Value, UriKind.Absolute),
                        Content = link.Key,
                        TargetName = link.Key,
                    };
                    InfoPanel.Children.Add(button);
                }
            }
            if(children != null) {
                if (Places.Children.Contains(PlacesTextBox) && children.Count == 0)
                    PlacesTextBox.Text = Location.Label + " does not contain any other places.";
                else {
                    Places.Children.Clear();
                    ListBox listbox = new ListBox() {
                        ItemTemplate = ListBoxTemplate,
                        ItemsSource = children,
                    };
                    
                    listbox.SelectionChanged += new SelectionChangedEventHandler(Places_SelectionChanged);
                    Places.Children.Add(listbox);
                }
            }
        }

        private void Places_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RhitLocation location = (RhitLocation) e.AddedItems[0];
            RhitMapView.Instance.Select(location);
            Location = location;
            NavigationService.Navigate(new Uri("/InfoPage.xaml?Id="+location.Id.ToString(), UriKind.Relative));
        }

        private void OnUpdateAvailable(object sender, ServiceEventArgs e) {
            RhitLocation location = DataCollector.Instance.GetLocation(Dispatcher, Location.Id);
            if(location == null) GoBack(); //TODO: Handle this better; Shouldn't give whiplash
            Location = location;
            UpdateValues();
        }

        private void GoBack() {
            NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            RhitLocation location;
            string idString;
            if(NavigationContext.QueryString.TryGetValue("Id", out idString)) {
                int id = Convert.ToInt32(idString);
                location = DataCollector.Instance.GetLocation(Dispatcher, id);
            }
            else location = RhitMapView.Instance.SelectedLocation;
            if(location == null) GoBack();//TODO: Handle this better; Shouldn't give whiplash
            Location = location;
            if(location.Description == null || location.Description == "")
                DataCollector.Instance.GetLocationDescription(Dispatcher, Location.Id);
            UpdateValues();
        }
    }
}