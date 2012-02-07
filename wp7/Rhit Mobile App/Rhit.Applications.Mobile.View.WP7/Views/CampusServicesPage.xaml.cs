using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace Rhit.Applications.View.Views {
    public partial class CampusServicesPage : PhoneApplicationPage {
        public const string NavigationArg_Index = "Index";

        public CampusServicesPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public int Index { get; private set; }

        private void ParseNavigationArgs() {
            string indexString;
            if(NavigationContext.QueryString.TryGetValue(NavigationArg_Index, out indexString))
                Index = Convert.ToInt32(indexString);
            else Index = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            int index = (int) (sender as Button).Tag;
            NavigationService.Navigate(new Uri("/Views/CampusServicesPage.xaml?Index=" + index.ToString(), UriKind.Relative));
        }

        //private void CategorySelected(object sender, SelectionChangedEventArgs e) {
        //    if((sender as ListBox).SelectedItem == null) return;
        //    ViewModel.SelectLocation((sender as ListBox).SelectedItem);
        //    NavigationService.Navigate(new Uri("/Views/InfoPage.xaml?Index=" + (Index + 1).ToString(), UriKind.Relative));
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            ParseNavigationArgs();
            ViewModel.SetIndex(Index);
        }

        //protected override void OnBackKeyPress(CancelEventArgs e) {
        //    if(Index == 0) ViewModel.ClearStack();
        //}
    }
}