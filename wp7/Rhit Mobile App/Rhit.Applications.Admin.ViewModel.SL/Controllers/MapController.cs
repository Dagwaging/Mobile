using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Maps.Modes;
using Rhit.Applications.Model.Maps.Sources;
using Rhit.Applications.Model.Services;
using Microsoft.Maps.MapControl;


namespace Rhit.Applications.ViewModel.Controllers {
    public class MapController : DependencyObject {
        private static MapController _instance;

        private MapController(Map map) {
            CreateMapLayers();
            MapControl = map;
            InitializeMapResources();
            InitializeMap();
            Load();
        }


        public static void CreateMapController(Map map) {
            _instance = new MapController(map);
        }

        #region Singleton Instance
        public static MapController Instance {
            get { return _instance; }
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
            //PolygonLayer = new MapLayer();
            //TextLayer = new MapLayer();
        }

        private void InitializeMap() {
            MapControl.Mode = CurrentMode;
            MapControl.ZoomLevel = 16; //TODO: No Hard Coding
            MapControl.CopyrightVisibility = Visibility.Collapsed;
            MapControl.LogoVisibility = Visibility.Collapsed;
            //MapControl.Tap += new EventHandler<GestureEventArgs>(MapControl_Tap);
            MapControl.CacheMode = new BitmapCache();

            //Store elements put onto the map in the view
            List<UIElement> es = new List<UIElement>();
            foreach(UIElement e in MapControl.Children) es.Add(e);
            MapControl.Children.Clear();

            MapControl.Children.Add(TileLayer);
            //MapControl.Children.Add(OverlayLayer);
            //MapControl.Children.Add(PolygonLayer);
            //MapControl.Children.Add(TextLayer);

            //Re-add elements put onto the map in the view
            foreach(UIElement e in es) MapControl.Children.Add(e);
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

        private MapTileLayer TileLayer { get; set; }

        private MapLayer PolygonLayer { get; set; }

        private MapLayer TextLayer { get; set; }
        #endregion

        public Map MapControl { get; private set; }

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

        public void Save() {
            DataStorage.SaveState(StorageKey.ZoomLevel, MapControl.ZoomLevel);
            DataStorage.SaveState(StorageKey.MapCenter, MapControl.Center);
            DataStorage.SaveState(StorageKey.MapMode, CurrentMode.Label);
            DataStorage.SaveState(StorageKey.TileSource, CurrentSource.Label);
            DataStorage.SaveState(StorageKey.RoseOverlay, FloorPlans);
        }

        public void Load() {
            if(MapControl == null) return;
            MapControl.ZoomLevel = (double) DataStorage.LoadState<object>(StorageKey.ZoomLevel, MapControl.ZoomLevel);
            MapControl.Center = (GeoCoordinate) DataStorage.LoadState<GeoCoordinate>(StorageKey.MapCenter, MapControl.Center as GeoCoordinate);

            string modeLabel = DataStorage.LoadState<string>(StorageKey.MapMode, string.Empty);
            if(modeLabel != string.Empty) foreach(RhitMode mode in Modes) if(mode.Label == modeLabel) CurrentMode = mode;

            string sourceLabel = DataStorage.LoadState<string>(StorageKey.TileSource, string.Empty);
            if(sourceLabel != string.Empty) foreach(BaseTileSource source in Sources) if(source.Label == sourceLabel) CurrentSource = source;

            FloorPlans = (bool) DataStorage.LoadState<object>(StorageKey.RoseOverlay, FloorPlans);
        }

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
