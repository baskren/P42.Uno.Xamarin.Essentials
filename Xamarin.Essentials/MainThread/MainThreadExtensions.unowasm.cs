using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;

namespace Xamarin.Essentials
{
    internal static partial class MainThreadExtensions
    {
        internal static void WatchForError(this UIAsyncOperation self)
            =>  self.CompletionSource.Task.WatchForError();
        

        internal static void WatchForError(this Task self)
        {
            
            var context = SynchronizationContext.Current;
            if (context == null)
                return;

            self.ContinueWith(
                t =>
                {
                    var exception = t.Exception.InnerExceptions.Count > 1 ? t.Exception : t.Exception.InnerException;

                    context.Post(e => { throw (Exception)e; }, exception);
                }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            
        }
    }
}
