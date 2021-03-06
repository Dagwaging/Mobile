﻿using System;
using System.Net;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.Model.Events {
    /// <summary>
    /// Event handler that is called when a server-based event is raised.
    /// </summary>
    public delegate void ServerEventHandler(Object sender, ServerEventArgs e);

    /// <summary>
    /// Event argument object for server-based events.
    /// </summary>
    public class ServerEventArgs : ServiceEventArgs {
        #region Public Properties
        /// <summary>
        /// Http response model object for the corresponding request.
        /// </summary>
        public ServerObject ResponseObject { get; set; }

        /// <summary>
        /// Http status object for the corresponding request.
        /// </summary>
        public HttpStatusCode ServerResponse { get; set; }

        public ResponseType Type { get; set; }

        public string RawResponse { get; set; }
        #endregion
    }
}