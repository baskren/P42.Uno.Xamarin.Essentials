using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Samples.View
{
    public static class TextBoxExtensions
    {
        public static readonly DependencyProperty AutoSizeProperty = DependencyProperty.RegisterAttached(
            "AutoSize",
            typeof(bool),
            typeof(TextBoxExtensions),
            new PropertyMetadata(false, OnAutoSizeChanged));

        static void OnAutoSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox element)
            {
                if (GetAutoSize(element))
                    element.TextChanged += OnTextBoxTextChanged;
                else
                    element.TextChanged -= OnTextBoxTextChanged;
            }
        }

        public static bool GetAutoSize(TextBox target)
            => (bool)target.GetValue(AutoSizeProperty);

        public static void SetAutoSize(TextBox target, bool value)
            => target.SetValue(AutoSizeProperty, value);

        static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.InvalidateMeasure();
        }
    }
}
