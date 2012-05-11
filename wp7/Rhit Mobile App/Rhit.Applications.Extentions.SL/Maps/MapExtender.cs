using System;
using System.Collections.Generic;
using System.Windows;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl;
using Rhit.Applications.Extentions.Maps.Overlays;
#endif

namespace Rhit.Applications.Extentions.Maps {
    public class MapExtender {
        static MapExtender() {
            MapTileDictionary = new Dictionary<Map, MapTilesBinding>();
            Settings = MapSettings.Instance;
            Settings.CurrentSourceChanged += new EventHandler(Settings_SourceChanged);
            Settings.CurrentOverlayChanged += new EventHandler(Settings_CurrentOverlayChanged);
        }
       
        public MapExtender() { }

        public static MapSettings Settings { get; protected set; }

        private static void Settings_SourceChanged(object sender, EventArgs e) {
            UpdateSource();
        }

        static void Settings_CurrentOverlayChanged(object sender, EventArgs e) {
            UpdateOverlay();
        }

        private static Dictionary<Map, MapTilesBinding> MapTileDictionary { get; set; }

        public static void Attach(Map map) {
            if(MapTileDictionary.ContainsKey(map)) return;

            map.Mode = new RhitMapMode();
            MapTilesBinding binding = new MapTilesBinding(map);
            binding.Overlays.Add(Settings.CurrentOverlay);
            binding.MainTileSource = Settings.CurrentSource;
            MapTileDictionary[map] = binding;
        }

        public static void Dettach(Map map) {
            if(!MapTileDictionary.ContainsKey(map)) return;

            MapTileDictionary[map].DettachFromMap();
            MapTileDictionary.Remove(map);
        }

        private static void UpdateSource() {
            foreach(MapTilesBinding binding in MapTileDictionary.Values) 
                binding.MainTileSource = Settings.CurrentSource;
        }

        private static void UpdateOverlay() {
            foreach(MapTilesBinding binding in MapTileDictionary.Values) {
                binding.Overlays.Clear();
                if(Settings.CurrentOverlay != null || Settings.CurrentOverlay != Overlay.EmptyOverlay)
                    binding.Overlays.Add(Settings.CurrentOverlay);
                binding.ForceUpdate();
            }
        }
    }
}
