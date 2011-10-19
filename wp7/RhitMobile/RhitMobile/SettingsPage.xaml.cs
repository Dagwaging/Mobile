﻿using System;
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
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using RhitMobile.ObjectModel;
using RhitMobile.MapSource;

namespace RhitMobile {
    public partial class SettingsPage : PhoneApplicationPage {
        private List<ListPickerObject> bingTypes;
        private List<ListPickerObject> googleTypes;
        private List<ToggleSwitch> toggleSwitches;

        /// <summary> Constructor for the application's settings page. </summary>
        public SettingsPage() {
            InitializeComponent();
            List<ListPickerObject> sources = new List<ListPickerObject>() {
                new ListPickerObject() { Name = "Bing", },
                new ListPickerObject() { Name = "Google", },
                new ListPickerObject() { Name = "OSM Mapnik", },
                new ListPickerObject() { Name = "Osma Render", },
            };
            mapSourcePicker.ItemsSource = sources;
            mapSourcePicker.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(mapSourcePicker_SelectionChanged);

            bingTypes = new List<ListPickerObject>() {
                new ListPickerObject() { Name = "Aerial", },
                new ListPickerObject() { Name = "Hybrid", },
                new ListPickerObject() { Name = "Road", },
            };

            googleTypes = new List<ListPickerObject>() {
                new ListPickerObject() { Name = "Hybrid", },
                new ListPickerObject() { Name = "Physical", },
                new ListPickerObject() { Name = "Physical Hybrid", },
                new ListPickerObject() { Name = "Satellite", },
                new ListPickerObject() { Name = "Street", },
            };

            List<ListPickerObject> overlays = new List<ListPickerObject>() {
                new ListPickerObject() { Name = "Street Overlay", },
                new ListPickerObject() { Name = "Water Overlay", },
                new ListPickerObject() { Name = "RHIT Floor Plans", },
            };

            toggleSwitches = new List<ToggleSwitch>() {
                floorPlanToggle,
                waterToggle,
                streetToggle,
            };
        }

        void mapSourcePicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            string sourceName = ((ListPickerObject) mapSourcePicker.SelectedItem).Name;
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

        private void selectBase(string name) {
            if(name == null || name == string.Empty) return;
            foreach(ListPickerObject source in mapSourcePicker.Items)
                if(source.Name == name)
                    mapSourcePicker.SelectedItem = source;
        }

        private void selectType(string name) {
            if(name == null || name == string.Empty) return;
            foreach(ListPickerObject source in mapTypePicker.Items)
                if(source.Name == name)
                    mapTypePicker.SelectedItem = source;
        }

        public void LoadSettings() {
            BaseTileSource source = RhitMapView.Instance.CurrentTileSource;
            if(source is BaseBingSource) {
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

            polygonToggle.IsChecked = RhitMapView.Instance.AreOutlinesVisible;
            textToggle.IsChecked = RhitMapView.Instance.AreLabelsVisible;
            debugToggle.IsChecked = RhitMapView.Instance.InDebugMode;

            foreach(BaseTileSource overlay in RhitMapView.Instance.CurrentOverlaySources)
                foreach(ToggleSwitch toggle in toggleSwitches)
                    if(overlay.Name == (toggle.Header as string)) toggle.IsChecked = true;
        }

        public void SaveSettings() {
            string sourceName = ((ListPickerObject) mapSourcePicker.SelectedItem).Name;
            string sourceType = ((ListPickerObject) mapTypePicker.SelectedItem).Name;
            RhitMapView.Instance.ChangeTileSource(sourceName, sourceType);

            RhitMapView.Instance.InDebugMode = (bool) debugToggle.IsChecked;
            RhitMapView.Instance.AreLabelsVisible = (bool) textToggle.IsChecked;
            RhitMapView.Instance.AreOutlinesVisible = (bool) polygonToggle.IsChecked;


            foreach(ToggleSwitch toggle in toggleSwitches) {
                if((bool) toggle.IsChecked)
                    RhitMapView.Instance.AddOverlay(toggle.Header as string);
                else
                    RhitMapView.Instance.RemoveOverlay(toggle.Header as string);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            SaveSettings();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            LoadSettings();
        }
    }

    public class ListPickerObject {
        public string Name { get; set; }
    }
}
