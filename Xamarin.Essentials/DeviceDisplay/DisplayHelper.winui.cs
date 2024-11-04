using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using System.Threading;

namespace Xamarin.Essentials;

partial class DisplayHelper
{
    [DllImport("User32.dll")]
    static extern uint GetDpiForWindow(IntPtr hwnd);

    public static uint GetCurrentDpi(Window window)
    {
        // Get the window handle (HWND) for the WinUI 3 window.
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        return GetDpiForWindow(hwnd);
    }


    [DllImport("Shcore.dll")]
    static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    [DllImport("User32.dll")]
    static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    public static (uint dpiX, uint dpiY) GetDpiForCurrentMonitor(Window window)
    {
        const int MDT_EFFECTIVE_DPI = 0;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var monitor = MonitorFromWindow(hwnd, 2); // MonitorFromWindow flags: 2 = MONITOR_DEFAULTTONEAREST
        GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);
        return (dpiX, dpiY);
    }

    [DllImport("User32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    public static int GetScreenWidth() => GetSystemMetrics(0); // SM_CXSCREEN
    public static int GetScreenHeight() => GetSystemMetrics(1); // SM_CYSCREEN



    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;

    public static string GetPrimaryMonitorOrientation()
    {
        var screenWidth = GetSystemMetrics(SM_CXSCREEN);
        var screenHeight = GetSystemMetrics(SM_CYSCREEN);

        return screenWidth >= screenHeight ? "Landscape" : "Portrait";
    }

    [DllImport("User32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("User32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public static DisplayInfo GetDisplayInfoForWindow(Window window)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var monitor = MonitorFromWindow(hwnd, 2); // MonitorFromWindow flags: 2 = MONITOR_DEFAULTTONEAREST

        var mi = new MONITORINFO();
        mi.cbSize = Marshal.SizeOf(mi);


        if (GetMonitorInfo(monitor, ref mi))
        {
            var width = mi.rcMonitor.right - mi.rcMonitor.left;
            var height = mi.rcMonitor.bottom - mi.rcMonitor.top;
            var orientation = width >= height ? DisplayOrientation.Landscape : DisplayOrientation.Portrait;

            var dpi = GetDpiForWindow(hwnd);
            var density = dpi / 96.0;

            return new DisplayInfo(width, height, density, orientation, DisplayRotation.Unknown);

        }


        return new DisplayInfo();
    }


}
