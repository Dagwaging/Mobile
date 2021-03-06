﻿using System;
using System.Collections.Generic;
using Microsoft.Maps.MapControl;

namespace Rhit.Applications.ViewModel.Providers {
    public interface IBuildingCornersProvider {
        void DisplayCorners(ICollection<Location> corners);
        ICollection<Location> GetCorners();
        void RemoveCorners();
    }
}
