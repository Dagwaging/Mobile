using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.Models.Services;
using Rhit.Applications.Models.Events;
using Rhit.Applications.Extentions.Maps;
using System.IO;

namespace Rhit.Applications.ViewModels.Controllers {
    public class MapController {
        private MapController() {
            DataCollector.Instance.FolderNamesReceived += new FolderNamesEventHandler(Instance_FolderNamesReceived);
        }

        private const string BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600/download/";

        private void Instance_FolderNamesReceived(object sender, FolderNamesEventArgs e) {
            MapSettings.Instance.GenerateOverlays(BaseAddress, e.FolderNames);
        }

        #region Singleton Instance
        private static MapController _instance;
        public static MapController Instance {
            get {
                if(_instance == null)
                    _instance = new MapController();
                return _instance;
            }
        }
        #endregion

        internal void UpdateOverlays() {
            DataCollector.Instance.GetFolders();
        }

        public void DeleteCurrentOverlay() {
            DataCollector.Instance.DeleteFolder(MapSettings.Instance.CurrentOverlay.Label);
        }

        public void UploadOverlay(FileInfo file) {
            DataCollector.Instance.UploadFile(file);
        }
    }
}