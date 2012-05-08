using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Rhit.Applications.ViewModels.Models;

namespace Rhit.Applications.Views.Converters {
    public class NodeStateToColorConverter : IValueConverter {
        public NodeStateToColorConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Node.State state = (Node.State) value;

            if(state.HasFlag(Node.State.EndNode))
                return new SolidColorBrush(Colors.Red);
            if(state.HasFlag(Node.State.CanSelect))
                return new SolidColorBrush(Colors.Blue);
            if(state.HasFlag(Node.State.Selected))
                return new SolidColorBrush(Colors.Green);
            return new SolidColorBrush(Colors.Black);
        }
    
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}