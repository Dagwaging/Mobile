using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Mvvm.Commands;
using Rhit.Applications.ViewModel.Controllers;

namespace Rhit.Applications.ViewModel.Models {
    public class DataViewModel : DependencyObject {
        public DataViewModel(Dispatcher dispatcher) {
            InitializeProperties();

            Locations = LocationsController.Instance;
            Locations.LocationsChanged += new LocationEventHandler(LocationsChanged);
            if(Locations.All.Count > 0 && Locations.CurrentLocation == null)
                Locations.SelectLocation(Locations.All[2].Id);

            DataCollector.Instance.StoredProcReturned += new StoredProcEventHandler(StoredProcReturned);
        }

        private void InitializeProperties() {
            ZoomRange = new ObservableCollection<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };

            AddNameCommand = new RelayCommand(p => AddAltName());
            AddLinkCommand = new RelayCommand(p => AddLink());
            DeleteCommand = new RelayCommand(p => DeleteLocation());
            SaveCommand = new RelayCommand(p => SaveLocation());
        }

        private void LocationsChanged(object sender, LocationEventArgs e) {
            if(Locations.All.Count > 0 && Locations.CurrentLocation == null)
                Locations.SelectLocation(Locations.All[2].Id);
        }

        private void StoredProcReturned(object sender, StoredProcEventArgs e) {
            //TODO: Actually update location in Tree that was just saved
            //It can take up to three calls to this method before all data is updated in database
        }

        private void AddAltName() {
            Locations.CurrentLocation.AltNames.Add(new AlternateName("Default Name"));
        }

        private void AddLink() {
            Locations.CurrentLocation.Links.Add(new Link() { Name = "Default", Address = "http://sample.com" });
        }

        private void SaveLocation() {
            if(Locations.CurrentLocation == null) return;
            ObservableRhitLocation newLocation = Locations.CurrentLocation;
            if(!newLocation.HasChanged) return;
            RhitLocation oldLocation = newLocation.OriginalLocation;

            List<string> changes = newLocation.CheckChanges();
            if(changes.Contains("Center")) changes.Remove("Center");
            if(changes.Contains("Links")) {
                var executions = new List<KeyValuePair<string, Dictionary<string, object>>>() {
                    new KeyValuePair<string,Dictionary<string,object>>("spDeleteLinks", new Dictionary<string,object>() {
                        { "location", oldLocation.Id }
                    })
                };
                foreach(Link link in newLocation.Links) {
                    executions.Add(new KeyValuePair<string, Dictionary<string, object>>("spAddLink", new Dictionary<string, object>() {
                        { "location", oldLocation.Id },
                        { "name", link.Name },
                        { "url", link.Address }
                    }));
                }
                DataCollector.Instance.ExecuteBatchStoredProcedure(Dispatcher, executions);

                changes.Remove("Links");
            }

            if(changes.Contains("AltNames")) {
                //TODO: Scott - Change AltNames
                //Use oldLocation.Id since upodating location id is last


                changes.Remove("AltNames");
            }

            if(changes.Count <= 0) return; //No changes left
            if(string.IsNullOrEmpty(newLocation.Label)) return; // Name error
            if(newLocation.Id <= 0) return; // Id error
            if(newLocation.ParentId < 0) return; // Parent Id error

            // Valid parameters
            DataCollector.Instance.ExecuteStoredProcedure(Dispatcher, "spUpdateLocation", new Dictionary<string, object>() {
                { "id", oldLocation.Id },
                { "name", newLocation.Label },
                { "newid", newLocation.Id },
                { "parent", newLocation.ParentId },
                { "description", newLocation.Description },
                { "labelonhybrid", newLocation.LabelOnHybrid },
                { "minzoomlevel", newLocation.MinZoom },
                { "type", Location_DC.ConvertTypeToTypeKey(newLocation.Type) },
            });
        }

        private void DeleteLocation() {
            if(Locations.CurrentLocation == null) return;
            DataCollector.Instance.ExecuteStoredProcedure(Dispatcher, "spDeleteLocation", new Dictionary<string, object>() {
                { "location", Locations.CurrentLocation.Id },
            });
        }

        #region Commands
        public ICommand AddNameCommand { get; private set; }

        public ICommand AddLinkCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }
        #endregion

        public LocationsController Locations { get; private set; }

        public ObservableCollection<int> ZoomRange { get; private set; }

        public void SelectLocation(object locationNode) {
            try { Locations.SelectLocation((LocationNode) locationNode); } catch { }
        }
    }
}