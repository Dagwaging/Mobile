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
using Microsoft.Maps.MapControl;
using System.Collections.Generic;
using Rhit.Applications.Extentions.Maps.Overlays;

namespace Rhit.Applications.Extentions.Maps {
    public class MapTilesBinding {
        public MapTilesBinding(Map map) {
            Overlays = new List<TileSource>();
            TileContainer = new MapTileLayer();
            OverlayContainer = new MapTileLayer();
            BoundMap = map;
            AttachToMap();
        }

        public Map BoundMap { get; protected set; }

        private MapTileLayer TileContainer { get; set; }
        private MapTileLayer OverlayContainer { get; set; }

        public List<TileSource> Overlays { get; protected set; }

        protected void AttachToMap() {
            List<UIElement> elements = new List<UIElement>();
            foreach(UIElement element in BoundMap.Children)
                elements.Add(element);
            BoundMap.Children.Clear();

            BoundMap.Children.Add(TileContainer);
            BoundMap.Children.Add(OverlayContainer);

            foreach(UIElement e in elements)
                BoundMap.Children.Add(e);
        }

        public void DettachFromMap() {
            if(BoundMap.Children.Contains(TileContainer))
                BoundMap.Children.Remove(TileContainer);
            BoundMap.Mode = new AerialMode();
        }

        private TileSource mainTileSource;
        internal TileSource MainTileSource {
            get { return mainTileSource; }
            set {
                if(value == null) return;
                if(mainTileSource == value) return;
                mainTileSource = value;
                ApplySources();
            }
        }

        protected void ApplySources() {
            TileContainer.TileSources.Clear();
            OverlayContainer.TileSources.Clear();
            TileContainer.TileSources.Add(mainTileSource);
            foreach(TileSource source in Overlays) {
                if(!(source is Overlay)) continue;
                if((source as Overlay).IsEmpty()) continue;
                OverlayContainer.TileSources.Add(source);
            }
        }

        internal void ForceUpdate() {
            ApplySources();
        }
    }
}
