using System;
using System.Drawing;
using Android.Graphics;

namespace Xamarin.Essentials;

public static partial class RectangleExtensions
{
    public static Rectangle ToSystemRectangle(this Rect rect) => new(rect.Left, rect.Top, rect.Width(), rect.Height());

    public static RectangleF ToSystemRectangleF(this RectF rect) =>
        new(rect.Left, rect.Top, rect.Width(), rect.Height());

    public static Rect ToPlatformRectangle(this Rectangle rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);

    public static RectF ToPlatformRectangleF(this RectangleF rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
}