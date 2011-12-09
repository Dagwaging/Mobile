using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Rhit.Applications.Model.Maps.Modes;
using Rhit.Applications.Model.Maps.Sources;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Navigation;

namespace Rhit.Applications.ViewModel.Models {
    public class MapViewModel {
        private string _mapId;

        #region Constructor and Initializers
        public MapViewModel(string mapId) {
            InitializeResources();

            _mapId = mapId;

            InitializeMap();
        }

        private void InitializeResources() {
            Modes = new List<RhitMode>() {
                new EmptyMode(),
                new BingMode(),
                new GoogleMode(),
                new MapnikMode(),
                new OsmaMode(),
            };

            Overlays = new List<BaseTileSource>() {
                new RoseOverlay(),
            };

            CurrentMode = Modes[1];
            CurrentOverlays = new List<BaseTileSource>();

            TileLayer = new MapTileLayer();
            TileLayer.TileSources.Add(CurrentMode.CurrentSource);

            OverlayLayer = new MapTileLayer();
            foreach(BaseTileSource source in CurrentOverlays)
                OverlayLayer.TileSources.Add(source);
        }

        private void InitializeMap() {
            RhitMap = new Map() {
                CredentialsProvider = new ApplicationIdCredentialsProvider(_mapId),
                Mode = CurrentMode,
                CopyrightVisibility = Visibility.Collapsed,
                LogoVisibility = Visibility.Collapsed,
                NavigationVisibility = Visibility.Visible,
            };

            RhitMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);
            RhitMap.MouseClick += new EventHandler<MapMouseEventArgs>(RhitMap_MouseClick);

            RhitMap.Children.Clear();
            RhitMap.Children.Add(TileLayer);
            RhitMap.Children.Add(OverlayLayer);
        }
        #endregion

        #region Event Handlers
        void RhitMap_MouseClick(object sender, MapMouseEventArgs e) {
            Location location = RhitMap.ViewportPointToLocation(e.ViewportPoint);
            if(ClickedPin == null) return;
            ClickedPin.Location = location;
            if(!RhitMap.Children.Contains(ClickedPin))
                RhitMap.Children.Add(ClickedPin);
        }

        void MapForeground_TemplateApplied(object sender, EventArgs e) {
            RhitMap.MapForeground.NavigationBar.TemplateApplied += new EventHandler(NavigationBar_TemplateApplied);
        }

        void NavigationBar_TemplateApplied(object sender, EventArgs e) {
            UpdateNaviBar();
        }

        private void Mode_Changed(object sender, RoutedEventArgs e) {
            string name = ((sender as CommandRadioButton).Content as TextBlock).Text;
            ChangeMode(name);
            UpdateNaviBar();
        }

        private void Source_Changed(object sender, RoutedEventArgs e) {
            string name = ((sender as CommandRadioButton).Content as TextBlock).Text;
            ChangeSource(name);
        }

        void Overlay_Checked(object sender, RoutedEventArgs e) {
            string name = ((sender as CommandToggleButton).Content as TextBlock).Text;
            AddOverlay(name);
        }

        void Overlay_Unchecked(object sender, RoutedEventArgs e) {
            string name = ((sender as CommandToggleButton).Content as TextBlock).Text;
            RemoveOverlay(name);
        }
        #endregion

        #region Private Properties
        

        private MapTileLayer OverlayLayer { get; set; }

        private MapTileLayer TileLayer { get; set; }
        #endregion

        #region Public Properties
        public Map RhitMap { get; private set; }

        public RhitMode CurrentMode { get; private set; }

        public List<BaseTileSource> Overlays { get; private set; }

        public List<BaseTileSource> CurrentOverlays { get; private set; }

        public List<RhitMode> Modes { get; private set; }

        public Pushpin ClickedPin { get; set; }
        #endregion

        #region Methods
        public void AddOverlay(string name) {
            foreach(BaseTileSource overlay in Overlays) {
                if(overlay.Label == name && !CurrentOverlays.Contains(overlay)) {
                    AddOverlay(overlay);
                    return;
                }
            }
        }

