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
using Rhit.Applications.Views.Utilities;

namespace Rhit.Applications.Views {
    public partial class MapPage : Page {
        public MapPage() {
            InitializeComponent();

            //TODO: Try not to have to do this
            //ViewModel.SetMode(MyMap);

            //TODO: Shouldn't have to do this
            DraggablePushpin.ParentMap = MyMap;
            DraggableShape.ParentContainer = MyCanvas;

            MyMap.MouseClick += ViewLocations.Map_Click;

            MyMap.MouseClick += Calibrator.Map_Click;
            MyCanvas.MouseLeftButtonUp += Calibrator.ImageViewer_Click;

            MyCanvas.MouseLeftButtonUp += ViewLocations.ImageViewer_Click;
            RhitMapExtender.Attach(MyMap);
            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(Map_MouseClick);
            MyMap.MapForeground.TemplateApplied += new EventHandler(MapForeground_TemplateApplied);

            DataContext = ViewModel;
            TaskModes.CurrentTaskMode = PathTasks;
        }

        #region Click Event Methods/Properties
        private Point LastEventCoordinate { get; set; }

        void Map_MouseClick(object sender, MapMouseEventArgs e) {
            if(LastEventCoordinate == e.ViewportPoint) return;
            if(!Calibrator.Calibrating && !ViewLocations.Working) ViewModel.Locations.UnSelect();
        }

        private void MapPolygon_Click(object sender, MouseButtonEventArgs e) {
            if(Calibrator.Calibrating) return;
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as MapPolygon).Tag);
        }

        private void Pushpin_Click(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as Pushpin).Tag);
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
            lb.DataContext = new RhitMapExtender();
            lb.SetBinding(ListBox.ItemsSourceProperty, new System.Windows.Data.Binding("Settings.Modes"));
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Settings.CurrentMode");
            binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            lb.SetBinding(ListBox.SelectedValueProperty, binding);
            lb.Style = (Style) this.Resources["NavigationBarList"];
            children.Add(lb);

            children.Add(new CommandSeparator());

            lb = new ListBox();
            lb.DataContext = new RhitMapExtender();
            lb.SetBinding(ListBox.ItemsSourceProperty, new System.Windows.Data.Binding("Settings.Sources"));
            binding = new System.Windows.Data.Binding("Settings.CurrentSource");
            binding.Mode = System.Windows.Data.BindingMode.TwoWay;
            lb.SetBinding(ListBox.SelectedValueProperty, binding);
            lb.Style = (Style) this.Resources["NavigationBarList"];
            children.Add(lb);

        }
        #endregion

        #region Page Navigation
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            //TaskModes.CurrentTaskMode = BuildingTasks;
        }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            MapLayout.Children.Remove(MyMap);
            MyMap.Children.Clear();
        }
        #endregion

        private void DraggableShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as DraggableShape).Tag);
        }

        private void DraggablePushpin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            LastEventCoordinate = e.GetPosition(MyMap);
            ViewModel.SelectLocation((int) (sender as Pushpin).Tag);
        }

        private void TaskModeRadioButton_Checked(object sender, RoutedEventArgs e) {
            var s = (sender as RadioButton).Content;
            var tmp = new TaskContainer();
            var tasks = new List<Task>() {
                new Task() { Label="Task1" },
            };
            tmp.AddTasks(tasks);
            TaskModes.CurrentTaskMode = tmp;
        }
    }
}
