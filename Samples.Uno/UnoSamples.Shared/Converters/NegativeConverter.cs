using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace Samples.Converters
{
    public class NegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool v)
                return !v;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool v)
                return !v;
            else
                return true;
        }
    }
}
