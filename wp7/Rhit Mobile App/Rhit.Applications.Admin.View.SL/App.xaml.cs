﻿using System;
using System.Windows;
using Rhit.Applications.ViewModel;
using Rhit.Applications.View.Views;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.View {
    public partial class App : Application {
        internal const string MapId = "AthZ1tu5ROM0PUWcIYFSxC1oQALFR-g0aoFIuL9tlbeGJ9Z6qKIRYoB_jGpct8Yu";

        public App() {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
            Services.Start(Deployment.Current.Dispatcher);
            //Initialize Locations by just creating the instance
            var locations = LocationsController.Instance;
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            this.RootVisual = new MainPage();
        }

        private void Application_Exit(object sender, EventArgs e) { }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if(!System.Diagnostics.Debugger.IsAttached) {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e) {
            try {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            } catch(Exception) { }
        }
    }
}
