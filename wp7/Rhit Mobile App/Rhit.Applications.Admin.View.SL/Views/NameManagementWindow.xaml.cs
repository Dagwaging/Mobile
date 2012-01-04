using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rhit.Applications.View.Views {
    public partial class NameManagementWindow : ChildWindow {
        public NameManagementWindow(IList<string> names) {
            InitializeComponent();
            DataContext = this;
            AltNames = names;
            Names = new ObservableCollection<string>();
            if(names != null) foreach(string s in names) Names.Add(s);
        }

        public IList<string> AltNames { get; private set; }

        public ObservableCollection<string> Names { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e) {
            AltNames = new List<string>();
            foreach(string s in Names) AltNames.Add(s);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void AddNameButton_Click(object sender, RoutedEventArgs e) {
            if(!Names.Contains(AddName.Text))
                Names.Add(AddName.Text);
            NamesList.SelectedItem = AddName.Text;
        }

        private void RemoveNameButton_Click(object sender, RoutedEventArgs e) {
            int tmp = NamesList.SelectedIndex;
            Names.Remove(NamesList.SelectedItem as string);
            try {
                if(tmp > 0) tmp--;
                NamesList.SelectedIndex = tmp;
            } catch { }
        }

        private void AddName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == Key.Enter) AddNameButton_Click(this, null);
        }
    }
}

