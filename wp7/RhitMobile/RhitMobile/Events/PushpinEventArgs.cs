using System;
using Microsoft.Phone.Controls.Maps;

namespace RhitMobile.Events {
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
