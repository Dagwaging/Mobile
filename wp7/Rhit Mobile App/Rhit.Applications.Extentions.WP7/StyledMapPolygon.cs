using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls.Maps;

namespace Rhit.Applications.Extentions {
    public class StyledMapPolygon : MapPolygon {
        #region Fill
        public static readonly DependencyProperty FillProperty =
           DependencyProperty.Register("Fill", typeof(Brush), typeof(StyledMapPolygon),
           new PropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnFillChanged)));

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StyledMapPolygon instance = (StyledMapPolygon) d;
            instance.Fill = e.NewValue as Brush;
        }
        #endregion

        #region Stroke
        public static readonly DependencyProperty StrokeProperty =
           DependencyProperty.Register("Stroke", typeof(Brush), typeof(StyledMapPolygon),
           new PropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnStrokeChanged)));

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StyledMapPolygon instance = (StyledMapPolygon) d;
            instance.Stroke = e.NewValue as Brush;
        }
        #endregion

        #region StrokeThickness
        public static readonly DependencyProperty StrokeThicknessProperty =
           DependencyProperty.Register("StrokeThickness", typeof(int), typeof(StyledMapPolygon),
           new PropertyMetadata(1, new PropertyChangedCallback(OnStrokeThicknessChanged)));

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StyledMapPolygon instance = (StyledMapPolygon) d;
            instance.StrokeThickness = (int) e.NewValue;
        }
        #endregion
    }
}
