using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Extentions.Maps.Modes;
using Rhit.Applications.Extentions.Maps.Sources;

namespace Rhit.Applications.Extentions.Maps {
    public class MapSettings : DependencyObject {
        private MapSettings() {
            Sources = new ObservableCollection<BaseTileSource>();
            Modes = new ObservableCollection<BaseMode>() {
                new EmptyMode(),
                new BingMode(),
                new GoogleMode(),
                new MapnikMode(),
                new OsmaMode(),
            };
            CurrentMode = Modes[2];
        }

        #region Singleton Instance
        private static MapSettings _instance;
        public static MapSettings Instance {
            get {
                if(_instance == null)
                    _instance = new MapSettings();
                return _instance;
            }
        }
        #endregion

        #region CurrentSourceChanged
        public event EventHandler CurrentSourceChanged;
        protected virtual void OnCurrentSourceChanged() {
            if(CurrentSourceChanged != null)
                CurrentSourceChanged(this, new EventArgs());
        }
        #endregion

        private void UpdateSources() {
            List<BaseTileSource> sources = new List<BaseTileSource>();
            foreach(BaseTileSource source in Sources) sources.Add(source);
            foreach(BaseTileSource source in CurrentMode.Sources) Sources.Add(source);
            if(CurrentMode.Sources.Count > 0)
                CurrentSource = CurrentMode.Sources[0];
            else CurrentSource = null;
            foreach(BaseTileSource source in sources) Sources.Remove(source);
            if(Sources.Count > 1) SourceChoices = true;
            else SourceChoices = false;
        }

        #region CurrentMode
        public BaseMode CurrentMode {
            get { return (BaseMode) GetValue(CurrentModeProperty); }
            set { SetValue(CurrentModeProperty, value); }
        }

        public static readonly DependencyProperty CurrentModeProperty =
           DependencyProperty.Register("CurrentMode", typeof(BaseMode), typeof(MapSettings),
           new PropertyMetadata(null, new PropertyChangedCallback(OnModeChanged)));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(e.NewValue != e.OldValue) (d as MapSettings).UpdateSources();
        }
        #endregion

        #region CurrentSource
        public BaseTileSource CurrentSource {
            get { return (BaseTileSource) GetValue(CurrentSourceProperty); }
            set { SetValue(CurrentSourceProperty, value); }
        }

        public static readonly DependencyProperty CurrentSourceProperty =
           DependencyProperty.Register("CurrentSource", typeof(BaseTileSource), typeof(MapSettings),
           new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(e.NewValue != e.OldValue) (d as MapSettings).OnCurrentSourceChanged();
        }
        #endregion

        #region SourceChoices
        public bool SourceChoices {
            get { return (bool) GetValue(SourceChoicesProperty); }
            set { SetValue(SourceChoicesProperty, value); }
        }

        public static readonly DependencyProperty SourceChoicesProperty =
           DependencyProperty.Register("SourceChoices", typeof(bool), typeof(MapSettings), new PropertyMetadata(false));
        #endregion

        public ObservableCollection<BaseMode> Modes { get; set; }

        public ObservableCollection<BaseTileSource> Sources { get; set; }
    }
}
