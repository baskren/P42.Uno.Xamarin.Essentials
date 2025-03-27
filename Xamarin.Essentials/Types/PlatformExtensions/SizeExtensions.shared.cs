using System;
using System.Drawing;
using WindowsSize = Windows.Foundation.Size;

namespace Xamarin.Essentials;

public static partial class SizeExtensions
{
    public static Size ToSystemSize(this WindowsSize size)
    {
        if (size.Width > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(size.Width));

        if (size.Height > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(size.Height));

        return new Size((int)size.Width, (int)size.Height);
    }

    public static SizeF ToSystemSizeF(this WindowsSize size) => new((float)size.Width, (float)size.Height);

    public static WindowsSize ToWindowsSize(this Size size) => new(size.Width, size.Height);

    public static WindowsSize ToWindowsSize(this SizeF size) => new(size.Width, size.Height);
}