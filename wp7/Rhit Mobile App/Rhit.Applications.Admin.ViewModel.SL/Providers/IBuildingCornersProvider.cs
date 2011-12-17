using System;
using System.Collections.Generic;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel.Providers {
    public interface IBuildingCornersProvider {
        //Display the given Coordinates
        void DisplayCorners(ICollection<Location> corners);

        //Retrieve any changes made to the corners
        List<Location> GetCorners();

        //Allow user to create a new set of corners
        void CreateNewCorners();

        //Done with corners
        void ClearCorners();
    }
}
