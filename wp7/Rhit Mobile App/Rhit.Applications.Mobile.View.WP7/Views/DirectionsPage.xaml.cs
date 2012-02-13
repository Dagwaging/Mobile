using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Rhit.Applications.Extentions.Maps;
using Rhit.Applications.ViewModel.Models;

namespace Rhit.Applications.View.Views {
    public partial class DirectionsPage : PhoneApplicationPage {
        public DirectionsPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public DirectionsViewModel ViewModel = new DirectionsViewModel();

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            RhitMapExtender.Attach(MyMap);

            string idString;
            int id;
            if(NavigationContext.QueryString.TryGetValue("Id", out idString))
                id = Convert.ToInt32(idString);
            else id = -1;
            ViewModel.SetLocation(id);
        }
    }
}