using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using RhitMobile.ObjectModel;

namespace RhitMobile {
    public partial class DescriptionPage : PhoneApplicationPage {
        public DescriptionPage() {
            InitializeComponent();
            UpdateValues();
        }

        private RhitLocation Location { get; set; }

        private void UpdateValues() {
            if(Location == null) return;
            PageTitle.Text = Location.Label;
            description.Text = Location.Description;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            //RhitLocation location = this.LoadState<RhitLocation>("SelectedOutline", null);
            RhitLocation location = RhitMapView.Instance.SelectedLocation;
            if(location == null) {
                //TODO: Handle this better; Shouldn't give whiplash
                if(NavigationService.CanGoBack) NavigationService.GoBack();
                else NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
            }
            Location = location;
            UpdateValues();
        }
    }
}