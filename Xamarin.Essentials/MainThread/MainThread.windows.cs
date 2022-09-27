﻿using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Xamarin.Essentials
{
    public static partial class MainThread
    {
        static bool PlatformIsMainThread
        {
            get
            {
                // if there is no main window, then this is either a service
                // or the UI is not yet constructed, so the main thread is the
                // current thread
                try
                {
                    if (Platform.MainThreadDispatchQueue == null)
                        return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to validate Window creation. {ex.Message}");
                    return true;
                }

                return Platform.MainThreadDispatchQueue?.HasThreadAccess ?? false;
            }
        }

        static void PlatformBeginInvokeOnMainThread(Action action)
        {
            /*
            if (CoreApplication.MainView?.CoreWindow?.Dispatcher is CoreDispatcher dispatcher)
                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).WatchForError();
            else
                action.Invoke();
            */
            if (action is null)
                return;

            if (IsMainThread)
                action.Invoke();
            else
                Platform.MainThreadDispatchQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, ()=>action.Invoke());
        }
    }
}
