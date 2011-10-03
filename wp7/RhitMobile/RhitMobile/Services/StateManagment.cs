using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;

namespace RhitMobile {
    /// <summary>
    /// Static class data storage.
    /// Utilizes both page settings and isolated storage.
    /// </summary>
    public static class StateManagment {
        #region Private Fields
        private static IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
        #endregion

        /// <summary>
        /// Saves data to page settings and application isolated storage.
        /// </summary>
        /// <param name="page">Page to save the data to (or null)</param>
        /// <param name="key">Key to identify the value</param>
        /// <param name="value">Object to save</param>
        public static void SaveState(this PhoneApplicationPage page, string key, object value) {
            if(userSettings.Contains(key)) userSettings[key] = value;
            else userSettings.Add(key, value);

            if(page == null) return;
            if(page.State.ContainsKey(key)) page.State[key] = value;
            else page.State.Add(key, value);
        }

        /// <summary>
        /// Retrieves data stored in the page settings or isolated storage.
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="page">Page to load the data from (or null)</param>
        /// <param name="key">Key to identify the value</param>
        /// <returns>The object stored with the given key (or default(T))</returns>
        public static T LoadState<T>(this PhoneApplicationPage page, string key) where T : class {
            //TODO: This order will result in never using the second if (using the page variable)
            //Switching the order however causes bugs
            if(userSettings.Contains(key)) return (T) userSettings[key];
            if(page != null && page.State.ContainsKey(key)) return (T) page.State[key];
            return default(T);
        }

        /// <summary>
        /// Retrieves data stored in the page settings or isolated storage.
        /// </summary>
        /// <typeparam name="T">Type of the returned object</typeparam>
        /// <param name="page">Page to load the data from (or null)</param>
        /// <param name="key">Key to identify the value</param>
        /// <param name="defaultValue">Value to be returned if key not found</param>
        /// <returns>The object stored with the given key</returns>
        public static T LoadState<T>(this PhoneApplicationPage page, string key, T defaultValue) where T : class {
            //TODO: This order will result in never using the second if (using the page variable)
            //Switching the order however causes bugs
            if(userSettings.Contains(key)) return (T) userSettings[key];
            if(page != null && page.State.ContainsKey(key)) return (T) page.State[key];
            return defaultValue;
        }
    }
}
