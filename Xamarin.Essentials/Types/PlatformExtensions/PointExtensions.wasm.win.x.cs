using System;
using System.Drawing;
using WindowsPoint = Windows.Foundation.Point;

namespace Xamarin.Essentials
{
    public static partial class PointExtensions
    {
        public static WindowsPoint ToPlatformPoint(this Point point) =>
            new WindowsPoint(point.X, point.Y);

        public static WindowsPoint ToPlatformPoint(this PointF point) =>
            new WindowsPoint(point.X, point.Y);
    }
}
