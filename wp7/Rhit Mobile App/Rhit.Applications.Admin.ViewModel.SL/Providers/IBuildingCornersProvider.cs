using System;
using System.Collections.Generic;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel.Providers {
    public interface IBuildingCornersProvider {
        void DisplayCorners(ICollection<Location> corners);
        List<Location> GetCorners();
        void RemoveCorners();
    }
}
