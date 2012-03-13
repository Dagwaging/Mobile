#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl.Core;
using Microsoft.Maps.MapControl;
#endif

using System;

namespace Rhit.Applications.Model.Events {
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
