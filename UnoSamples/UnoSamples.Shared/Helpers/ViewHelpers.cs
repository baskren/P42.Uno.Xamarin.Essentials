using Windows.Foundation;
using Windows.UI.Xaml;

namespace Samples.Helpers
{
    public static class ViewHelpers
    {
        public static Rect GetAbsoluteBounds(this FrameworkElement element)
        {
            var ttv = element.TransformToVisual(Windows.UI.Xaml.Window.Current.Content);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
        }

#if !NETFX_CORE && !__WASM__
        public static System.Drawing.Rectangle ToSystemRectangle(this Rect rect) =>
            new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
#endif
    }
}
