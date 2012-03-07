using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Rhit.Applications.ViewModels;
using Rhit.Applications.ViewModels.Controllers;
using System.Windows.Media;

namespace Rhit.Applications.Views {
    public partial class App : Application {
        private bool phoneApplicationInitialized = false;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App() {
            //Set the base address of the service
            Services.Start(Deployment.Current.Dispatcher);

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            InitializeComponent();
            InitializePhoneApplication();
        }

        #region Properties
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }
        #endregion

        #region Initialization Methods
        /// <summary> Initialize Application. </summary>
        private void InitializePhoneApplication() {
            if(phoneApplicationInitialized) return;
            // Create the frame but don't set it as RootVisual yet.
            // This allows the splash screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        /// <summary> Finialize initialization. </summary>
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e) {
            // Set the root visual to allow the application to render
            if(RootVisual != RootFrame) RootVisual = RootFrame;
            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }
        #endregion

        #region Application Level Exception Handlers
        /// <summary> Execute if a navigation fails. </summary>
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
            if(System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        /// <summary> Execute on Unhandled Exceptions. </summary>
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
            if(System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
        #endregion

        #region Application Level OnStart and OnStop Event Handlers
        /// <summary>
        /// Code to be executed when the application is launching.
        /// This will not execute when the application is reactivated.
        /// </summary>
        private void Application_Launching(object sender, LaunchingEventArgs e) {
            
        }

        /// <summary>
        /// Code to execute when the application is activated.
        /// This code will not execute when the application is first launched.
        /// </summary>
        private void Application_Activated(object sender, ActivatedEventArgs e) {
            
        }

        /// <summary>
        /// Code to execute when the application is deactivated (sent to background).
        /// This code will not execute when the application is closing.
        /// </summary>
        private void Application_Deactivated(object sender, DeactivatedEventArgs e) {
            //TODO: Save to Isolated Storage
        }

        /// <summary>
        /// Code to execute when the application is closing (eg, user hit Back).
        /// This code will not execute when the application is deactivated.
        /// </summary>
        private void Application_Closing(object sender, ClosingEventArgs e) {
            //TODO: Save to Isolated Storage
        }
        #endregion


    }
}