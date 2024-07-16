using System;
//using Windows.Phone.Devices.Notification;
using Windows.Devices.Haptics;

namespace Xamarin.Essentials
{
    public static partial class Vibration
    {

        internal static bool IsSupported => false;

        static void PlatformVibrate(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        static void PlatformCancel()
        {
            throw new NotSupportedException();
        }
    }
}
