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

            string queryString;
            int id;
            bool isTour;
            if(NavigationContext.QueryString.TryGetValue("Id", out queryString))
                id = Convert.ToInt32(queryString);
            else id = -1;
            if(NavigationContext.QueryString.TryGetValue("Tours", out queryString))
                isTour = Convert.ToBoolean(queryString);
            else isTour = false;
            ViewModel.SetLocation(id, isTour);
        }
    }
}