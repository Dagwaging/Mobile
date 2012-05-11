using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Navigation;
using Microsoft.Maps.MapControl.Overlays;
using Rhit.Applications.Extentions.Controls;
using Rhit.Applications.Extentions.Maps;
using Rhit.Applications.ViewModels.Controllers;
using Rhit.Applications.Views.Utilities;

namespace Rhit.Applications.Views.Views {
    public partial class NodesMapPage : Page {
        public NodesMapPage() {
            InitializeComponent();

            //TODO: Try not to have to do this
            //ViewModel.SetMode(MyMap);

            DraggablePushpin.ParentMap = NodeMap;

            NodeMap.MouseClick += ViewLocations.Map_Click;

            NodeMap.MouseClick += Calibrator.Map_Click;

            MapExtender.Attach(NodeMap);
            NodeMap.MouseClick += new EventHandler<MapMouseEventArgs>(Map_MouseClick);
            NodeMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);

            DataContext = ViewModel;
            //TaskModes.CurrentTaskMode = PathTasks;
        }

        #region Click Event Methods/Properties
        private Point LastEventCoordinate { get; set; }

        void Map_MouseClick(object sender, MapMouseEventArgs e) {
            if(LastEventCoordinate == e.ViewportPoint) return;
            if(!Calibrator.Calibrating && !ViewLocations.Working) ViewModel.Locations.UnSelect();
        }
        #endregion

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

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) {

            //TaskModes.CurrentTaskMode = BuildingTasks;

            if(!LoginController.Instance.IsLoggedIn) {
                NavigationService.Navigate(new Uri("/LoginPage", UriKind.Relative));
            }

        }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            MapLayout.Children.Remove(NodeMap);
            NodeMap.Children.Clear();
        }
        #endregion
    }
}
