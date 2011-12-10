using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rhit.Applications.Mvvm.Utilities {
    public class BindingUtility {

        public static bool GetUpdateSourceOnChange(DependencyObject d) {
            return (bool) d.GetValue(UpdateSourceOnChangeProperty);
        }

        public static void SetUpdateSourceOnChange(DependencyObject d, bool value) {
            d.SetValue(UpdateSourceOnChangeProperty, value);
        }

        // Using a DependencyProperty as the backing store for …
        public static readonly DependencyProperty
          UpdateSourceOnChangeProperty =
            DependencyProperty.RegisterAttached(
            "UpdateSourceOnChange",
            typeof(bool), typeof(BindingUtility),
            new PropertyMetadata(false, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(d is TextBox) {
                TextBox textBox = d as TextBox;
                if(textBox == null)
                    return;
                if((bool) e.NewValue) {
                    textBox.TextChanged += OnTextChanged;
                } else {
                    textBox.TextChanged -= OnTextChanged;
                }
            } else if(d is PasswordBox) {
                PasswordBox passwordBox = d as PasswordBox;
                if(passwordBox == null)
                    return;
                if((bool) e.NewValue) {
                    passwordBox.PasswordChanged += OnPasswordChanged;
                } else {
                    passwordBox.PasswordChanged -= OnPasswordChanged;
                }
            }
        }

        static void OnPasswordChanged(object sender, RoutedEventArgs e) {
            PasswordBox passwordBox = sender as PasswordBox;
            if(passwordBox == null) return;

            BindingExpression bindingExpression = passwordBox.GetBindingExpression(PasswordBox.PasswordProperty);
            if(bindingExpression != null) {
                bindingExpression.UpdateSource();
            }
        }

        static void OnTextChanged(object s, TextChangedEventArgs e) {
            TextBox textBox = s as TextBox;
            if(textBox == null) return;

            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            if(bindingExpression != null) {
                bindingExpression.UpdateSource();
            }
        }
    }
}