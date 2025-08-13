using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Windows.UI;
using Xamarin.Essentials;

namespace Samples.Converters;

public class SolidBrushConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Windows.UI.Color v)
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(v);

        if (value is System.Drawing.Color c)
            return new Microsoft.UI.Xaml.Media.SolidColorBrush(c.ToWindowsColor());

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Microsoft.UI.Xaml.Media.SolidColorBrush v)
            return v.Color;
        return default(Color);
    }
}
