using System;

namespace Xamarin.Essentials;

public static partial class DeviceDisplay
{
    static event EventHandler<DisplayInfoChangedEventArgs> MainDisplayInfoChangedInternal;

    static DisplayInfo currentMetrics;

    [Obsolete("Use .RequestActive() and .RequestRelease() from an instance of Windows.System.Display.DisplayRequest() instead.")]
    public static bool KeepScreenOn
    {
        get => PlatformKeepScreenOn;
        set => PlatformKeepScreenOn = value;
    }

    [Obsolete("Use Windows.Graphics.Display.DisplayInformation.GetForCurrentView() instead")]
    public static DisplayInfo MainDisplayInfo => GetMainDisplayInfo();

    static void SetCurrent(DisplayInfo metrics) =>
        currentMetrics = new DisplayInfo(metrics.Width, metrics.Height, metrics.Density, metrics.Orientation, metrics.Rotation, metrics.RefreshRate);

    [Obsolete("Use .DpiChanged and .OrientationChanged in Windows.Graphics.Display.DisplayInformation.GetForCurrentView() instead")]
    public static event EventHandler<DisplayInfoChangedEventArgs> MainDisplayInfoChanged
    {
        add
        {
            var wasRunning = MainDisplayInfoChangedInternal != null;

            MainDisplayInfoChangedInternal += value;

            if (!wasRunning && MainDisplayInfoChangedInternal != null)
            {
                SetCurrent(GetMainDisplayInfo());
                StartScreenMetricsListeners();
            }
        }

        remove
        {
            var wasRunning = MainDisplayInfoChangedInternal != null;

            MainDisplayInfoChangedInternal -= value;

            if (wasRunning && MainDisplayInfoChangedInternal == null)
                StopScreenMetricsListeners();
        }
    }

    static void OnMainDisplayInfoChanged(DisplayInfo metrics)
        => OnMainDisplayInfoChanged(new DisplayInfoChangedEventArgs(metrics));

    static void OnMainDisplayInfoChanged(DisplayInfoChangedEventArgs e)
    {
        if (!currentMetrics.Equals(e.DisplayInfo))
        {
            SetCurrent(e.DisplayInfo);
            MainDisplayInfoChangedInternal?.Invoke(null, e);
        }
    }
}

public class DisplayInfoChangedEventArgs : EventArgs
{
    public DisplayInfoChangedEventArgs(DisplayInfo displayInfo) =>
        DisplayInfo = displayInfo;

    public DisplayInfo DisplayInfo { get; }
}