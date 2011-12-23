using System.Collections.Generic;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel.Providers {
    public interface ILocationsProvider {
        //Display the given Coordinates
        void DisplayCorners(ICollection<Location> corners);

        //Retrieve any changes made to the corners
        IList<Location> GetLocations();

        //Allow user to create a new set of corners
        void CreateNewCorners();

        int GetId();

        //Done with corners
        void ClearLocations();

        void QueryLocation();
    }
}
