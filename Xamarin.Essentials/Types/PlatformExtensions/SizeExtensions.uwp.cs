using System;
using System.Drawing;
using WindowsSize = Windows.Foundation.Size;

namespace Xamarin.Essentials
{
    public static partial class SizeExtensions
    {
        public static WindowsSize ToPlatformSize(this Size size) =>
            new WindowsSize(size.Width, size.Height);

        public static WindowsSize ToPlatformSize(this SizeF size) =>
            new WindowsSize(size.Width, size.Height);
    }
}
