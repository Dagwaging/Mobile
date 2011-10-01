using System.Collections.Generic;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace RhitMobile {
    public partial class SettingsPage : PhoneApplicationPage {
        /// <summary> Constructor for the application's settings page. </summary>
        public SettingsPage() {
            InitializeComponent();
            List<MapTileSource> source = new List<MapTileSource>();
            source.Add(new MapTileSource() { Name = "Bing Aerial", });
            source.Add(new MapTileSource() { Name = "Bing Road", });
            source.Add(new MapTileSource() { Name = "OSM Mapnik", });
            source.Add(new MapTileSource() { Name = "Osma Render", });
            source.Add(new MapTileSource() { Name = "Google Hybrid", });
            source.Add(new MapTileSource() { Name = "Google Street", });
            this.mapSourcePicker.ItemsSource = source;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            string source = ((MapTileSource) mapSourcePicker.SelectedItem).Name;
            this.SaveState("MapSource", source);
            this.SaveState("TileOverlay", tileToggle.IsChecked);
            this.SaveState("AreOutlinesVisible", polygonToggle.IsChecked);
            this.SaveState("AreLabelsVisible", textToggle.IsChecked);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            string ms_name = this.LoadState<string>("MapSource");
            tileToggle.IsChecked = (bool) this.LoadState<object>("TileOverlay", false);
            polygonToggle.IsChecked = (bool) this.LoadState<object>("AreOutlinesVisible", false);
            textToggle.IsChecked = (bool) this.LoadState<object>("AreLabelsVisible", false);
            if(ms_name != null) {
                foreach(MapTileSource source in mapSourcePicker.Items) {
                    if(source.Name == ms_name)
                        mapSourcePicker.SelectedItem = source;
                }
            }
        }

        public class MapTileSource {
            public string Name {
                get;
                set;
            }
        }
    }
}
