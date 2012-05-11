using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Navigation;
using Microsoft.Maps.MapControl.Overlays;
using Rhit.Applications.Extentions.Maps;

namespace Rhit.Applications.Views.Views {
    public partial class GpsMapPage : Page {
        public GpsMapPage() {
            InitializeComponent();

            MapExtender.Attach(MyMap);
            MyMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);

            DataContext = this;
        }

        #region NavigationBar Initialization Methods
        void MapForeground_TemplateApplied(object sender, EventArgs e) {
            if(!(sender is MapForeground)) return;
            (sender as MapForeground).NavigationBar.TemplateApplied += new EventHandler(NavigationBar_TemplateApplied);
        }

        void NavigationBar_TemplateApplied(object sender, EventArgs e) {
            if(!(sender is NavigationBar)) return;
            UpdateNaviBar(sender as NavigationBar);
        }

        private void UpdateNaviBar(NavigationBar naviBar) {
            UIElementCollection children = naviBar.HorizontalPanel.Children;
            children.Clear();

            ListBox lb = new ListBox();
            lb.DataContext = new MapExtender();
            lb.SetBinding(ListBox.ItemsSourceProperty, new System.Windows.Data.Binding("Settings.Modes"));
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Settings.CurrentMode");
            binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            lb.SetBinding(ListBox.SelectedValueProperty, binding);
            lb.Style = (Style) App.Current.Resources["MapNavigationBarStyle"];
            children.Add(lb);

            children.Add(new CommandSeparator());

            lb = new ListBox();
            lb.DataContext = new MapExtender();
            lb.SetBinding(ListBox.ItemsSourceProperty, new System.Windows.Data.Binding("Settings.Sources"));
            binding = new System.Windows.Data.Binding("Settings.CurrentSource");
            binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            lb.SetBinding(ListBox.SelectedValueProperty, binding);
            lb.Style = (Style) App.Current.Resources["MapNavigationBarStyle"];
            children.Add(lb);

        }
        #endregion

        #region public Location Coordinate
        public Location Coordinate {
            get { return (Location) GetValue(CoordinateProperty); }
            set { SetValue(CoordinateProperty, value); }
        }

        public static readonly DependencyProperty CoordinateProperty =
           DependencyProperty.Register("Coordinate", typeof(Location), typeof(GpsMapPage), new PropertyMetadata(new Location()));
        #endregion

        private void MyMap_MouseClick(object sender, MapMouseEventArgs e) {
            Coordinate = MyMap.ViewportPointToLocation(e.ViewportPoint);
        }

    }
}
