using System.Device.Location;
using Microsoft.Phone.Controls.Maps;

namespace RhitMobile.ObjectModel {
    /// \ingroup objects
    /// <summary>
    /// Not Implemented.
    /// </summary>
    public class User {
        #region Private Fields
        private GeoCoordinate _location;
        #endregion

        public User() {
            Pin = new Pushpin();
        }

        #region Public Properties
        public GeoCoordinate Location {
            get { return _location; }
            set {
                _location = value;
                Pin.Location = _location;
            }
        }

        public string Name { get; set; }

        public string Password { get; set; }

        public Pushpin Pin { get; private set; }

        public string UserName { get; set; }
        #endregion
    }
}
