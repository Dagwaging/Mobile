using System.Collections.Generic;
using Microsoft.Maps.MapControl;
using System.Windows;

namespace Rhit.Applications.ViewModel.Providers {
    public interface ILocationsProvider {
        //Display the given Coordinates
        void DisplayCorners(ICollection<Location> corners);

        //Retrieve any changes made to the corners
        IList<Location> GetLocations();

        IList<Point> GetPoints();

        //Allow user to create a new set of corners
        void CreateNewCorners();

	    int Id { get; set; }

        //Done with corners
        void Clear();

        void QueryLocation();
    }
}
