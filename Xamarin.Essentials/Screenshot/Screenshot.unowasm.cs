using System.IO;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class Screenshot
    {
        static bool PlatformIsCaptureSupported =>
            true;

        static async Task<ScreenshotResult> PlatformCaptureAsync()
        {
            var url = await WebAssemblyRuntime.InvokeAsync("UnoScreenshot_GeneratePromise()");
            return null;
        }
    }

    public partial class ScreenshotResult
    {
        ScreenshotResult()
        {
        }

        internal Task<Stream> PlatformOpenReadAsync(ScreenshotFormat format) =>
            throw ExceptionUtils.NotSupportedOrImplementedException;
    }
}
