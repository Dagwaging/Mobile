using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using RhitMobile.Events;
using RhitMobile.MapSource;
using RhitMobile.Services;

namespace RhitMobile.ObjectModel {
    /// <summary>
    /// Singleton class to handle the map object.
    /// </summary>
    public class RhitMapView {
        #region Private Variables
        private static RhitMapView _instance;
        private Map _map;
        private BaseTileSource _tileSource;
        private List<BaseTileSource> _overlaySources;
        private List<RhitLocation> _locations;
        private MapTileLayer _mapTileLayer;
        private MapTileLayer _mapOverlayLayer;
        private MapLayer _polygonLayer;
        private MapLayer _textLayer;
        private MapPolygon _lastSelected;
        private double _zoomLevel;
        private bool _areOutlinesVisible;
        private bool _areLabelsVisible;
        private string _debugText;
        private bool _inDebugMode;
        #endregion

        #region Events
        public event DebugEventHandler DebugTextChanged;

        public event DebugEventHandler DebugModeChanged;

        public event OutlineEventHandler OutlineTapped;

        public event PushpinEventHandler PushpinTapped;
        #endregion

        private RhitMapView() {
            CreateAvailableSources();
            Map = new Map();
            Initialize();
            Map.MapZoom += new EventHandler<MapZoomEventArgs>(Map_MapZoom);

            InDebugMode = false;
            DebugText = " For Testing Purposes: Will be removed for Release.";
        }

        #region Public Properties
        /// <summary> Should the labels be visible? </summary>
        public bool AreLabelsVisible {
            get { return _areLabelsVisible; }
            set {
                if(value == _areLabelsVisible) return;
                _areLabelsVisible = value;
                _textLayer.Children.Clear();
                if(value) foreach(RhitLocation location in _locations)
                        if(ZoomLevel > location.MinZoomLevel)
                            _textLayer.Children.Add(location.GetLabel());
            }
        }

        /// <summary> Should the polygons be visible? </summary>
        public bool AreOutlinesVisible {
            get { return _areOutlinesVisible; }
            set {
                if(value == _areOutlinesVisible) return;
                _areOutlinesVisible = value;
                foreach(UIElement element in _polygonLayer.Children) {
                    if(!value) RhitLocation.HideOutline((MapPolygon) element);
                    else RhitLocation.ShowOutline((MapPolygon) element);
                }
            }
        }

        /// <summary> Current tile source for the map. </summary>
        public BaseTileSource CurrentTileSource {
            get { return _tileSource; }
            set {
                if(value.Equals(CurrentTileSource)) return;
                if(!TileSources.Contains(value)) return;
                _tileSource = value;
                Map.Mode = _tileSource.Mode;
                _mapTileLayer.TileSources.Clear();
                _mapTileLayer.TileSources.Add(_tileSource);
                //TODO: Raise source change event?
            }
        }

        /// <summary> The overlays currently applied to the map. </summary>
        public List<BaseTileSource> CurrentOverlaySources {
            get { return _overlaySources; }
            set {
                if(value.Equals(CurrentOverlaySources)) return;
                _overlaySources.Clear();
                _mapOverlayLayer.TileSources.Clear();
                foreach(BaseTileSource source in value) {
                    if(!OverlaySources.Contains(source)) continue;
                    _overlaySources.Add(source);
                    _mapOverlayLayer.TileSources.Add(source);
                }
                //TODO: Raise source change event?
            }
        }

        /// <summary>
        /// The text to be displayed if in debug mode.
        /// </summary>
        public string DebugText {
            get { return _debugText; }
            set {
                _debugText = value;
                OnDebugTextChanged(new DebugEventArgs());
            }
        }

        /// <summary>
        /// True if in debug mode.
        /// </summary>
        public bool InDebugMode {
            get { return _inDebugMode; }
            set {
                if(_inDebugMode == value) return;
                _inDebugMode = value;
                OnDebugModeChanged(new DebugEventArgs());
            }
        }

        /// <summary> The single instance of this class. </summary>
        public static RhitMapView Instance {
            get {
                if(_instance == null) _instance = new RhitMapView();
                return _instance;
            }
        }

