using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace Rhit.Applications.Extentions.Controls {
    public class DraggablePushpin : Pushpin {
        private bool isDragging = false;
        private EventHandler<MapMouseDragEventArgs> ParentMapMousePanHandler;
        private MouseButtonEventHandler ParentMapMouseLeftButtonUpHandler;
        private MouseEventHandler ParentMapMouseMoveHandler;

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

        public static MapBase ParentMap { get; set; }

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

        #region Event Handlers
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            // I do this initialization here because it is the first point
            //   where I know the pushpin is added to the map.
            AddEventHandlers();

            // Enable Dragging
            isDragging = true;

            base.OnMouseLeftButtonDown(e);
        }

        private void ParentMap_MousePan(object sender, MapMouseDragEventArgs e) {
            // If the Pushpin is being dragged, then suppress the
            //  other event handlers from firing
            if(isDragging) e.Handled = true;
        }

        private void ParentMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if(isDragging) VirtualLocation = Location;
            isDragging = false;
        }

        private void ParentMap_MouseMove(object sender, MouseEventArgs e) {
            Map map = sender as Microsoft.Maps.MapControl.Map;
            MapLayer parentLayer = FindVisualParent<MapLayer>(this);

            if(isDragging) {
                Point mouseMapPosition = e.GetPosition(map);
                Location mouseGeocode = map.ViewportPointToLocation(mouseMapPosition);
                Location = mouseGeocode;
                parentLayer.InvalidateArrange();
            }
        }
        #endregion

        #region VirtualLocation
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
    }
}