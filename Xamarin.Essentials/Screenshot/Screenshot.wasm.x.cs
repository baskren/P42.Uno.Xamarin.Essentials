using System;
using System.IO;
using System.Net.Http;
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
            //var result = await WebAssemblyRuntime.InvokeAsync("UnoScreenshot_GetUrlPromise()");
            var result = await JavaScripts.Uno.ScreenShot.GetUrlAsync();

            // var url = WebAssemblyRuntime.InvokeJS("UnoScreenshot_GetUrl()");
            Console.WriteLine("PlatformCaptureAsync result:" + result);
            if (result.StartsWith("success: true"))
                return new ScreenshotResult(result);

            return null;
        }
    }

    public partial class ScreenshotResult
    {
        public string PngPath { get; private set; }

        public string JpegPath { get; private set; }

        internal ScreenshotResult(string javascriptResponse)
        {
            var kvps = javascriptResponse.Split(',');
            foreach (var kvp in kvps)
            {
                var pair = kvp.Split(':');
                if (pair.Length > 1)
                {
                    switch (pair[0].Trim())
                    {
                        case nameof(Width): Width = int.Parse(pair[1].Trim()); break;
                        case nameof(Height): Height = int.Parse(pair[1].Trim()); break;
                        case nameof(PngPath): PngPath = pair[1].Trim(); break;
                        case nameof(JpegPath): JpegPath = pair[1].Trim(); break;
                        default: break;
                    }
                }
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal async Task<Stream> PlatformOpenReadAsync(ScreenshotFormat format)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var path = format == ScreenshotFormat.Jpeg ? JpegPath : PngPath;
            var b64 = File.ReadAllText(path);
            var bytes = Convert.FromBase64String(b64);
            return new MemoryStream(bytes);
        }
    }
}