        private void AddOverlay(BaseTileSource source) {
            CurrentOverlays.Add(source);
            OverlayLayer.TileSources.Add(source);
        }

        public void RemoveOverlay(string name) {
            foreach(BaseTileSource overlay in Overlays) {
                if(overlay.Label == name && !CurrentOverlays.Contains(overlay)) {
                    RemoveOverlay(overlay);
                    return;
                }
            }
        }

        private void RemoveOverlay(BaseTileSource source) {
            CurrentOverlays.Remove(source);
            OverlayLayer.TileSources.Remove(source);
        }

        public void ChangeSource(string name) {
            BaseTileSource source = CurrentMode.ChangeSource(name);
            if(source == null) return;
            TileLayer.TileSources.Clear();
            TileLayer.TileSources.Add(source);
        }

        public void ChangeMode(string name) {
            foreach(RhitMode mode in Modes)
                if(mode.Label == name && CurrentMode != mode) {
                    CurrentMode = mode;
                    TileLayer.TileSources.Clear();
                    if(CurrentMode.CurrentSource != null)
                        TileLayer.TileSources.Add(CurrentMode.CurrentSource);
                    return;
                }
        }

        public void GotoRhit() {
            RhitMap.Center = new Location(39.4820263, -87.3248677);
            RhitMap.ZoomLevel = 16;
        }
        #endregion

        #region Navigation Bar Update Methods
        private void UpdateNaviBar() {
            NavigationBar navControl = RhitMap.MapForeground.NavigationBar;
            UIElementCollection children = navControl.HorizontalPanel.Children;

            children.Clear();

            AddModesToNaviBar(children);
            AddSourcesToNaviBar(children);
            AddOverlaysToNaviBar(children);
        }

        private void AddModesToNaviBar(UIElementCollection naviBar) {
            
            foreach(RhitMode mode in Modes) {
                bool isChecked = false;
                if(CurrentMode == mode) isChecked = true;
                CommandRadioButton button = new CommandRadioButton() {
                    Content = new TextBlock() { Text = mode.Label, },
                    GroupName = "Modes",
                    IsChecked = isChecked,
                };
                button.Checked += new RoutedEventHandler(Mode_Changed);
                naviBar.Add(button);
            }
        }

        private void AddSourcesToNaviBar(UIElementCollection naviBar) {
            BaseTileSource currentSource = CurrentMode.CurrentSource;
            if(CurrentMode.Sources.Count > 1) {
                naviBar.Add(new CommandSeparator());
                foreach(BaseTileSource source in CurrentMode.Sources) {
                    bool isChecked = false;
                    if(currentSource == source) isChecked = true;
                    CommandRadioButton button = new CommandRadioButton() {
                        Content = new TextBlock() { Text = source.Label, },
                        GroupName = "Sources",
                        IsChecked = isChecked,
                    };
                    naviBar.Add(button);
                    button.Checked += new RoutedEventHandler(Source_Changed);
                }
            }
        }

        private void AddOverlaysToNaviBar(UIElementCollection naviBar) {
            List<BaseTileSource> overlays = Overlays;
            List<BaseTileSource> currentOverlays = CurrentOverlays;
            if(overlays.Count > 0) {
                naviBar.Add(new CommandSeparator());
                foreach(BaseTileSource overlay in overlays) {
                    bool isChecked = false;
                    if(currentOverlays.Contains(overlay)) isChecked = true;
                    CommandToggleButton button = new CommandToggleButton() {
                        Content = new TextBlock() { Text = overlay.Label, },
                        IsChecked = isChecked,
                    };
                    naviBar.Add(button);
                    button.Checked += new RoutedEventHandler(Overlay_Checked);
                    button.Unchecked += new RoutedEventHandler(Overlay_Unchecked);
                }
            }
        }
        #endregion
    }
}
