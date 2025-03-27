using System.Drawing;
using AndroidSize = Android.Util.Size;
using AndroidSizeF = Android.Util.SizeF;

namespace Xamarin.Essentials;

public static partial class SizeExtensions
{
    public static Size ToSystemSize(this AndroidSize size) => new(size.Width, size.Height);

    public static SizeF ToSystemSizeF(this AndroidSizeF size) => new(size.Width, size.Height);

    public static AndroidSize ToPlatformSize(this Size size) => new(size.Width, size.Height);

    public static AndroidSizeF ToPlatformSizeF(this SizeF size) => new(size.Width, size.Height);
}