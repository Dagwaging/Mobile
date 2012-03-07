using System.Windows;
using System.Windows.Media;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Maps;
#else
using Microsoft.Maps.MapControl;
#endif

namespace Rhit.Applications.Extentions.Controls {
    public class StyledMapPolygon : MapPolygon {
        #region DependencyFill
        public Brush DependencyFill {
            get { return Fill; }
            set { SetValue(DependencyFillProperty, value); }
        }

        public static readonly DependencyProperty DependencyFillProperty =
           DependencyProperty.Register("DependencyFill", typeof(Brush), typeof(StyledMapPolygon),
           new PropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnFillChanged)));

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StyledMapPolygon instance = (StyledMapPolygon) d;
            instance.Fill = e.NewValue as Brush;
        }
        #endregion

        #region DependencyStroke
        public Brush DependencyStroke {
            get { return Stroke; }
            set { SetValue(DependencyStrokeProperty, value); }
        }

        public static readonly DependencyProperty DependencyStrokeProperty =
           DependencyProperty.Register("DependencyStroke", typeof(Brush), typeof(StyledMapPolygon),
           new PropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnStrokeChanged)));

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StyledMapPolygon instance = (StyledMapPolygon) d;
            instance.Stroke = e.NewValue as Brush;
        }
        #endregion

        #region DependencyStrokeThickness
        public double DependencyStrokeThickness {
            get { return StrokeThickness; }
            set { SetValue(DependencyStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty DependencyStrokeThicknessProperty =
           DependencyProperty.Register("DependencyStrokeThickness", typeof(double), typeof(StyledMapPolygon),
           new PropertyMetadata(1.0, new PropertyChangedCallback(OnStrokeThicknessChanged)));

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            StyledMapPolygon instance = (StyledMapPolygon) d;
            instance.StrokeThickness = (int) e.NewValue;
        }
        #endregion
    }
}
