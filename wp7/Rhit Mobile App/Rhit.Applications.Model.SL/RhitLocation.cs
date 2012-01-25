#if WINDOWS_PHONE
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl.Core;
using Microsoft.Maps.MapControl;
#endif

using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace Rhit.Applications.Model {
    /// \ingroup objects
    /// <summary>
    /// Represents an area, building, or room at Rose-Hulman.
    /// </summary>
    public class RhitLocation {
        #region Constructors
        /// <summary>
        /// Constructor; Basic initialization; Make sure to set relevant properties.
        /// </summary>
        public RhitLocation() {
            Links = new List<ILink>();
            AltNames = new List<string>();
        }
        #endregion

        #region Public Properties
        /// <summary> Center of the location. </summary>
        public GeoCoordinate Center { get; set; }

        /// <summary> Description of the location. </summary>
        public string Description { get; set; }

        /// <summary> Floor of the location. </summary>
        public int Floor { get; set; }

        /// <summary> Id of the location (Used only by the service). </summary>
        public int Id { get; set; }

        /// <summary> Name of the location. </summary>
        public string Label { get; set; }

        /// <summary> Does the name already appear on hybrid maps? </summary>
        public bool LabelOnHybrid { get; set; }

        /// <summary>
        /// Corner points used to form the outline of the location.
        /// </summary>
        public LocationCollection Corners { get; set; }

        /// <summary> Minimum zoom level the label should be displayed. </summary>
        public int MinZoomLevel { get; set; }

        public int ParentId { get; set; }

        public LocationType Type { get; set; }

        public IList<ILink> Links { get; set; }

        public IList<string> AltNames { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a corner point to the outline of the location..
        /// </summary>
        /// <param name="location">The corner point to add</param>
        public void AddLocation(GeoCoordinate location) {
            Corners.Add(location);
        }

        public RhitLocation Copy() {
            return new RhitLocation() {
                AltNames = AltNames,
                Center = Center,
                Corners = Corners,
                Description = Description,
                Floor = Floor,
                Id = Id,
                Label = Label,
                LabelOnHybrid = LabelOnHybrid,
                Links = Links,
                MinZoomLevel = MinZoomLevel,
                ParentId = ParentId,
                Type = Type,
            };
        }

        public void Overwrite(RhitLocation location) {
            if(Id != location.Id) return;
            AltNames = location.AltNames;
            Center = location.Center;
            Corners = location.Corners;
            Description = location.Description;
            Floor = location.Floor;
            Label = location.Label;
            LabelOnHybrid = location.LabelOnHybrid;
            Links = location.Links;
            MinZoomLevel = location.MinZoomLevel;
            ParentId = location.ParentId;
            Type = location.Type;
        }

        public void AddLink(string name, string address) {
            Links.Add(new Link(name, address));
        }
        #endregion

        private class Link : ILink {
            public Link(string name, string address) {
                Name = name;
                Address = Address;
            }
            public string Name { get; set; }
            public string Address { get; set; }
        }
    }

    public interface ILink {
        string Name { get; set; }
        string Address { get; set; }
    }
}