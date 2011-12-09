using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Maps.Modes;
using Rhit.Applications.Model.Maps.Sources;

namespace Rhit.Applications.ViewModel.Controllers {
    public class MapController : DependencyObject {
        private static MapController _instance;

        private MapController(Map map) {
            MapControl = map;
            CreateMapLayers();
            InitializeMapResources();
            InitializeMap();
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
            EventCoordinate = new GeoCoordinate(0, 0);
        }

        private void CreateMapLayers() {
            TileLayer = new MapTileLayer();
            OverlayLayer = new MapTileLayer();
            PolygonLayer = new MapLayer();
            TextLayer = new MapLayer();
        }

        private void InitializeMap() {
            MapControl.Mode = CurrentMode;
            MapControl.ZoomLevel = 16; //TODO: No Hard Coding
            MapControl.CopyrightVisibility = Visibility.Collapsed;
            MapControl.LogoVisibility = Visibility.Collapsed;
            MapControl.Tap += new EventHandler<GestureEventArgs>(MapControl_Tap);

            //Store elements put onto the map in the view
            List<UIElement> es = new List<UIElement>();
            foreach(UIElement e in MapControl.Children) es.Add(e);
            MapControl.Children.Clear();

            MapControl.Children.Add(TileLayer);
            MapControl.Children.Add(OverlayLayer);
            MapControl.Children.Add(PolygonLayer);
            MapControl.Children.Add(TextLayer);

            //Re-add elements put onto the map in the view
            foreach(UIElement e in es) MapControl.Children.Add(e);
        }
        #endregion

        #region Update Methods
        private void UpdateSources() {
            Sources.Clear();
            foreach(BaseTileSource source in CurrentMode.Sources)
                Sources.Add(source);
            CurrentSource = CurrentMode.CurrentSource;
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

        private void UpdatePolygons() {
            if(Outlines == null) return;
            if(AreOutlinesVisible)
                foreach(MapPolygon polygon in Outlines.Keys)
                    RhitLocation.ShowOutline(polygon);
            else foreach(MapPolygon polygon in Outlines.Keys)
                    RhitLocation.HideOutline(polygon);
        }

        private void UpdateLabels() {
            if(LabeledLocations == null) return;
            TextLayer.Children.Clear();
            foreach(RhitLocation location in LabeledLocations.Keys)
                if(ShouldShowLabel(location))
                    TextLayer.Children.Add(LabeledLocations[location]);
        }
        #endregion

        #region Map Events
        private void MapControl_Tap(object sender, GestureEventArgs e) {
            GeoCoordinate coordinate = MapControl.ViewportPointToLocation(e.GetPosition(MapControl));
            if(EventCoordinate == coordinate) return;
            UnSelect();
        }

        private void Outline_Tap(object sender, MouseButtonEventArgs e) {
            EventCoordinate = MapControl.ViewportPointToLocation(e.GetPosition(MapControl));
            SelectLocation(sender as MapPolygon);
        }
        #endregion

        #region Map Layers
        private MapTileLayer OverlayLayer { get; set; }

        private MapTileLayer TileLayer { get; set; }

        private MapLayer PolygonLayer { get; set; }

        private MapLayer TextLayer { get; set; }
        #endregion

        private void SelectLocation(MapPolygon polygon) {
            SelectLocation(Outlines[polygon]);
        }

        public void SelectLocation(RhitLocation location) {
            if(!AreOutlinesVisible) {
                if(CurrentLocation != null)
                    RhitLocation.HideOutline(CurrentLocation.OutLine);
                CurrentLocation = location;
                RhitLocation.ShowOutline(location.OutLine);
            } else {
                CurrentLocation = location;
            }
        }

        public void UnSelect() {
            if(CurrentLocation != null) {
                RhitLocation.HideOutline(CurrentLocation.OutLine);
                CurrentLocation = null;
            }
        }

        private List<RhitLocation> Locations { get; set; }

        private GeoCoordinate EventCoordinate { get; set; }

        private Dictionary<MapPolygon, RhitLocation> Outlines { get; set; }

        private Dictionary<RhitLocation, Pushpin> LabeledLocations { get; set; }

        public Map MapControl { get; private set; }

        public ObservableCollection<RhitMode> Modes { get; set; }

        public ObservableCollection<BaseTileSource> Sources { get; set; }

        public void SetLocations(List<RhitLocation> locations) {
            Locations = locations;
            LabeledLocations = new Dictionary<RhitLocation, Pushpin>();
            Outlines = new Dictionary<MapPolygon, RhitLocation>();
            PolygonLayer.Children.Clear();
            TextLayer.Children.Clear();
            foreach(RhitLocation location in Locations) {
                MapPolygon polygon = location.OutLine;
                if(polygon.Locations == null || polygon.Locations.Count <= 0) continue;
                PolygonLayer.Children.Add(polygon);
                if(!AreOutlinesVisible) RhitLocation.HideOutline(polygon);
                polygon.MouseLeftButtonUp +=new MouseButtonEventHandler(Outline_Tap);
                Outlines[polygon] = location;
                LabeledLocations[location] = location.GetLabel();
                if(ShouldShowLabel(location)) TextLayer.Children.Add(LabeledLocations[location]);
            }
        }

        private bool ShouldShowLabel(RhitLocation location) {
            //TODO: Add logic to handle maps with text on them already
            if(!AreLabelsVisible) return false;
            if(MapControl.ZoomLevel < location.MinZoomLevel) return false;
            return true;
        }

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

        private static void OnPolygonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdatePolygons();
        }

