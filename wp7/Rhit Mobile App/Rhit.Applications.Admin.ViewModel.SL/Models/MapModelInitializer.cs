using Rhit.Applications.ViewModel.Providers;

namespace Rhit.Applications.ViewModel.Models {
    public class MapModelInitializer {
        public MapModelInitializer() { }

        private DynMapViewModel _viewModelInstance;
        public DynMapViewModel ViewModelInstance {
            get { return _viewModelInstance; }
            set {
                _viewModelInstance = value;
                TryInit();
            }
        }

        private IBuildingMappingProvider _buildingMappingProvider;
        public IBuildingMappingProvider BuildingMappingProvider {
            get { return _buildingMappingProvider; }
            set {
                _buildingMappingProvider = value;
                TryInit();
            }
        }

        private ILocationsProvider _locationsProvider;
        public ILocationsProvider LocationsProvider {
            get { return _locationsProvider; }
            set {
                _locationsProvider = value;
                TryInit();
            }
        }

        private IBitmapProvider _bitmapProvider;
        public IBitmapProvider BitmapProvider {
            get { return _bitmapProvider; }
            set {
                _bitmapProvider = value;
                TryInit();
            }
        }

        public bool TryInit() {
            if(ViewModelInstance == null || BuildingMappingProvider == null || LocationsProvider == null || BitmapProvider == null) return false;
            ViewModelInstance.Initialize(BuildingMappingProvider, LocationsProvider, BitmapProvider);
            return true;
        }

    }
}
