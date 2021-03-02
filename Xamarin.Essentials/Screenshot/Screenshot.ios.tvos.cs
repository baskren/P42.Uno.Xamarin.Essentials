using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Xamarin.Essentials
{
    public static partial class Screenshot
    {
        internal static bool PlatformIsCaptureSupported =>
            UIScreen.MainScreen != null;

        static Task<ScreenshotResult> PlatformCaptureAsync()
        {
            var img = UIScreen.MainScreen.Capture();
            var result = new ScreenshotResult(img);

            return Task.FromResult(result);
        }
    }

    public partial class ScreenshotResult
    {
        readonly UIImage uiImage;

        internal ScreenshotResult(UIImage image)
        {
            uiImage = image;

            Width = (int)(image.Size.Width * image.CurrentScale);
            Height = (int)(image.Size.Height * image.CurrentScale);
        }

        internal Task<Stream> PlatformOpenReadAsync(ScreenshotFormat format)
        {
            if (format == ScreenshotFormat.Jpeg)
                return Task.FromResult(uiImage.AsJPEG().AsStream());
            return Task.FromResult(uiImage.AsPNG().AsStream());
        }
    }
}
