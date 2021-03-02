using System;
using System.Runtime.InteropServices;

namespace Xamarin.Essentials
{
    public static partial class DeviceDisplay
    {
        [DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_request_lock")]
        static extern void RequestKeepScreenOn(int type = 1, int timeout = 0);

        [DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_release_lock")]
        static extern void ReleaseKeepScreenOn(int type = 1);

        static bool keepScreenOn = false;

        static bool PlatformKeepScreenOn
        {
            get => keepScreenOn;
            set
            {
                if (value)
                    RequestKeepScreenOn();
                else
                    ReleaseKeepScreenOn();
                keepScreenOn = value;
            }
        }

        static DisplayInfo GetMainDisplayInfo()
        {
            var display = Platform.MainWindow;
            return new DisplayInfo(
                width: display.ScreenSize.Width,
                height: display.ScreenSize.Height,
                density: display.ScreenDpi.X / (DeviceInfo.Idiom == DeviceIdiom.TV ? 72.0 : 160.0),
                orientation: GetOrientation(),
                rotation: GetRotation());
        }

        static DisplayOrientation GetOrientation()
        {
            switch( Platform.MainWindow.Rotation )
            {
                case 0: return DisplayOrientation.Portrait;
                case 90: return DisplayOrientation.Landscape;
                case 180: return DisplayOrientation.Portrait;
                case 270: return DisplayOrientation.Landscape;
                default: return DisplayOrientation.Unknown;
            };
        }

        static DisplayRotation GetRotation()
        {
            switch( Platform.MainWindow.Rotation)
            {
                case 0 : return DisplayRotation.Rotation0;
                case 90 : return DisplayRotation.Rotation90;
                case 180 : return DisplayRotation.Rotation180;
                case 270 : return DisplayRotation.Rotation270;
                default: return DisplayRotation.Unknown;
            };
        }

        static void StartScreenMetricsListeners()
        {
            Platform.MainWindow.RotationChanged += OnRotationChanged;
        }

        static void StopScreenMetricsListeners()
        {
            Platform.MainWindow.RotationChanged -= OnRotationChanged;
        }

        static void OnRotationChanged(object s, EventArgs e)
        {
            OnMainDisplayInfoChanged(GetMainDisplayInfo());
        }
    }
}