        /// <summary> The map object to add to the GUI. </summary>
        public Map Map {
            get { return _map; }
            private set {
                _map = value;
                _map.CredentialsProvider = new ApplicationIdCredentialsProvider(App.MapId);
                _map.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Map_MouseLeftButtonUp);
                ZoomLevel = _map.ZoomLevel;
            }
        }

        /// <summary>
        /// All of the available location received from the service.
        /// </summary>
        public List<RhitLocation> Outlines {
            get { return _locations; }
            set {
                _locations = value;
                _polygonLayer.Children.Clear();
                _textLayer.Children.Clear();
                foreach(RhitLocation location in _locations) {
                    if(location.OutLine == null || location.OutLine.Locations.Count <= 0) continue;
                    if(!AreOutlinesVisible) RhitLocation.HideOutline(location.OutLine);
                    else RhitLocation.ShowOutline(location.OutLine);
                    location.OutLine.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Polgon_MouseLeftButtonUp);
                    _polygonLayer.Children.Add(location.OutLine);
                    if(AreLabelsVisible && ZoomLevel > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
                }
                DebugText = _polygonLayer.Children.Count.ToString();
            }
        }

        /// <summary> The available overlays to add to the map. </summary>
        public List<BaseTileSource> OverlaySources { get; private set; }

        public RhitLocation SelectedLocation { get; private set; }

        /// <summary> Pushpin to show when a location is selected. </summary>
        public Pushpin SelectedPushpin { get; private set; }

        /// <summary>
        /// The available tile sources to be applied to the map.
        /// </summary>
        public List<BaseTileSource> TileSources { get; private set; }

        /// <summary>
        /// Data container for information specific to the user.
        /// </summary>
        public User User { get; private set; }

        /// <summary> Current zoom level of the map. </summary>
        public double ZoomLevel {
            get { return _zoomLevel; }
            set {
                if(_zoomLevel > 1) return;
                if(_zoomLevel == value) return;
                _zoomLevel = value;
                Map.ZoomLevel = _zoomLevel;
                if(_textLayer == null) return;
                _textLayer.Children.Clear();
                foreach(RhitLocation location in Outlines)
                    if(AreLabelsVisible && value > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
            }
        }
        #endregion

        #region Event Handler Callabacks
        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            //TODO: If there is a selected polygon, then unselect it
        }

