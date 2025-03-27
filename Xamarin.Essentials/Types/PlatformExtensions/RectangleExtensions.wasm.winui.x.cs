using System;
using System.Drawing;
using Windows.Foundation;

namespace Xamarin.Essentials;

public static partial class RectangleExtensions
{
    public static Rect ToPlatformRectangle(this Rectangle rect) => new(rect.X, rect.Y, rect.Width, rect.Height);

    public static Rect ToPlatformRectangle(this RectangleF rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
}