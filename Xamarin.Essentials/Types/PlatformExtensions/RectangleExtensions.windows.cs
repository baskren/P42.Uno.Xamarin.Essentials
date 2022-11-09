using System;
using System.Drawing;
using Windows.Foundation;

namespace Xamarin.Essentials
{
    public partial static class RectangleExtensions
    {

        public static Rect ToPlatformRectangle(this Rectangle rect) =>
            new Rect(rect.X, rect.Y, rect.Width, rect.Height);

        public static Rect ToPlatformRectangle(this RectangleF rect) =>
            new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }
}