        private void UserPushpin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            //TODO: Do something here
        }

        private void SelectedPushpin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if(Map.Children.Contains(SelectedPushpin)) Map.Children.Remove(SelectedPushpin);
            if(_lastSelected != null && !AreOutlinesVisible)
                RhitLocation.HideOutline(_lastSelected);
            _lastSelected = null;

            OnSelectedTap(new PushpinEventArgs() { SelectedPushpin = SelectedPushpin });
        }

        private void Polgon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            RhitLocation location = Select((MapPolygon) sender);
            RhitLocation.ShowOutline(location.OutLine);
            if(_lastSelected != null && !AreOutlinesVisible)
                RhitLocation.HideOutline(_lastSelected);
            _lastSelected = location.OutLine;

            OnTap(new OutlineEventArgs() { Outline = location.OutLine });
        }

        private void Map_MapZoom(object sender, MapZoomEventArgs e) {
            _zoomLevel = Map.ZoomLevel;
            _textLayer.Children.Clear();
            if(!AreLabelsVisible) return;
            foreach(RhitLocation location in Outlines) //TODO: Make more efficient
                if(_zoomLevel > location.MinZoomLevel) _textLayer.Children.Add(location.GetLabel());
        }
        #endregion

        #region Private/Protected Methods
        private void CreateAvailableSources() {
            TileSources = new List<BaseTileSource>() {
                new BaseBingSource() { Name = "Aerial", MapType = BingType.Aerial},
                new BaseBingSource() { Name = "Hybrid", MapType = BingType.Hybrid},
                new BaseBingSource() { Name = "Road", MapType = BingType.Road},
                new BaseGoogleSource() {Name = "Hybrid", MapType = GoogleType.Hybrid},
                new BaseGoogleSource() {Name = "Physical", MapType = GoogleType.Physical},
                new BaseGoogleSource() {Name = "Physical Hybrid", MapType = GoogleType.PhysicalHybrid},
                new BaseGoogleSource() {Name = "Satellite", MapType = GoogleType.Satellite},
                new BaseGoogleSource() {Name = "Street", MapType = GoogleType.Street},
                new Mapnik() {Name = "OSM Mapnik"},
                new OsmaRender() {Name = "Osma Render"},
            };
            OverlaySources = new List<BaseTileSource>() {
                new BaseGoogleSource() {Name = "Google Street Overlays", MapType = GoogleType.StreetOverlay},
                new BaseGoogleSource() {Name = "Google Water Overlays", MapType = GoogleType.WaterOverlay},
                new RoseTileOverlay() { Name = "RHIT Floor Plans"},
            };
        }

        private void Initialize() {
            _mapTileLayer = new MapTileLayer();
            _mapOverlayLayer = new MapTileLayer();
            _overlaySources = new List<BaseTileSource>();
            _locations = new List<RhitLocation>();
            CurrentTileSource = TileSources[7];
            CurrentOverlaySources = new List<BaseTileSource>();
            User = new User();
            _polygonLayer = new MapLayer();
            _textLayer = new MapLayer();
            AreLabelsVisible = false;
            AreOutlinesVisible = false;
            User.Pin.MouseLeftButtonUp += new MouseButtonEventHandler(UserPushpin_MouseLeftButtonUp);
            Map.Children.Add(User.Pin);
            SelectedPushpin = new Pushpin();
            SelectedPushpin.MouseLeftButtonUp += new MouseButtonEventHandler(SelectedPushpin_MouseLeftButtonUp);
            Map.Children.Add(_mapTileLayer);
            Map.Children.Add(_mapOverlayLayer);
            Map.Children.Add(_polygonLayer);
            Map.Children.Add(_textLayer);
            LoadData();
            GoToRhit(); //TODO: Load position from last time (Default here though)
        }

        protected virtual void OnSelectedTap(PushpinEventArgs e) {
            if(PushpinTapped != null) PushpinTapped(this, e);
        }

        protected virtual void OnTap(OutlineEventArgs e) {
            if(OutlineTapped != null) OutlineTapped(this, e);
        }

        protected virtual void OnDebugModeChanged(DebugEventArgs e) {
            if(DebugModeChanged != null) DebugModeChanged(this, e);
        }

        protected virtual void OnDebugTextChanged(DebugEventArgs e) {
            if(DebugTextChanged != null) DebugTextChanged(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add an overlay to the map.
        /// </summary>
        /// <param name="sourceName">Name of one of the available sources</param>
        public void AddOverlay(string sourceName) {
            foreach(BaseTileSource source in OverlaySources)
                if(source.Name == sourceName) {
                    if(CurrentOverlaySources.Contains(source)) return;
                    CurrentOverlaySources.Add(source);
                    _mapOverlayLayer.TileSources.Add(source);
                    return;
                }
        }

        /// <summary>
        /// Switch which tile source the map is using.
        /// </summary>
        /// <param name="sourceType">Name of the base source type.</param>
        /// <param name="sourceName">Name of one of the available sources</param>
        public void ChangeTileSource(string sourceType, string sourceName) {
            if(sourceType == null && sourceName == null) return;
            if(sourceName == null) ChangeTileSource(sourceType);
            if(sourceType == null) ChangeTileSource(sourceName);
            if(sourceType == "Bing") {
                if(CurrentTileSource is BaseBingSource && CurrentTileSource.Name == sourceName) return;
                foreach(BaseTileSource source in TileSources)
                    if(source is BaseBingSource && source.Name == sourceName) {
                        CurrentTileSource = source;
                        DebugText = "Tile Source: Bing " + source.Name;
                        return;
                    }
            } else if(sourceType == "Google") {
                if(CurrentTileSource is BaseGoogleSource && CurrentTileSource.Name == sourceType) return;
                foreach(BaseTileSource source in TileSources)
                    if(source is BaseGoogleSource && source.Name == sourceName) {
                        CurrentTileSource = source;
                        DebugText = "Tile Source: Google " + source.Name;
                        return;
                    }
            } else
                ChangeTileSource(sourceType);
        }

        /// <summary>
        /// Switch which tile source the map is using.
        /// </summary>
        /// <param name="sourceName">Name of one of the available sources</param>
        public void ChangeTileSource(string sourceName) {
            if(CurrentTileSource.Name == sourceName) return;
            foreach(BaseTileSource source in TileSources)
                if(source.Name == sourceName) {
                    CurrentTileSource = source;
                    DebugText = "Tile Source: " + source.Name;
                    return;
                }
            DebugText = "Tile Source Failed: " + sourceName;
        }

        /// <summary>
        /// Makes Rose-Hulman is visible on the map.
        /// </summary>
        public void GoToRhit() {
            Map.Center = App.LOCATION_RHIT.Center;
            ZoomLevel = 16; //TODO: Don't hard code numbers
        }
        
        /// <summary>
        /// Makes the user's current location visible on the map.
        /// </summary>
        public void GoToUserLocation() {
            if(User.Location == null) //TODO: Show Error Message
                return;
            Map.Center = User.Location;
            ZoomLevel = 18; //TODO: Don't hard code numbers
        }

        /// <summary>
        /// Loads data for the map from isolated storage.
        /// </summary>
        public void LoadData() {
            ZoomLevel = (double) DataStorage.LoadState<object>(StorageKey.ZoomLevel, ZoomLevel);
            ChangeTileSource(DataStorage.LoadState<string>(StorageKey.TileSource, CurrentTileSource.Name));
            List<string> sourceNames = DataStorage.LoadState<List<string>>(StorageKey.Overlays, new List<string>());
            foreach(string sourceName in sourceNames) AddOverlay(sourceName);
            AreOutlinesVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleOutlines, AreOutlinesVisible);
            AreLabelsVisible = (bool) DataStorage.LoadState<object>(StorageKey.VisibleLabels, AreLabelsVisible);
            User = DataStorage.LoadState<User>(StorageKey.User, User);
            User.Pin.MouseLeftButtonUp += new MouseButtonEventHandler(UserPushpin_MouseLeftButtonUp);
        }

        /// <summary>
        /// Removes an overlay from the map.
        /// </summary>
        /// <param name="sourceName">Name of one of the available source</param>
        public void RemoveOverlay(string sourceName) {
            foreach(BaseTileSource source in CurrentOverlaySources)
                if(source.Name == sourceName) {
                    CurrentOverlaySources.Remove(source);
                    _mapOverlayLayer.TileSources.Remove(source);
                    return;
                }
        }

        /// <summary>
        /// Makes a location visible on the map.
        /// </summary>
        /// <param name="polygon">Polygon to make visible</param>
        /// <returns>The location that was made visible</returns>
        public RhitLocation Select(MapPolygon polygon) {
            foreach(RhitLocation location in Outlines)
                if(location.IsPolygonEqual(polygon)) {
                    return Select(location);
                }
            return null;
        }

        /// <summary>
        /// Makes a location visible on the map.
        /// </summary>
        /// <param name="location">Location to make visible</param>
        /// <returns>The location that was made visible</returns>
        public RhitLocation Select(RhitLocation location) {
            foreach(RhitLocation _location in Outlines)
                if(_location.Label == location.Label) {
                    if(_lastSelected != null && !AreOutlinesVisible)
                        RhitLocation.HideOutline(_lastSelected);
                    _lastSelected = _location.OutLine;
                    SelectedPushpin.Content = _location.Label + "\n(click for more)";
                    SelectedPushpin.Location = _location.Center;
                    if(!Map.Children.Contains(SelectedPushpin)) Map.Children.Add(SelectedPushpin);
                    RhitLocation.ShowOutline(_location.OutLine);
                    Map.Center = _location.Center;
                    if(ZoomLevel < 18) ZoomLevel = 18;
                    SelectedLocation = _location;
                    return _location;
                }
            return null;
        }

        /// <summary>
        /// Stores map data to isolated storage.
        /// </summary>
        public void StoreData() {
            DataStorage.SaveState(StorageKey.ZoomLevel, ZoomLevel);
            DataStorage.SaveState(StorageKey.TileSource, CurrentTileSource.Name);
            List<string> sourceNames = new List<string>();
            foreach(BaseTileSource source in CurrentOverlaySources)
                sourceNames.Add(source.Name);
            DataStorage.SaveState(StorageKey.Overlays, sourceNames);
            DataStorage.SaveState(StorageKey.VisibleOutlines, AreOutlinesVisible);
            DataStorage.SaveState(StorageKey.VisibleLabels, AreLabelsVisible);
            DataStorage.SaveState(StorageKey.User, User);
            //TODO: Cache Map Tiles
        }
        #endregion
    }
}
