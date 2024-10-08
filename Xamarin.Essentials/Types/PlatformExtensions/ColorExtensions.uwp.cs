﻿using System.Drawing;
using WindowsColor = Windows.UI.Color;

namespace Xamarin.Essentials
{
    public static partial class ColorExtensions
    {

        public static WindowsColor ToPlatformColor(this Color color) =>
               WindowsColor.FromArgb(color.A, color.R, color.G, color.B);
    }
}
