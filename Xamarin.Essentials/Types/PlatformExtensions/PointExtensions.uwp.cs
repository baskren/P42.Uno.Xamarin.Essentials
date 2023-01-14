using System;
using System.Drawing;
using Windows.Foundation;

namespace Xamarin.Essentials
{
    public static partial class RectangleExtensions
    {
        public static WindowsPoint ToPlatformPoint(this Point point) =>
            new WindowsPoint(point.X, point.Y);

        public static WindowsPoint ToPlatformPoint(this PointF point) =>
            new WindowsPoint(point.X, point.Y);

    }
}
