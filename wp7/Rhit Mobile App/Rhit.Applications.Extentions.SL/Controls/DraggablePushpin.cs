using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace Rhit.Applications.Extentions.Controls {
    public class DraggablePushpin : Pushpin {
        public DraggablePushpin() : base() {
            IsDragging = false;
        }

        public static MapBase ParentMap { get; set; }

        protected bool IsDragging { get; set; }

        public static T FindVisualParent<T>(DependencyObject obj) where T : DependencyObject {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while(parent != null) {
                T typed = parent as T;
                if(typed != null) {
                    return typed;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private void AddEventHandlers() {
            // Check to see if this is the event handlers have been attached yet and if not, attach them.
            // This will only happen on the first click of the Pushpin.
            if(ParentMap == null) {
                MapLayer parentLayer = Parent as MapLayer;
                if(parentLayer == null) return;
                ParentMap = parentLayer.ParentMap;
            }

            if(ParentMapMousePanHandler == null) {
                ParentMapMousePanHandler = new EventHandler<MapMouseDragEventArgs>(ParentMap_MousePan);
                ParentMap.MousePan += ParentMapMousePanHandler;
            }
            if(ParentMapMouseLeftButtonUpHandler == null) {
                ParentMapMouseLeftButtonUpHandler = new MouseButtonEventHandler(ParentMap_MouseLeftButtonUp);
                ParentMap.MouseLeftButtonUp += ParentMapMouseLeftButtonUpHandler;
            }
            if(ParentMapMouseMoveHandler == null) {
                ParentMapMouseMoveHandler = new MouseEventHandler(ParentMap_MouseMove);
                ParentMap.MouseMove += ParentMapMouseMoveHandler;
            }

        }

        #region Event Handlers
        protected EventHandler<MapMouseDragEventArgs> ParentMapMousePanHandler { get; set; }

        protected MouseButtonEventHandler ParentMapMouseLeftButtonUpHandler { get; set; }

        protected MouseEventHandler ParentMapMouseMoveHandler { get; set; }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if(!CanChange) return;
            AddEventHandlers();
            IsDragging = true;
            base.OnMouseLeftButtonDown(e);
        }

        private void ParentMap_MousePan(object sender, MapMouseDragEventArgs e) {
            if(!CanChange) return;
            if(IsDragging) e.Handled = true;
        }

        private void ParentMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if(IsDragging) VirtualLocation = Location;
            IsDragging = false;
        }

        private void ParentMap_MouseMove(object sender, MouseEventArgs e) {
            if(!CanChange) return;
            Map map = sender as Microsoft.Maps.MapControl.Map;
            MapLayer parentLayer = FindVisualParent<MapLayer>(this);

            if(IsDragging) {
                Point mouseMapPosition = e.GetPosition(map);
                Location mouseGeocode = map.ViewportPointToLocation(mouseMapPosition);
                Location = mouseGeocode;
                parentLayer.InvalidateArrange();
            }
        }
        #endregion

        #region public Location VirtualLocation
        public Location VirtualLocation {
            get { return (Location) GetValue(VirtualLocationProperty); }
            set { SetValue(VirtualLocationProperty, value); }
        }

        public static readonly DependencyProperty VirtualLocationProperty =
           DependencyProperty.Register("VirtualLocation", typeof(Location), typeof(DraggablePushpin), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnVirtualLocationChanged)));

        private static void OnVirtualLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            DraggablePushpin instance = (DraggablePushpin) d;
            instance.Location = instance.VirtualLocation;
        }
        #endregion

        #region public bool CanChange
        public bool CanChange {
            get { return (bool) GetValue(CanChangeProperty); }
            set { SetValue(CanChangeProperty, value); }
        }

        public static readonly DependencyProperty CanChangeProperty =
           DependencyProperty.Register("CanChange", typeof(bool), typeof(DraggablePushpin), new PropertyMetadata(true));
        #endregion
    }
}