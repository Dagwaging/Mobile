#region Original MapController
//        using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Windows;
//using System.Windows.Input;
//using System.Windows.Media;
//using Rhit.Applications.Model;
//using Rhit.Applications.Model.Events;
//using Rhit.Applications.Model.Maps.Modes;
//using Rhit.Applications.Model.Maps.Sources;
//using Rhit.Applications.Model.Services;
//using Microsoft.Maps.MapControl;


//namespace Rhit.Applications.ViewModel.Controllers {
//    public class MapController : DependencyObject {
//        private static MapController _instance;

//        private MapController(Map map) {
//            Initialize();
//            MapControl = map;
//            CreateMapLayers();
//            InitializeMapResources();
//            InitializeMap();
//            Load();
//        }

//        private void LocationsChanged(object sender, LocationEventArgs e) {
//            if(e.NewLocations == null) return;
//            UpdateLayers(e.NewLocations);
//        }

//        private void UpdateLayers(ICollection<RhitLocation> locations) {
//            LabeledLocations.Clear();
//            Outlines.Clear();
//            PolygonLayer.Children.Clear();
//            TextLayer.Children.Clear();

//            foreach(RhitLocation location in locations) {
//                MapPolygon polygon = location.OutLine;
//                if(polygon.Locations == null || polygon.Locations.Count <= 0) continue;
//                PolygonLayer.Children.Add(polygon);
//                if(!AreOutlinesVisible) RhitLocation.HideOutline(polygon);
//                polygon.MouseLeftButtonUp += new MouseButtonEventHandler(Outline_Tap);
//                Outlines[polygon] = location;
//                LabeledLocations[location] = location.GetLabel();
//                if(ShouldShowLabel(location)) TextLayer.Children.Add(LabeledLocations[location]);
//            }
//        }

//        private void CurrentLocationChanged(object sender, LocationEventArgs e) {
//            if(!AreOutlinesVisible) {
//                if(e.NewLocation == null)
//                    RhitLocation.HideOutline(e.OldLocation.OutLine);
//                else {
//                    if(e.OldLocation != null)
//                        RhitLocation.HideOutline(e.OldLocation.OutLine);
//                    RhitLocation.ShowOutline(e.NewLocation.OutLine);
//                }
//            }
//            if(e.NewLocation != null)
//                MapControl.Center = e.NewLocation.Center;
//        }

//        public static void CreateMapController(Map map) {
//            _instance = new MapController(map);
//        }

//        #region Singleton Instance
//        public static MapController Instance {
//            get { return _instance; }
//        }
//        #endregion

//        #region Instance Initializer Methods
//        private void Initialize() {
//            LocationsController.Instance.CurrentLocationChanged += new LocationEventHandler(CurrentLocationChanged);
//            LocationsController.Instance.LocationsChanged += new LocationEventHandler(LocationsChanged);
//            LabeledLocations = new Dictionary<RhitLocation, Pushpin>();
//            Outlines = new Dictionary<MapPolygon, RhitLocation>();
//        }

//        private void InitializeMapResources() {
//            Sources = new ObservableCollection<BaseTileSource>();
//            Modes = new ObservableCollection<RhitMode>() {
//                new EmptyMode(),
//                new BingMode(),
//                new GoogleMode(),
//                new MapnikMode(),
//                new OsmaMode(),
//            };
//            CurrentMode = Modes[2];
//            EventCoordinate = new GeoCoordinate(0, 0);
//        }

//        private void CreateMapLayers() {
//            TileLayer = new MapTileLayer();
//            OverlayLayer = new MapTileLayer();
//            PolygonLayer = new MapLayer();
//            TextLayer = new MapLayer();
//        }

//        private void InitializeMap() {
//            MapControl.Mode = CurrentMode;
//            MapControl.ZoomLevel = 16; //TODO: No Hard Coding
//            MapControl.CopyrightVisibility = Visibility.Collapsed;
//            MapControl.LogoVisibility = Visibility.Collapsed;
//            MapControl.MouseClick += new EventHandler<MapMouseEventArgs>(MouseClick);
//            //MapControl.Tap += new EventHandler<GestureEventArgs>(MapControl_Tap);
//            MapControl.CacheMode = new BitmapCache();

