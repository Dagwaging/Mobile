using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.ViewModel.Controllers;
using System.Collections.Generic;
using Microsoft.Phone.Controls.Maps;
using System.ComponentModel;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.ViewModel.Models {
    public class DirectionsViewModel : DependencyObject {
        public DirectionsViewModel() {
            Initialize();
        }

        private void Initialize() {
            if(DesignerProperties.IsInDesignTool) return;
            Map = MapController.Instance;
            Directions = PathsController.Instance;
            DataCollector.Instance.GetTestDirections();
        }

        public void SetMode(Map map) {
            map.Mode = Map.CurrentMode;

            List<UIElement> es = new List<UIElement>();
            foreach(UIElement e in map.Children) es.Add(e);
            map.Children.Clear();
            Map.TileLayer.ParentMap.Children.Remove(Map.TileLayer);
            map.Children.Add(Map.TileLayer);

            //Re-add elements put onto the map in the view
            foreach(UIElement e in es) map.Children.Add(e);
            Map.ZoomLevel = 20;
            return;
        }

        public MapController Map { get; protected set; }

        public PathsController Directions { get; private set; }
    }
}
