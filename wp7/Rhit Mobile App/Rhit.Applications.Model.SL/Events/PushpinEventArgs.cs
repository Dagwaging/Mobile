#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl.Core;
using Microsoft.Maps.MapControl;
#endif

using System;

namespace Rhit.Applications.Model.Events {
    /// <summary>
    /// Event handler that is called when a Pushpin object event is raised.
    /// </summary>
    public delegate void PushpinEventHandler(Object sender, PushpinEventArgs e);

    /// <summary>
    /// Event argument object for Pushpin object events.
    /// </summary>
    public class PushpinEventArgs : EventArgs {
        #region Public Properties
        public Pushpin SelectedPushpin { get; set; }
        #endregion
    }
}
