using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class Share
    {
        static async Task PlatformRequestAsync(ShareTextRequest request)
        {
            var shareData = "{ ";
            shareData += string.IsNullOrEmpty(request.Title) ? null : $"title: '{request.Title}',";
            shareData += string.IsNullOrEmpty(request.Text) ? null : $"text: '{request.Text}',";
            shareData += string.IsNullOrEmpty(request.Uri) ? null : $"url: '{request.Uri}' ";
            shareData += "}";
            var script = $"navigator.share({shareData})";
            await WebAssemblyRuntime.InvokeAsync(script);
        }

        static async Task PlatformRequestAsync(ShareMultipleFilesRequest request)
        {
            System.Diagnostics.Debug.WriteLine("Share.PlatformRequestAsync ShareMultipleFilesRequest A");
            var canShare = WebAssemblyRuntime.InvokeJS($"UnoShare_CanShareFiles([ '{string.Join("', '", request.Files.Select(f => f.FullPath))}' ])");
            System.Diagnostics.Debug.WriteLine("Share. B");
            if (canShare == "true")
            {
                System.Diagnostics.Debug.WriteLine("Share. C");
                await WebAssemblyRuntime.InvokeAsync($"UnoShare_ShareFiles({request.Title}, [ '{string.Join("', '", request.Files.Select(f => f.FullPath))}' ])");
                System.Diagnostics.Debug.WriteLine("Share. DONE");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Share. Can't share! EXIT");
            }

        }

        static bool PlatformCanShareFile() => false;

    }
}
