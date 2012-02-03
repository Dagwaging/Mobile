using System.Windows;

namespace Rhit.Applications.ViewModel.Utilities {
    public class AlternateName : DependencyObject {
        public AlternateName() { Name = ""; }
        public AlternateName(string name) { Name = name; }
        public virtual string Name { get; set; }
    }
}