//            //Store elements put onto the map in the view
//            List<UIElement> es = new List<UIElement>();
//            foreach(UIElement e in MapControl.Children) es.Add(e);
//            MapControl.Children.Clear();

//            MapControl.Children.Add(TileLayer);
//            MapControl.Children.Add(OverlayLayer);
//            MapControl.Children.Add(PolygonLayer);
//            MapControl.Children.Add(TextLayer);

//            //Re-add elements put onto the map in the view
//            foreach(UIElement e in es) MapControl.Children.Add(e);
//        }


//        #endregion

//        #region Update Methods
//        private void UpdateSources() {
//            List<BaseTileSource> sources = new List<BaseTileSource>();
//            foreach(BaseTileSource source in Sources) sources.Add(source);
//            foreach(BaseTileSource source in CurrentMode.Sources) Sources.Add(source);
//            CurrentSource = CurrentMode.CurrentSource;
//            foreach(BaseTileSource source in sources) Sources.Remove(source);
//            if(Sources.Count > 1) SourceChoices = true;
//            else SourceChoices = false;
//        }

//        private void UpdateSource() {
//            if(CurrentSource == null) return;
//            CurrentMode.ChangeSource(CurrentSource);
//            TileLayer.TileSources.Clear();
//            TileLayer.TileSources.Add(CurrentSource);
//        }

//        private void UpdateOverlays() {
//            OverlayLayer.TileSources.Clear();
//            if(FloorPlans) OverlayLayer.TileSources.Add(new RoseOverlay());
//            if(GoogleWater) OverlayLayer.TileSources.Add(new GoogleSource(GoogleType.WaterOverlay));
//            if(GoogleStreet) OverlayLayer.TileSources.Add(new GoogleSource(GoogleType.StreetOverlay));
//        }

//        private void UpdatePolygons() {
//            if(Outlines == null) return;
//            if(AreOutlinesVisible)
//                foreach(MapPolygon polygon in Outlines.Keys)
//                    RhitLocation.ShowOutline(polygon);
//            else foreach(MapPolygon polygon in Outlines.Keys)
//                    RhitLocation.HideOutline(polygon);
//        }

//        private void UpdateLabels() {
//            if(LabeledLocations == null) return;
//            TextLayer.Children.Clear();
//            foreach(RhitLocation location in LabeledLocations.Keys)
//                if(ShouldShowLabel(location))
//                    TextLayer.Children.Add(LabeledLocations[location]);
//        }
//        #endregion

//        #region Map Event Handlers
//        private void MapControl_Tap(object sender, GestureEventArgs e) {
//            GeoCoordinate coordinate = (GeoCoordinate) MapControl.ViewportPointToLocation(e.GetPosition(MapControl));
//            if(EventCoordinate == coordinate) return;
//            LocationsController.Instance.UnSelect();
//        }

//        void MouseClick(object sender, MapMouseEventArgs e) {
//            //GeoCoordinate coordinate = (GeoCoordinate) MapControl.ViewportPointToLocation(e.ViewportPoint);
//            //if(EventCoordinate == coordinate) return;
//            //LocationsController.Instance.UnSelect();
//        }

//        private void Outline_Tap(object sender, MouseButtonEventArgs e) {
//            //EventCoordinate = (GeoCoordinate) MapControl.ViewportPointToLocation(e.GetPosition(MapControl));
//            //SelectLocation(sender as MapPolygon);
//        }
//        #endregion

//        #region Map Layers
//        private MapTileLayer OverlayLayer { get; set; }

//        private MapTileLayer TileLayer { get; set; }

//        private MapLayer PolygonLayer { get; set; }

//        private MapLayer TextLayer { get; set; }
//        #endregion

//        private void SelectLocation(MapPolygon polygon) {
//            LocationsController.Instance.SelectLocation(Outlines[polygon]);
//        }

