using System;
//using Windows.Phone.Devices.Notification;
using Windows.Devices.Haptics;

namespace Xamarin.Essentials
{
    public static partial class Vibration
    {

        internal static bool IsSupported => false;

        // => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice") && DefaultDevice != null;
        // static VibrationDevice DefaultDevice => VibrationDevice.GetDefault();

        static void PlatformVibrate(TimeSpan duration)
        {
            // => DefaultDevice.Vibrate(duration);
        }

        static void PlatformCancel()
        {
            // => DefaultDevice.Cancel();
        }
    }
}
