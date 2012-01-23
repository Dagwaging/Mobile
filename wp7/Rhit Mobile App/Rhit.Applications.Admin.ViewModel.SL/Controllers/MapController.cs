using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model.Maps.Modes;
using Rhit.Applications.Model.Maps.Sources;

namespace Rhit.Applications.ViewModel.Controllers {
    public class MapController : DependencyObject {
        private static MapController _instance;

        private MapController() {
            CreateMapLayers();
            InitializeMapResources();
            InitializeMap();
        }

        #region Singleton Instance
        public static MapController Instance {
            get {
                if(_instance == null)
                    _instance = new MapController();
                return _instance;
            }
        }
        #endregion

        #region Instance Initializer Methods
        private void InitializeMapResources() {
            Sources = new ObservableCollection<BaseTileSource>();
            Modes = new ObservableCollection<RhitMode>() {
                new EmptyMode(),
                new BingMode(),
                new GoogleMode(),
                new MapnikMode(),
                new OsmaMode(),
            };
            CurrentMode = Modes[2];
        }

        private void CreateMapLayers() {
            TileLayer = new MapTileLayer();
        }

        private void InitializeMap() {
            ZoomLevel = 16;
        }
        #endregion

        private void UpdateSources() {
            List<BaseTileSource> sources = new List<BaseTileSource>();
            foreach(BaseTileSource source in Sources) sources.Add(source);
            foreach(BaseTileSource source in CurrentMode.Sources) Sources.Add(source);
            CurrentSource = CurrentMode.CurrentSource;
            foreach(BaseTileSource source in sources) Sources.Remove(source);
            if(Sources.Count > 1) SourceChoices = true;
            else SourceChoices = false;
        }
        
        private void UpdateSource() {
            TileLayer.TileSources.Clear();
            if(CurrentSource != null) {
                CurrentMode.ChangeSource(CurrentSource);
                TileLayer.TileSources.Add(CurrentSource);
            }
        }

        public MapTileLayer TileLayer { get; set; }

        public ObservableCollection<RhitMode> Modes { get; set; }

        public ObservableCollection<BaseTileSource> Sources { get; set; }

        #region CurrentMode
        public RhitMode CurrentMode {
            get { return (RhitMode) GetValue(CurrentModeProperty); }
            set { SetValue(CurrentModeProperty, value); }
        }

        public static readonly DependencyProperty CurrentModeProperty =
           DependencyProperty.Register("CurrentMode", typeof(RhitMode), typeof(MapController),
           new PropertyMetadata(null, new PropertyChangedCallback(OnModeChanged)));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdateSources();
        }
        #endregion

        #region ZoomLevel
        public double ZoomLevel {
            get { return (double) GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty ZoomLevelProperty =
           DependencyProperty.Register("ZoomLevel", typeof(double), typeof(MapController),new PropertyMetadata(16.0));
        #endregion

        #region Center
        public Location Center {
            get { return (Location) GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
           DependencyProperty.Register("Center", typeof(Location), typeof(MapController), new PropertyMetadata(new Location()));
        #endregion

        #region CurrentSource
        public BaseTileSource CurrentSource {
            get { return (BaseTileSource) GetValue(CurrentSourceProperty); }
            set { SetValue(CurrentSourceProperty, value); }
        }

        public static readonly DependencyProperty CurrentSourceProperty =
           DependencyProperty.Register("CurrentSource", typeof(BaseTileSource),
           typeof(MapController), new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdateSource();
        }
        #endregion

        #region SourceChoices
        public bool SourceChoices {
            get { return (bool) GetValue(SourceChoicesProperty); }
            set { SetValue(SourceChoicesProperty, value); }
        }

        public static readonly DependencyProperty SourceChoicesProperty =
           DependencyProperty.Register("SourceChoices", typeof(bool), typeof(MapController), new PropertyMetadata(false));
        #endregion
    }
}
