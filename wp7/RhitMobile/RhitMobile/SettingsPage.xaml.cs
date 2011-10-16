using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using RhitMobile.MapSource;
using RhitMobile.ObjectModel;

namespace RhitMobile {
    public partial class SettingsPage : PhoneApplicationPage {
        List<ListPickerItem> bingTypes;
        List<ListPickerItem> googleTypes;

        /// <summary> Constructor for the application's settings page. </summary>
        public SettingsPage() {
            InitializeComponent();
            List<ListPickerItem> sources = new List<ListPickerItem>() {
                new ListPickerItem() { Name = "Bing", },
                new ListPickerItem() { Name = "Google", },
                new ListPickerItem() { Name = "OSM Mapnik", },
                new ListPickerItem() { Name = "Osma Render", },
            };
            mapSourcePicker.ItemsSource = sources;
            mapSourcePicker.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(mapSourcePicker_SelectionChanged);

            bingTypes = new List<ListPickerItem>() {
                new ListPickerItem() { Name = "Aerial", },
                new ListPickerItem() { Name = "Road", },
                new ListPickerItem() { Name = "Core Aerial", },
                new ListPickerItem() { Name = "Core Road", },
            };

            googleTypes = new List<ListPickerItem>() {
                new ListPickerItem() { Name = "Hybrid", },
                new ListPickerItem() { Name = "Physical", },
                new ListPickerItem() { Name = "Physical Hybrid", },
                new ListPickerItem() { Name = "Satellite", },
                new ListPickerItem() { Name = "Street", },
            };            
        }

        void mapSourcePicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            string sourceName = ((ListPickerItem) mapSourcePicker.SelectedItem).Name;
            if(sourceName == "Bing") {
                mapTypePicker.ItemsSource = bingTypes;
                mapTypePicker.Visibility = Visibility.Visible;
            } else if(sourceName == "Google") {
                mapTypePicker.ItemsSource = googleTypes;
                mapTypePicker.Visibility = Visibility.Visible;
            } else {
                mapTypePicker.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            string sourceName = ((ListPickerItem) mapSourcePicker.SelectedItem).Name;
            string sourceType = ((ListPickerItem) mapTypePicker.SelectedItem).Name;
            //this.SaveState("MapSource", source_name);
            if (mapTypePicker.Visibility == Visibility.Collapsed)
                RhitMapView.Instance.ChangeTileSource(sourceName);
            else
                RhitMapView.Instance.ChangeTileSource(sourceName, sourceType);

            RhitMapView.Instance.InDebugMode = (bool) debugToggle.IsChecked;
            RhitMapView.Instance.AreLabelsVisible = (bool) textToggle.IsChecked;
            RhitMapView.Instance.AreOutlinesVisible = (bool) polygonToggle.IsChecked;

            //this.SaveState("CurrentTileSource", sourceType);

            this.SaveState("TileOverlay", tileToggle.IsChecked);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            BaseTileSource source = RhitMapView.Instance.CurrentTileSource;
            if (source is BaseBingSource) {
                mapTypePicker.ItemsSource = bingTypes;
                mapTypePicker.Visibility = Visibility.Visible;
                selectBase("Bing");
                selectType(source.Name);
            } else if(source is BaseGoogleSource) {
                mapTypePicker.ItemsSource = googleTypes;
                mapTypePicker.Visibility = Visibility.Visible;
                selectBase("Google");
                selectType(source.Name);
            } else {
                mapTypePicker.Visibility = Visibility.Collapsed;
                selectBase(source.Name);
            }

            tileToggle.IsChecked = (bool) this.LoadState<object>("TileOverlay", false);

            polygonToggle.IsChecked = RhitMapView.Instance.AreOutlinesVisible;
            textToggle.IsChecked = RhitMapView.Instance.AreLabelsVisible;

            //TODO: [DEBUG] Remove debugToggle before release
            debugToggle.IsChecked = RhitMapView.Instance.InDebugMode;
        }
        
        private void selectBase(string name) {
            if(name == null || name == string.Empty) return;
            foreach(ListPickerItem source in mapSourcePicker.Items)
                if(source.Name == name)
                    mapSourcePicker.SelectedItem = source;
        }

        private void selectType(string name) {
            if(name == null || name == string.Empty) return;
            foreach(ListPickerItem source in mapTypePicker.Items)
                if(source.Name == name)
                    mapTypePicker.SelectedItem = source;
        }

        public class ListPickerItem {
            public string Name { get; set; }
        }
    }
}
