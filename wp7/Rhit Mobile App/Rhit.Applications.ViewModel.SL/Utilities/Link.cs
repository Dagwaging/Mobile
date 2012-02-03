using System.Windows;
using Rhit.Applications.Model;

namespace Rhit.Applications.ViewModel.Utilities {
    public class Link : DependencyObject, ILink {
        public Link() { }

        public virtual string Name { get; set; }

        public virtual string Address { get; set; }
    }
}
