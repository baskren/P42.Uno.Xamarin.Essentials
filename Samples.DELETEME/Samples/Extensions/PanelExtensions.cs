using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Samples.View
{
    public static class PanelExtensions
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(PanelExtensions),
            new PropertyMetadata(true, OnIsEnabledChanged));

        static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is Control element)
                        element.IsEnabled = (bool)e.NewValue;
                }
            }
        }

        public static bool GetIsEnabled(Panel target)
            => (bool)target.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(Panel target, bool value)
            => target.SetValue(IsEnabledProperty, value);
    }
}