        private static void OnTextVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MapController instance = (MapController) d;
            instance.UpdateLabels();
        }
        #endregion

        #region Dependency Properties
        #region CurrentLocation
        public RhitLocation CurrentLocation {
            get { return (RhitLocation) GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        public static readonly DependencyProperty CurrentLocationProperty =
           DependencyProperty.Register("CurrentLocation", typeof(RhitLocation), typeof(MapController), new PropertyMetadata(null));
        #endregion

        #region CurrentMode
        public RhitMode CurrentMode {
            get { return (RhitMode) GetValue(CurrentModeProperty); }
            set { SetValue(CurrentModeProperty, value); }
        }

        public static readonly DependencyProperty CurrentModeProperty =
           DependencyProperty.Register("CurrentMode", typeof(RhitMode),
           typeof(MapController), new PropertyMetadata(null, new PropertyChangedCallback(OnModeChanged)));
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

        #region AreOutlinesVisible
        public bool AreOutlinesVisible {
            get { return (bool) GetValue(AreOutlinesVisibleProperty); }
            set { SetValue(AreOutlinesVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreOutlinesVisibleProperty =
           DependencyProperty.Register("AreOutlinesVisible", typeof(bool), typeof(MapController),
           new PropertyMetadata(false, new PropertyChangedCallback(OnPolygonVisibilityChanged)));
        #endregion

        #region AreLabelsVisible
        public bool AreLabelsVisible {
            get { return (bool) GetValue(AreLabelsVisibleProperty); }
            set { SetValue(AreLabelsVisibleProperty, value); }
        }

        public static readonly DependencyProperty AreLabelsVisibleProperty =
           DependencyProperty.Register("AreLabelsVisible", typeof(bool), typeof(MapController),
           new PropertyMetadata(false, new PropertyChangedCallback(OnTextVisibilityChanged)));
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



        //public RhitLocation SelectedLocation { get; private set; }

        ///// <summary> Pushpin to show when a location is selected. </summary>
        //public Pushpin SelectedPushpin { get; private set; }

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
        //            if(AreLabelsVisible && value > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
        //    }
        //}

        //private void SelectedPushpin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        //    if(Map.Children.Contains(SelectedPushpin)) Map.Children.Remove(SelectedPushpin);
        //    if(_lastSelected != null && !AreOutlinesVisible)
        //        RhitLocation.HideOutline(_lastSelected);
        //    _lastSelected = null;

        //    OnSelectedTap(new PushpinEventArgs() { SelectedPushpin = SelectedPushpin });
        //}

        //private void Polgon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        //    RhitLocation location = Select((MapPolygon) sender);
        //    RhitLocation.ShowOutline(location.OutLine);
        //    if(_lastSelected != null && !AreOutlinesVisible)
        //        RhitLocation.HideOutline(_lastSelected);
        //    _lastSelected = location.OutLine;

        //    OnTap(new OutlineEventArgs() { Outline = location.OutLine });
        //}

        //private void Map_MapZoom(object sender, MapZoomEventArgs e) {
        //    _zoomLevel = Map.ZoomLevel;
        //    _textLayer.Children.Clear();
        //    if(!AreLabelsVisible) return;
        //    foreach(RhitLocation location in Outlines) //TODO: Make more efficient
        //        if(_zoomLevel > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
        //}
        //#endregion


        //private void Initialize() {
        //    _locations = new List<RhitLocation>();
        //    User = new User();
        //    _polygonLayer = new MapLayer();
        //    _textLayer = new MapLayer();
        //    User.Pin.MouseLeftButtonUp += new MouseButtonEventHandler(UserPushpin_MouseLeftButtonUp);
        //    Map.Children.Add(User.Pin);
        //    SelectedPushpin = new Pushpin();
        //    SelectedPushpin.MouseLeftButtonUp += new MouseButtonEventHandler(SelectedPushpin_MouseLeftButtonUp);
        //    Map.Children.Add(_polygonLayer);
        //    Map.Children.Add(_textLayer);
        //    LoadData();
        //    GoToRhit(); //TODO: Load position from last time (Default here though)
        //}
        
        ///// <summary>
        ///// Makes Rose-Hulman is visible on the map.
        ///// </summary>
        //public void GoToRhit() {
        //    Map.Center = App.LOCATION_RHIT.Center;
        //    ZoomLevel = 16; //TODO: Don't hard code numbers
        //}

        ///// <summary>
        ///// Makes the user's current location visible on the map.
        ///// </summary>
        //public void GoToUserLocation() {
        //    if(User.Location == null) //TODO: Show Error Message
        //        return;
        //    Map.Center = User.Location;
        //    ZoomLevel = 18; //TODO: Don't hard code numbers
        //}

        ///// <summary>
        ///// Loads data for the map from isolated storage.
        ///// </summary>
        //public void LoadData() {
        //    ZoomLevel = (double) DataStorage.LoadState<object>(StorageKey.ZoomLevel, ZoomLevel);
        //    ChangeTileSource(DataStorage.LoadState<string>(StorageKey.TileSource, CurrentTileSource.Name));
        //    List<string> sourceNames = DataStorage.LoadState<List<string>>(StorageKey.Overlays, new List<string>());
        //    foreach(string sourceName in sourceNames) AddOverlay(sourceName);
        //    AreOutlinesVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleOutlines, AreOutlinesVisible);
        //    AreLabelsVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleLabels, AreLabelsVisible);
        //    User = DataStorage.LoadState<User>(StorageKey.User, User);
        //    User.Pin.MouseLeftButtonUp += new MouseButtonEventHandler(UserPushpin_MouseLeftButtonUp);
        //}

     
        ///// <summary>
        ///// Makes a location visible on the map.
        ///// </summary>
        ///// <param name="polygon">Polygon to make visible</param>
        ///// <returns>The location that was made visible</returns>
        //public RhitLocation Select(MapPolygon polygon) {
        //    foreach(RhitLocation location in Outlines)
        //        if(location.IsPolygonEqual(polygon)) {
        //            return Select(location);
        //        }
        //    return null;
        //}

        ///// <summary>
        ///// Makes a location visible on the map.
        ///// </summary>
        ///// <param name="location">Location to make visible</param>
        ///// <returns>The location that was made visible</returns>
        //public RhitLocation Select(RhitLocation location) {
        //    //TODO: This is ugly code
        //    if(location == null) return null;
        //    if(_lastSelected != null && !AreOutlinesVisible)
        //        RhitLocation.HideOutline(_lastSelected);
        //    foreach(RhitLocation _location in Outlines)
        //        if(_location.Label == location.Label) {
        //            _lastSelected = _location.OutLine;
        //            SelectedPushpin.Content = _location.Label + "\n(click for more)";
        //            SelectedPushpin.Location = _location.Center;
        //            if(!Map.Children.Contains(SelectedPushpin)) Map.Children.Add(SelectedPushpin);
        //            RhitLocation.ShowOutline(_location.OutLine);
        //            Map.Center = _location.Center;
        //            if(ZoomLevel < 18) ZoomLevel = 18;
        //            SelectedLocation = _location;
        //            return _location;
        //        }
        //    SelectedPushpin.Content = location.Label + "\n(click for more)";
        //    SelectedPushpin.Location = location.Center;
        //    if(!Map.Children.Contains(SelectedPushpin)) Map.Children.Add(SelectedPushpin);
        //    Map.Center = location.Center;
        //    if(ZoomLevel < 18) ZoomLevel = 18;
        //    SelectedLocation = location;
        //    return SelectedLocation;
        //}

        ///// <summary>
        ///// Stores map data to isolated storage.
        ///// </summary>
        //public void StoreData() {
        //    DataStorage.SaveState(StorageKey.ZoomLevel, ZoomLevel);
        //    DataStorage.SaveState(StorageKey.TileSource, CurrentTileSource.Name);
        //    List<string> sourceNames = new List<string>();
        //    foreach(BaseTileSource source in CurrentOverlaySources)
        //        sourceNames.Add(source.Name);
        //    DataStorage.SaveState(StorageKey.Overlays, sourceNames);
        //    DataStorage.SaveState(StorageKey.VisibleOutlines, AreOutlinesVisible);
        //    DataStorage.SaveState(StorageKey.VisibleLabels, AreLabelsVisible);
        //    DataStorage.SaveState(StorageKey.User, User);
        //    //TODO: Cache Map Tiles
        //}
    }
}
