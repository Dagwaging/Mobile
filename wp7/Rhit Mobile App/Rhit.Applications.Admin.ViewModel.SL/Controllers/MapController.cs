using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Maps.Modes;
using Rhit.Applications.Model.Maps.Sources;
using Rhit.Applications.Model.Services;


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
            OverlayLayer = new MapTileLayer();
        }

        private void InitializeMap() {
            ZoomLevel = 16;
        }
        #endregion

        #region Update Methods
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
            if(CurrentSource == null) return;
            CurrentMode.ChangeSource(CurrentSource);

            TileLayer.TileSources.Clear();
            TileLayer.TileSources.Add(CurrentSource);
        }

        private void UpdateOverlays() {
            OverlayLayer.TileSources.Clear();
            if(FloorPlans) OverlayLayer.TileSources.Add(new RoseOverlay());
            if(GoogleWater) OverlayLayer.TileSources.Add(new GoogleSource(GoogleType.WaterOverlay));
            if(GoogleStreet) OverlayLayer.TileSources.Add(new GoogleSource(GoogleType.StreetOverlay));
        }
        #endregion

        #region Map Layers
        private MapTileLayer OverlayLayer { get; set; }

        public MapTileLayer TileLayer { get; set; }

        private MapLayer PolygonLayer { get; set; }

        private MapLayer TextLayer { get; set; }
        #endregion

        public ObservableCollection<RhitMode> Modes { get; set; }

        public ObservableCollection<BaseTileSource> Sources { get; set; }

        #region Property Changed Event Handlers
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdateSources();
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdateSource();
        }

        private static void OnOverlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdateOverlays();
        }
        #endregion

        #region Dependency Properties
        #region CurrentMode
        public RhitMode CurrentMode {
            get { return (RhitMode) GetValue(CurrentModeProperty); }
            set { SetValue(CurrentModeProperty, value); }
        }

        public static readonly DependencyProperty CurrentModeProperty =
           DependencyProperty.Register("CurrentMode", typeof(RhitMode), typeof(MapController),
           new PropertyMetadata(null, new PropertyChangedCallback(OnModeChanged)));
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
        #endregion

        #region Overlays
        #region FloorPlans
        public bool FloorPlans {
            get { return (bool) GetValue(FloorPlansProperty); }
            set { SetValue(FloorPlansProperty, value); }
        }

        public static readonly DependencyProperty FloorPlansProperty =
           DependencyProperty.Register("FloorPlans", typeof(bool), typeof(MapController),
           new PropertyMetadata(false, new PropertyChangedCallback(OnOverlayChanged)));
        #endregion

        #region GoogleStreet
        public bool GoogleStreet {
            get { return (bool) GetValue(GoogleStreetProperty); }
            set { SetValue(GoogleStreetProperty, value); }
        }

        public static readonly DependencyProperty GoogleStreetProperty =
           DependencyProperty.Register("GoogleStreet", typeof(bool), typeof(MapController),
           new PropertyMetadata(false, new PropertyChangedCallback(OnOverlayChanged)));
        #endregion

        #region GoogleWater
        public bool GoogleWater {
            get { return (bool) GetValue(GoogleWaterProperty); }
            set { SetValue(GoogleWaterProperty, value); }
        }

        public static readonly DependencyProperty GoogleWaterProperty =
           DependencyProperty.Register("GoogleWater", typeof(bool), typeof(MapController),
           new PropertyMetadata(false, new PropertyChangedCallback(OnOverlayChanged)));
        #endregion
        #endregion

        #region SourceChoices
        public bool SourceChoices {
            get { return (bool) GetValue(SourceChoicesProperty); }
            set { SetValue(SourceChoicesProperty, value); }
        }

        public static readonly DependencyProperty SourceChoicesProperty =
           DependencyProperty.Register("SourceChoices", typeof(bool), typeof(MapController), new PropertyMetadata(false));
        #endregion
        #endregion

            ///// <summary> Current zoom level of the map. </summary>
        //public double ZoomLevel {
        //    get { return _zoomLevel; }
        //    set {
        //        if(_zoomLevel > 1) return;
        //        if(_zoomLevel == value) return;
        //        _zoomLevel = value;
        //        Map.ZoomLevel = _zoomLevel;
        //        if(_textLayer == null) return;
        //        _textLayer.Children.Clear();
        //        foreach(RhitLocation location in Outlines)
        //            if(ShowLabels && value > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
        //    }
        //}


        //private void Map_MapZoom(object sender, MapZoomEventArgs e) {
        //    _zoomLevel = Map.ZoomLevel;
        //    _textLayer.Children.Clear();
        //    if(!ShowLabels) return;
        //    foreach(RhitLocation location in Outlines) //TODO: Make more efficient
        //        if(_zoomLevel > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
        //}
        //#endregion

    }
}
