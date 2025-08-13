using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Net.IpSec.Ike.Exceptions;
using Android.OS;
using Android.Util;
using Android.Views;

namespace Xamarin.Essentials
{
    public static partial class Screenshot
    {
        static bool PlatformIsCaptureSupported =>
            true;

        static async Task<ScreenshotResult> PlatformCaptureAsync()
        {
            if (Platform.WindowManager?.DefaultDisplay?.Flags.HasFlag(DisplayFlags.Secure) == true)
                throw new UnauthorizedAccessException("Unable to take a screenshot of a secure window.");

            //var view = Platform.GetCurrentActivity(true)?.Window?.DecorView?.RootView;
            
            if ( Platform.GetCurrentActivity(true)?.Window?.DecorView?.RootView is not View view)
                throw new NullReferenceException("Unable to find the main window.");

            /*
            var bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);

            using (var canvas = new Canvas(bitmap))
            {
                var drawable = view.Background;
                if (drawable != null)
                    drawable.Draw(canvas);
                else
                    canvas.DrawColor(Color.White);

                view.Draw(canvas);
            }
            */
            
            var bitmap = await CaptureViewScreenShot(Platform.CurrentActivity, view);

            var result = new ScreenshotResult(bitmap);

            return result;
        }
        
        static async Task<Bitmap> CaptureViewScreenShot(global::Android.App.Activity activity, View targetView)
        {
            // Get the view's rectangle within the window
            var location = new int[2];
            targetView.GetLocationInWindow(location);
            /*
            var sourceRect = new Rect(location[0], location[1], location[0] + targetView.Width, location[1] + targetView.Height);

            // Create a bitmap to store the screenshot
            var bitmap = Bitmap.CreateBitmap(sourceRect.Width(), sourceRect.Height(), Bitmap.Config.Argb8888);

            // Create a handler for the main looper to receive the callback on the UI thread
            var handler = new global::Android.OS.Handler(global::Android.OS.Looper.MainLooper);

            // Create the PixelCopy request
            var request = global::Android.Views.PixelCopy.PixelCopyRequest.Builder.OfWindow(activity.Window)
                .SetSourceRect(sourceRect)
                .SetDestinationBitmap(bitmap)
                .Build();

            // Perform the pixel copy
            PixelCopy.Request(request, handler, new PixelCopyFinishedListener(result =>
            {
                if (result == (int)PixelCopy.Success)
                {
                    // Screenshot successful, 'bitmap' now contains the image
                    // You can now save, display, or process the bitmap
                    Console.WriteLine("Screenshot taken successfully!");
                }
                else
                {
                    Console.WriteLine($"Screenshot failed with error: {result}");
                }
                bitmap.Recycle(); // Release bitmap resources when done
            }));
            */

            var displayMetrics = new DisplayMetrics();
            activity.WindowManager.DefaultDisplay.GetRealMetrics(displayMetrics);
            var bitmap = Bitmap.CreateBitmap
            (
                displayMetrics.WidthPixels,
                displayMetrics.HeightPixels,
                Bitmap.Config.Argb8888
            );

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                throw new Exception("PixelCopy requires API 26+");

            var window = activity.Window;
            var locationInWindow = new int[2];
            window.DecorView.GetLocationInWindow(locationInWindow);
            
            var tcs = new TaskCompletionSource<PixelCopyResult>();
            var handler = new FinishedListener(tcs);
            
            PixelCopy.Request(
                window,
                bitmap,
                handler,
                new Handler(Looper.MainLooper)
            );
            
            await tcs.Task;
            return bitmap;
        }

    }
    
    class FinishedListener(TaskCompletionSource<PixelCopyResult> completionSource)
        : global::Java.Lang.Object, PixelCopy.IOnPixelCopyFinishedListener
    {
        TaskCompletionSource<PixelCopyResult> CompletionSource = completionSource;

        public void OnPixelCopyFinished(int copyResult) => CompletionSource.SetResult((PixelCopyResult)copyResult);
    }


    public partial class ScreenshotResult
    {
        readonly Bitmap bmp;

        internal ScreenshotResult(Bitmap bmp)
            : base()
        {
            this.bmp = bmp;

            Width = bmp.Width;
            Height = bmp.Height;
        }

        internal async Task<Stream> PlatformOpenReadAsync(ScreenshotFormat format)
        {
            var stream = new MemoryStream();

            var f = format switch
            {
                ScreenshotFormat.Jpeg => Bitmap.CompressFormat.Jpeg,
                _ => Bitmap.CompressFormat.Png,
            };

            await bmp.CompressAsync(f, 100, stream).ConfigureAwait(false);
            stream.Position = 0;

            return stream;
        }
    }
    
    
}

