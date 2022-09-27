using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Essentials
{
    public static partial class MainThread
    {
        public static bool IsMainThread
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

        public static void BeginInvokeOnMainThread(Action action)
        {
            if (action is null)
                return;

            if (IsMainThread)
                action.Invoke();
            else
                Platform.MainThreadDispatchQueue.TryEnqueue(() => action.Invoke());
        }

        public static Task InvokeOnMainThreadAsync(Action action)
        {
            if (action is null)
                return Task.CompletedTask;

            if (IsMainThread)
            {
                action();
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>();

            Platform.MainThreadDispatchQueue.TryEnqueue(() =>
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
        {
            if (func is null)
                return Task.FromResult(default(T));

            if (IsMainThread)
                return Task.FromResult(func());

            var tcs = new TaskCompletionSource<T>();
            Platform.MainThreadDispatchQueue.TryEnqueue(() =>
            {
                try
                {
                    var result = func();
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }

        public static Task InvokeOnMainThreadAsync(Func<Task> funcTask)
        {
            if (funcTask is null)
                return Task.CompletedTask;

            if (IsMainThread)
                return funcTask();

            var tcs = new TaskCompletionSource<object>();
            Platform.MainThreadDispatchQueue.TryEnqueue(
                async () =>
                {
                    try
                    {
                        await funcTask().ConfigureAwait(false);
                        tcs.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
        {
            if (funcTask is null)
                return Task.FromResult(default(T));

            if (IsMainThread)
                return funcTask();

            var tcs = new TaskCompletionSource<T>();
            Platform.MainThreadDispatchQueue.TryEnqueue(
                async () =>
                {
                    try
                    {
                        var ret = await funcTask().ConfigureAwait(false);
                        tcs.SetResult(ret);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }

    }
}
