using System;
using Microsoft.Phone.Controls.Maps;

namespace RhitMobile.Events {
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
