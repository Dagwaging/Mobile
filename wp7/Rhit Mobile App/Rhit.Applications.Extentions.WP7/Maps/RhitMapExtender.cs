using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls.Maps;

namespace Rhit.Applications.Extentions.Maps {
    public class RhitMapExtender {
        static RhitMapExtender() {
            MapTileDictionary = new Dictionary<Map, MapTileLayer>();
            Settings = MapSettings.Instance;
            Settings.CurrentSourceChanged += new EventHandler(Settings_SourceChanged);
        }
        
        public RhitMapExtender() { }

        public static MapSettings Settings { get; protected set; }

        private static void Settings_SourceChanged(object sender, EventArgs e) {
            UpdateSource();
        }

        private static Dictionary<Map, MapTileLayer> MapTileDictionary { get; set; }

        public static void Attach(Map map) {
            if(MapTileDictionary.ContainsKey(map)) return;
            map.Mode = new RhitMapMode();

            List<UIElement> es = new List<UIElement>();
            foreach(UIElement e in map.Children) es.Add(e);
            map.Children.Clear();

            MapTileLayer tileLayer = new MapTileLayer();
            tileLayer.TileSources.Add(MapSettings.Instance.CurrentSource);
            MapTileDictionary[map] = tileLayer;
            map.Children.Add(tileLayer);

            //Re-add elements put onto the map in the view
            foreach(UIElement e in es) map.Children.Add(e);
        }

        public static void Dettach(Map map) {
            if(!MapTileDictionary.ContainsKey(map)) return;
            map.Children.Remove(MapTileDictionary[map]);
            map.Mode = new AerialMode();
            MapTileDictionary.Remove(map);
        }

        private static void UpdateSource() {
            foreach(MapTileLayer layer in MapTileDictionary.Values) {
                layer.TileSources.Clear();
                if(MapSettings.Instance.CurrentSource != null)
                    layer.TileSources.Add(MapSettings.Instance.CurrentSource);
            }
        }
    }
}
