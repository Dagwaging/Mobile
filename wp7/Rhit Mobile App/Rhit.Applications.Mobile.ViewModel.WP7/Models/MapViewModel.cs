using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Controllers;
using System;
using Microsoft.Phone.Controls.Maps;

namespace Rhit.Applications.ViewModel.Models {
    public class MapViewModel : DependencyObject {
        

        //NOTE: Requires a call to Initialize and SetMode before class is usable
        //Note: NoArg Constructor so ViewModel can be created in xaml
        public MapViewModel() {}

        public void Initialize() {

            Locations = LocationsController.Instance;

            

            

            

            

            ShowBuildings = true;

            
            
            Map = MapController.Instance;

            
        }

        public void SetMode(Map map) {
            //map.Mode = Map.CurrentMode;

            //List<UIElement> es = new List<UIElement>();
            //foreach(UIElement e in map.Children) es.Add(e);
            //map.Children.Clear();

            //map.Children.Add(Map.TileLayer);

            ////Re-add elements put onto the map in the view
            //foreach(UIElement e in es) map.Children.Add(e);
            //Map.ZoomLevel = 16;
            //if(Locations.CurrentLocation != null)
            //    Map.Center = Locations.CurrentLocation.Center;
        }

        private void CurrentLocationChanged(object sender, EventArgs e) {
            if(LocationsController.Instance.CurrentLocation != null)
                Map.Center = LocationsController.Instance.CurrentLocation.Center;
        }

        public MapController Map { get; private set; }

        public LocationsController Locations { get; private set; }


        #region Dependency Properties
        #region ShowBuildings
        public bool ShowBuildings {
            get { return (bool) GetValue(ShowBuildingsProperty); }
            set { SetValue(ShowBuildingsProperty, value); }
        }

        public static readonly DependencyProperty ShowBuildingsProperty =
           DependencyProperty.Register("ShowBuildings", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowAllLocations
        public bool ShowAllLocations {
            get { return (bool) GetValue(ShowAllLocationsProperty); }
            set { SetValue(ShowAllLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowAllLocationsProperty =
           DependencyProperty.Register("ShowAllLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowInnerLocations
        public bool ShowInnerLocations {
            get { return (bool) GetValue(ShowInnerLocationsProperty); }
            set { SetValue(ShowInnerLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowInnerLocationsProperty =
           DependencyProperty.Register("ShowInnerLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion

        #region ShowTopLocations
        public bool ShowTopLocations {
            get { return (bool) GetValue(ShowTopLocationsProperty); }
            set { SetValue(ShowTopLocationsProperty, value); }
        }

        public static readonly DependencyProperty ShowTopLocationsProperty =
           DependencyProperty.Register("ShowTopLocations", typeof(bool), typeof(MapViewModel), new PropertyMetadata(false));
        #endregion
        #endregion


        
    }
}
