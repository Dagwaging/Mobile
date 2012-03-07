using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Rhit.Applications.Models.Services {
    public class DataStorage {

        #region Private Fields
        private static DataStorage _instance;
        private static IsolatedStorageSettings _userSettings = IsolatedStorageSettings.ApplicationSettings;
        #endregion

        private DataStorage() {
            AllLocations = DataStorage.LoadState<Dictionary<int, LocationData>>(StorageKey.All, new Dictionary<int, LocationData>());
            TopLocations = DataStorage.LoadState<Dictionary<int, LocationData>>(StorageKey.Top, new Dictionary<int, LocationData>());
        }

        public static DataStorage Instance {
            get {
                if(_instance == null)
                    _instance = new DataStorage();
                return _instance;
            }
        }

        public static double Version {
            get {
                return (double) DataStorage.LoadState<object>(StorageKey.Version, 0);
            }
            set {
                DataStorage.SaveState(StorageKey.Version, value);
            }
        }

        public static bool IsAllFull {
            get {
                return (bool) DataStorage.LoadState<object>(StorageKey.IsAllFull, false);
            }
            private set {
                DataStorage.SaveState(StorageKey.IsAllFull, value);
            }
        }

        public Dictionary<int, LocationData> AllLocations { get; private set; }

        public Dictionary<int, LocationData> TopLocations { get; private set; }

        public void OverwriteData(List<LocationData> locations, StorageKey key) {
            if(key == StorageKey.Top) {
                TopLocations = MakeDict(locations);
                MergeData(locations, StorageKey.All);
                DataStorage.SaveState(StorageKey.Top, TopLocations);
                DataStorage.RemoveState(StorageKey.All);
                IsAllFull = false;
            } else if(key == StorageKey.All) {
                AllLocations = MakeDict(locations);
                DataStorage.SaveState(StorageKey.All, AllLocations);
                IsAllFull = true;
            }
        }

        public void MergeData(List<LocationData> locations, StorageKey key) {
            if(locations == null) return;
            if(key == StorageKey.Top) {
                TopLocations = MakeDict(locations);
                DataStorage.SaveState(StorageKey.Top, TopLocations);
            }
            foreach(LocationData location in locations) {
                if(AllLocations.ContainsKey(location.Id))
                    AllLocations[location.Id].Overwrite(location);
                else AllLocations[location.Id] = location;
            }
            DataStorage.SaveState(StorageKey.All, AllLocations);
        }

        private static Dictionary<int, LocationData> MakeDict(List<LocationData> locations) {
            Dictionary<int, LocationData> dict = new Dictionary<int, LocationData>();
            if(locations == null) return dict;
            foreach(LocationData location in locations)
                dict[location.Id] = location;
            return dict;
        }


        public static void RemoveState(StorageKey key) {
            if(_userSettings.Contains(key.ToString()))
                _userSettings.Remove(key.ToString());
        }

        public static void SaveState(StorageKey key, object value) {
            _userSettings[key.ToString()] = value;
        }

        public static T LoadState<T>(StorageKey key) where T : class {
            if(_userSettings.Contains(key.ToString()))
                return (T) _userSettings[key.ToString()];
            return default(T);
        }

        public static T LoadState<T>(StorageKey key, T defaultValue) where T : class {
            if(_userSettings.Contains(key.ToString())) {
                var o = _userSettings[key.ToString()];
                return (T) o;
            }
            return defaultValue;
        }
    }
}
