using System;
using System.Collections.Generic;
using System.Text;
using Uno.Foundation;

#if __WASM__
namespace Xamarin.Essentials.Vibration
{
    public partial class Vibration
    {
        internal static bool IsSupported
        {
            get
            {
                var result = WebAssemblyRuntime.InvokeJS("UnoVibrationIsSupported()");
                return result == "true";
            }
        }

        static void PlatformVibrate(TimeSpan duration)
        {
            WebAssemblyRuntime.InvokeJS($"UnoVibration_Vibrate({duration.TotalMilliseconds})");
        }

        static void PlatformCancel()
        {
            WebAssemblyRuntime.InvokeJS("UnoVibration_Cancel()");
        }
    }
}
#endif
