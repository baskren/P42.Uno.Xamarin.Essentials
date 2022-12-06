using System.Threading;
using Microsoft.UI.Dispatching;

namespace Xamarin.Essentials
{
    public static partial class Platform
    {
        public static Microsoft.UI.Xaml.Application Application { get; private set; }

        public static Microsoft.UI.Xaml.Window Window { get; private set; }

        public static Thread MainThread { get; private set; }

        public static DispatcherQueue MainThreadDispatchQueue { get; private set; }

        public static void Init(Microsoft.UI.Xaml.Application application, Microsoft.UI.Xaml.Window window)
        {
            Application = application;
            Window = window;
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
