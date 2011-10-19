using System;

namespace RhitMobile.Events {
    /// <summary>
    /// Event handler that is called when a text-based event is raised.
    /// </summary>
    public delegate void DebugEventHandler(Object sender, DebugEventArgs e);

    /// <summary>
    /// Event argument object for text-based events.
    /// </summary>
    public class DebugEventArgs : EventArgs { }
}