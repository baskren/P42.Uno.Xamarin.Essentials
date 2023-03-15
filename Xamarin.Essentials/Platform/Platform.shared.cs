using System;
using System.Threading;
using Microsoft.UI.Dispatching;

namespace Xamarin.Essentials
{
    public static partial class Platform
    {
        public static Microsoft.UI.Xaml.Application Application { get; private set; }

        static Microsoft.UI.Xaml.Window mainWindow;

        public static Microsoft.UI.Xaml.Window MainWindow
        {
            get
            {
                if (mainWindow != null)
                    return mainWindow;
                var message =
                    "Cannot show Xamarin.Essentials Share UI unless Xamarin.Essentials is first initialized using .Platform.Init(Application, Window)";
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => throw new InvalidOperationException(message));
                throw new InvalidOperationException(message);
            }
            private set => mainWindow = value;
        }

        public static Thread MainThread { get; private set; }

        public static DispatcherQueue MainThreadDispatchQueue { get; private set; }

        public static void Init(Microsoft.UI.Xaml.Application application, Microsoft.UI.Xaml.Window window)
        {
            Application = application;
            MainWindow = window;
            MainThread = Thread.CurrentThread;
            MainThreadDispatchQueue = DispatcherQueue.GetForCurrentThread();

#if __ANDROID__
            var propertyInfo = typeof(Microsoft.UI.Xaml.ApplicationActivity).GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var obj = propertyInfo.GetValue(null, null);
            if (obj is Microsoft.UI.Xaml.ApplicationActivity activity) 
            {
                Xamarin.Essentials.Platform.Init(activity, null);
            }
#endif
        }
    }
}
