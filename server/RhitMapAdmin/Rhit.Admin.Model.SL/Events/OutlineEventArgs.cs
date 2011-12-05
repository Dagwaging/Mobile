using System;
using Microsoft.Maps.MapControl;

namespace Rhit.Admin.Model.Events {
    /// <summary>
    /// Event handler that is called when a MapPolygon object (Outline) event is raised.
    /// </summary>
    public delegate void OutlineEventHandler(Object sender, OutlineEventArgs e);

    /// <summary>
    /// Event argument object for MapPolgon object (Outline) events.
    /// </summary>
    public class OutlineEventArgs : EventArgs {
        #region Public Properties
        public MapPolygon Outline { get; set; }
        #endregion
    }
}
