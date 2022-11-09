using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Samples.Helpers
{
    public static class ViewHelpers
    {
        public static Rect GetAbsoluteBounds(this FrameworkElement element)
        {
            var ttv = element.TransformToVisual(Xamarin.Essentials.Platform.Window.Content);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
        }

        /*
        public static System.Drawing.Rectangle ToSystemRectangle(this Rect rect) =>
            new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        */
    }
}
