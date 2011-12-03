using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl;

namespace Rhit.Admin.Model {
    /// \ingroup objects
    /// <summary>
    /// Represents an area, building, or room at Rose-Hulman.
    /// </summary>
    public class RhitLocation {
        #region Private Fields
        private LocationCollection _locations;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor; Basic initialization; Make sure to set relevant properties.
        /// </summary>
        public RhitLocation() { Initialize(); }

        /// <summary>
        /// Constructor; Initializes 'Center' property.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public RhitLocation(double latitude, double longitude) {
            Center = new Location(latitude, longitude);
            Initialize();
        }

        /// <summary>
        /// Constructor; Initializes 'Center' property.
        /// </summary>
        /// <param name="latitude">Center latitude of the location</param>
        /// <param name="longitude">Center longitude of the location</param>
        /// <param name="altitude">Center altitude of the location</param>
        public RhitLocation(double latitude, double longitude, double altitude) {
            Center = new Location(latitude, longitude, altitude);
            Initialize();
        }
        #endregion

        #region Public Properties
        /// <summary> Center of the location. </summary>
        public Location Center { get; set; }

        /// <summary> Description of the location. </summary>
        public string Description { get; set; }

        /// <summary> Id of the location (Used only by the service). </summary>
        public int Id { get; set; }

        public bool IsPOI { get; set; }

        /// <summary> Name of the location. </summary>
        public string Label { get; set; }

        /// <summary> Does the name already appear on hybrid maps? </summary>
        public bool LabelOnHybrid { get; set; }

        /// <summary>
        /// Corner points used to form the outline of the location.
        /// </summary>
        public LocationCollection Locations {
            get {
                if(_locations == null)
                    Locations = new LocationCollection();
                return _locations;
            }
            set {
                _locations = value;
                OutLine = new MapPolygon() {
                    Fill = new SolidColorBrush(Colors.Gray) { Opacity = 0.3 },
                    Stroke = new SolidColorBrush(Colors.White) { Opacity = 0.7 },
                    StrokeThickness = 5,
                    Locations = _locations,
                };
            }
        }

        /// <summary> Minimum zoom level the label should be displayed. </summary>
        public int MinZoomLevel { get; set; }

        /// <summary> The polygon outline of the location. </summary>
        public MapPolygon OutLine { get; private set; }
        #endregion

        #region Private Methods
        private void Initialize() {
            OutLine = new MapPolygon() {
                StrokeThickness = 5,
                Locations = _locations,
            };
            ShowOutline(OutLine);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a corner point to the outline of the location..
        /// </summary>
        /// <param name="location">The corner point to add</param>
        public void AddLocation(Location location) {
            Locations.Add(location);
        }

        /// <summary>
        /// The label to show for this location.
        /// </summary>
        /// <returns>Pushpin to be added to the map</returns>
        public Pushpin GetLabel() {
            return new Pushpin() {
                Location = this.Center,
                Content = this.Label,
                Background = new SolidColorBrush(Colors.Transparent),
                PositionOrigin = PositionOrigin.Center,
            };
        }

        /// <summary>
        /// The label to show for this location.
        /// </summary>
        /// <param name="template">Template to be applied to the pushpin</param>
        /// <returns>Pushpin to be added to the map</returns>
        public Pushpin GetLabel(ControlTemplate template) {
            return new Pushpin() {
                Location = this.Center,
                Template = template,
                Content = this.Label,
                Background = new SolidColorBrush(Colors.Transparent),
                PositionOrigin = PositionOrigin.Center,
            };
        }

        /// <summary>
        /// Turns a polygon invisible
        /// </summary>
        /// <param name="polygon">The polygon to hide</param>
        public static void HideOutline(MapPolygon polygon) {
            polygon.Fill = new SolidColorBrush(Colors.Transparent);
            polygon.Stroke = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Compares polygons by their label
        /// </summary>
        /// <param name="polygon">The second polygon</param>
        /// <returns>Whether their labels are equal</returns>
        public bool IsPolygonEqual(MapPolygon polygon) {
            if(Locations.Count <= 0) return false;
            if(polygon == null || polygon.Locations == null) return false;
            if(polygon.Locations.Count <= 0) return false;
            return polygon.Locations.Contains(Locations[0]);
        }

        public void Merge(RhitLocation location) {
            if(Id != location.Id) return;
            Center = location.Center;
            Description = location.Description;
            IsPOI = location.IsPOI;
            Label = location.Label;
            LabelOnHybrid = location.LabelOnHybrid;
            MinZoomLevel = location.MinZoomLevel;
            OnQuikList = location.OnQuikList;
            ParentId = location.ParentId;
            IsDepartable = IsDepartable;
            Links = location.Links;
            AltNames = location.AltNames;
        }

        /// <summary>
        /// Makes a hidden polygon become visible again.
        /// </summary>
        /// <param name="polygon">Polygon to show</param>
        public static void ShowOutline(MapPolygon polygon) {
            polygon.Fill = new SolidColorBrush(Colors.Gray) { Opacity = 0.3 };
            polygon.Stroke = new SolidColorBrush(Colors.White) { Opacity = 0.7 };
        }
        #endregion

        public int ParentId { get; set; }

        public bool IsDepartable { get; set; }

        public bool OnQuikList { get; set; }

        public Dictionary<string, string> Links { get; set; }

        public List<string> AltNames { get; set; }
    }
}