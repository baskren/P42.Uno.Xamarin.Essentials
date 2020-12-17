#if __WASM__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials.Share
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
            var filesArray = "['" + string.Join("', '", request.Files.Select(f => f.FullPath)) + "']";

            var canShare = WebAssemblyRuntime.InvokeJS("navigator.canShare({ files: " + filesArray + " })");
            if (canShare == "true")
            {
                var shareData = "{ ";
                shareData += string.IsNullOrEmpty(request.Title) ? null : $"title: '{request.Title}',";
                shareData += $"files: '{filesArray}' ";
                shareData += "}";
                var script = $"navigator.share({shareData})";
                await WebAssemblyRuntime.InvokeAsync(script);
            }
        }
    }
}
#endif
