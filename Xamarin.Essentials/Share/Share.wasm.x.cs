using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class Share
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async Task PlatformRequestAsync(ShareRequestBase request)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var json = JsonConvert.SerializeObject(request);
            //WebAssemblyRuntime.InvokeJS($"UnoShare_Share('{json}')");
            JavaScripts.Uno.JsonShare.Share(json);
        }

        /*
        static async Task PlatformRequestAsync(ShareMultipleFilesRequest request)
        {
            //throw new Exception("File Sharing in WASM requires the use of SharingExtensions.SetShareRequestPayload.  Share.RequestAsync will not work due to HTML5 security limitations.");
            var json = JsonConvert.SerializeObject(request);
            WebAssemblyRuntime.InvokeJS($"UnoShare_Share('{json}')");
        }
        */

        static bool PlatformCanShare(ShareRequestBase request)
        {
            var json = JsonConvert.SerializeObject(request);
            //var result = WebAssemblyRuntime.InvokeJS($"UnoShare_CanShare('{json}')");
            var result = JavaScripts.Uno.JsonShare.CanShare(json);
            return result;
        }

        static bool PlatformIsAvailable()
        {
            //return WebAssemblyRuntime.InvokeJS("UnoShare_IsAvailable()") == "true";
            return JavaScripts.Uno.JsonShare.IsAvailable();
        }
    }
}
