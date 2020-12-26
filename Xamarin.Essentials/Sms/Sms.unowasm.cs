using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class Sms
    {
        internal static bool IsComposeSupported => true;

        static Task PlatformComposeAsync(SmsMessage message)
        {
            WebAssemblyRuntime.InvokeJS($"UnoSms_Compose('{message.Recipients.FirstOrDefault()}','{message.Body}')");
            return Task.CompletedTask;
        }
    }
}