//        public GeoCoordinate EventCoordinate { get; set; }

//        private Dictionary<MapPolygon, RhitLocation> Outlines { get; set; }

//        private Dictionary<RhitLocation, Pushpin> LabeledLocations { get; set; }

//        public Map MapControl { get; private set; }

//        public ObservableCollection<RhitMode> Modes { get; set; }

//        public ObservableCollection<BaseTileSource> Sources { get; set; }

//        private bool ShouldShowLabel(RhitLocation location) {
//            //TODO: Add logic to handle maps with text on them already
//            if(!AreLabelsVisible) return false;
//            if(MapControl.ZoomLevel < location.MinZoomLevel) return false;
//            return true;
//        }

//        #region Property Changed Event Handlers
//        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//            MapController instance = (MapController) d;
//            instance.UpdateSources();
//        }

//        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//            MapController instance = (MapController) d;
//            instance.UpdateSource();
//        }

//        private static void OnOverlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//            MapController instance = (MapController) d;
//            instance.UpdateOverlays();
//        }

//        private static void OnPolygonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//            MapController instance = (MapController) d;
//            instance.UpdatePolygons();
//        }

//        private static void OnTextVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
//            MapController instance = (MapController) d;
//            instance.UpdateLabels();
//        }
//        #endregion

//        #region Dependency Properties
//        #region CurrentMode
//        public RhitMode CurrentMode {
//            get { return (RhitMode) GetValue(CurrentModeProperty); }
//            set { SetValue(CurrentModeProperty, value); }
//        }

//        public static readonly DependencyProperty CurrentModeProperty =
//           DependencyProperty.Register("CurrentMode", typeof(RhitMode),
//           typeof(MapController), new PropertyMetadata(null, new PropertyChangedCallback(OnModeChanged)));
//        #endregion

//        #region CurrentSource
//        public BaseTileSource CurrentSource {
//            get { return (BaseTileSource) GetValue(CurrentSourceProperty); }
//            set { SetValue(CurrentSourceProperty, value); }
//        }

//        public static readonly DependencyProperty CurrentSourceProperty =
//           DependencyProperty.Register("CurrentSource", typeof(BaseTileSource),
//           typeof(MapController), new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));
//        #endregion

//        #region Overlays
//        #region FloorPlans
//        public bool FloorPlans {
//            get { return (bool) GetValue(FloorPlansProperty); }
//            set { SetValue(FloorPlansProperty, value); }
//        }

//        public static readonly DependencyProperty FloorPlansProperty =
//           DependencyProperty.Register("FloorPlans", typeof(bool), typeof(MapController),
//           new PropertyMetadata(false, new PropertyChangedCallback(OnOverlayChanged)));
//        #endregion

//        #region GoogleStreet
//        public bool GoogleStreet {
//            get { return (bool) GetValue(GoogleStreetProperty); }
//            set { SetValue(GoogleStreetProperty, value); }
//        }

//        public static readonly DependencyProperty GoogleStreetProperty =
//           DependencyProperty.Register("GoogleStreet", typeof(bool), typeof(MapController),
//           new PropertyMetadata(false, new PropertyChangedCallback(OnOverlayChanged)));
//        #endregion

//        #region GoogleWater
//        public bool GoogleWater {
//            get { return (bool) GetValue(GoogleWaterProperty); }
//            set { SetValue(GoogleWaterProperty, value); }
//        }

//        public static readonly DependencyProperty GoogleWaterProperty =
//           DependencyProperty.Register("GoogleWater", typeof(bool), typeof(MapController),
//           new PropertyMetadata(false, new PropertyChangedCallback(OnOverlayChanged)));
//        #endregion

//        #region AreOutlinesVisible
//        public bool AreOutlinesVisible {
//            get { return (bool) GetValue(AreOutlinesVisibleProperty); }
//            set { SetValue(AreOutlinesVisibleProperty, value); }
//        }

