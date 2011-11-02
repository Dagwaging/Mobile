using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace RhitMobile {
    public partial class SearchPage : PhoneApplicationPage {
        public SearchPage() {
            InitializeComponent();
        }

        private void SearchAll_KeyUp(object sender, KeyEventArgs e) {
            //if(e.Key == Key.Enter)
            //    PerformSearch(SearchAllTextBox.Text);
        }

        private void SearchPlaces_KeyUp(object sender, KeyEventArgs e) {
            //if(e.Key == Key.Enter)
            //    PerformSearch(SearchPlacesTextBox.Text);
        }
        
        private void SearchPeople_KeyUp(object sender, KeyEventArgs e) {
            //if(e.Key == Key.Enter)
            //    PerformSearch(SearchPeopleTextBox.Text);
        }
    }
}