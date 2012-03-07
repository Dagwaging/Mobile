using System.Collections.ObjectModel;
using System.Windows;
using Rhit.Applications.Models;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps.Platform;
using System.Device.Location;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.ViewModels.Utilities {
    public class SimpleRhitLocation : DependencyObject {
        public SimpleRhitLocation(LocationData location) {
            OriginalLocation = location;
            InitilizeData();
        }

        private void InitilizeData() {
            AltNames = new ObservableCollection<AlternateName>();
            foreach(string name in OriginalLocation.AltNames) AltNames.Add(new AlternateName(name));
            Corners = new ObservableCollection<Location>();
            foreach(Location location in OriginalLocation.Corners) Corners.Add(location);
            Links = new ObservableCollection<Link>();
            foreach(ILink link in OriginalLocation.Links)
                Links.Add(new Link() { Name = link.Name, Address = link.Address, });

            Center = OriginalLocation.Center;
            Description = OriginalLocation.Description;
            Floor = OriginalLocation.Floor;
            Id = OriginalLocation.Id;
            LabelOnHybrid = OriginalLocation.LabelOnHybrid;
            MinZoom = OriginalLocation.MinZoomLevel;
            Label = OriginalLocation.Label;
            ParentId = OriginalLocation.ParentId;
            Type = OriginalLocation.Type;
        }

        public LocationData OriginalLocation { get; private set; }

        public ObservableCollection<AlternateName> AltNames { get; set; }

        public ObservableCollection<Location> Corners { get; private set; }

        public ObservableCollection<Link> Links { get; private set; }

        public virtual string Description { get; set; }

        public virtual int Floor { get; set; }

        public virtual GeoCoordinate Center { get; set; }

        public virtual int Id { get; set; }

        public virtual string Label { get; set; }

        public virtual bool LabelOnHybrid { get; set; }

        public virtual int MinZoom { get; set; }

        public virtual int ParentId { get; set; }

        public virtual LocationType Type { get; set; }
    }
}
