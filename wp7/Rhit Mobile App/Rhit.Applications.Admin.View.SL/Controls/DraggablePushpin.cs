using System;
using System.Windows.Input;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Core;

namespace Rhit.Applications.View.Controls {
    public class DraggablePushpin : Pushpin {
        private bool isDragging = false;
        EventHandler<MapMouseDragEventArgs> ParentMapMousePanHandler;
        MouseButtonEventHandler ParentMapMouseLeftButtonUpHandler;
        MouseEventHandler ParentMapMouseMoveHandler;

        private void AddEventHandlers() {
            // Check to see if this is the event handlers have been attached yet and if not, attach them.
            // This will only happen on the first click of the Pushpin.
            MapLayer parentLayer = Parent as MapLayer;
            if(parentLayer != null) {
                MapBase parentMap = parentLayer.ParentMap;
                if(parentMap != null) {
                    if(ParentMapMousePanHandler == null) {
                        ParentMapMousePanHandler = new EventHandler<MapMouseDragEventArgs>(ParentMap_MousePan);
                        parentMap.MousePan += ParentMapMousePanHandler;
                    }
                    if(ParentMapMouseLeftButtonUpHandler == null) {
                        ParentMapMouseLeftButtonUpHandler = new MouseButtonEventHandler(ParentMap_MouseLeftButtonUp);
                        parentMap.MouseLeftButtonUp += ParentMapMouseLeftButtonUpHandler;
                    }
                    if(ParentMapMouseMoveHandler == null) {
                        ParentMapMouseMoveHandler = new MouseEventHandler(ParentMap_MouseMove);
                        parentMap.MouseMove += ParentMapMouseMoveHandler;
                    }
                }
            }
        }

        // Event Handler for Pushpin
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            // I do this initialization here because it is the first point
            //   where I know the pushpin is added to the map.
            AddEventHandlers();

            // Enable Dragging
            isDragging = true;

            base.OnMouseLeftButtonDown(e);
        }

        #region "Mouse Event Handler Methods"

        void ParentMap_MousePan(object sender, MapMouseDragEventArgs e) {
            // If the Pushpin is being dragged, then suppress the
            //  other event handlers from firing
            if(isDragging) e.Handled = true;
        }

        void ParentMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            // Left Mouse Button released, stop dragging the Pushpin
            isDragging = false;
        }

        void ParentMap_MouseMove(object sender, MouseEventArgs e) {
            Map map = sender as Microsoft.Maps.MapControl.Map;
            // Check if the user is currently dragging the Pushpin
            // If so, then move the Pushpin to where the mouse is.
            if(isDragging) {
                Location = map.ViewportPointToLocation(e.GetPosition(map));
            }
        }

        #endregion
    }
}