//        public static readonly DependencyProperty AreOutlinesVisibleProperty =
//           DependencyProperty.Register("AreOutlinesVisible", typeof(bool), typeof(MapController),
//           new PropertyMetadata(false, new PropertyChangedCallback(OnPolygonVisibilityChanged)));
//        #endregion

//        #region AreLabelsVisible
//        public bool AreLabelsVisible {
//            get { return (bool) GetValue(AreLabelsVisibleProperty); }
//            set { SetValue(AreLabelsVisibleProperty, value); }
//        }

//        public static readonly DependencyProperty AreLabelsVisibleProperty =
//           DependencyProperty.Register("AreLabelsVisible", typeof(bool), typeof(MapController),
//           new PropertyMetadata(false, new PropertyChangedCallback(OnTextVisibilityChanged)));
//        #endregion
//        #endregion

//        #region SourceChoices
//        public bool SourceChoices {
//            get { return (bool) GetValue(SourceChoicesProperty); }
//            set { SetValue(SourceChoicesProperty, value); }
//        }

//        public static readonly DependencyProperty SourceChoicesProperty =
//           DependencyProperty.Register("SourceChoices", typeof(bool), typeof(MapController), new PropertyMetadata(false));
//        #endregion
//        #endregion

//        public void Save() {
//            DataStorage.SaveState(StorageKey.ZoomLevel, MapControl.ZoomLevel);
//            DataStorage.SaveState(StorageKey.MapCenter, MapControl.Center);
//            DataStorage.SaveState(StorageKey.MapMode, CurrentMode.Label);
//            DataStorage.SaveState(StorageKey.TileSource, CurrentSource.Label);
//            DataStorage.SaveState(StorageKey.RoseOverlay, FloorPlans);
//            DataStorage.SaveState(StorageKey.VisibleOutlines, AreOutlinesVisible);
//            DataStorage.SaveState(StorageKey.VisibleLabels, AreLabelsVisible);
//        }

//        public void Load() {
//            if(MapControl == null) return;
//            MapControl.ZoomLevel = (double) DataStorage.LoadState<object>(StorageKey.ZoomLevel, MapControl.ZoomLevel);
//            MapControl.Center = (GeoCoordinate) DataStorage.LoadState<GeoCoordinate>(StorageKey.MapCenter, MapControl.Center as GeoCoordinate);

//            string modeLabel = DataStorage.LoadState<string>(StorageKey.MapMode, string.Empty);
//            if(modeLabel != string.Empty) foreach(RhitMode mode in Modes) if(mode.Label == modeLabel) CurrentMode = mode;

//            string sourceLabel = DataStorage.LoadState<string>(StorageKey.TileSource, string.Empty);
//            if(sourceLabel != string.Empty) foreach(BaseTileSource source in Sources) if(source.Label == sourceLabel) CurrentSource = source;

//            AreOutlinesVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleOutlines, AreOutlinesVisible);
//            AreLabelsVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleLabels, AreLabelsVisible);
//            FloorPlans = (bool) DataStorage.LoadState<object>(StorageKey.RoseOverlay, FloorPlans);
//        }

//            ///// <summary> Current zoom level of the map. </summary>
//        //public double ZoomLevel {
//        //    get { return _zoomLevel; }
//        //    set {
//        //        if(_zoomLevel > 1) return;
//        //        if(_zoomLevel == value) return;
//        //        _zoomLevel = value;
//        //        Map.ZoomLevel = _zoomLevel;
//        //        if(_textLayer == null) return;
//        //        _textLayer.Children.Clear();
//        //        foreach(RhitLocation location in Outlines)
//        //            if(AreLabelsVisible && value > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
//        //    }
//        //}


//        //private void Map_MapZoom(object sender, MapZoomEventArgs e) {
//        //    _zoomLevel = Map.ZoomLevel;
//        //    _textLayer.Children.Clear();
//        //    if(!AreLabelsVisible) return;
//        //    foreach(RhitLocation location in Outlines) //TODO: Make more efficient
//        //        if(_zoomLevel > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
//        //}
//        //#endregion

//    }
//}
#endregion