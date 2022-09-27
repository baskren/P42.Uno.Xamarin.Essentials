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
        }

    }
}
