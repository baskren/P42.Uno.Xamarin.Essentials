using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Xamarin.Essentials;
using ImageSource = Microsoft.UI.Xaml.Media.Imaging.BitmapImage;

namespace Samples.ViewModel
{
    class ScreenshotViewModel : BaseViewModel
    {
        ImageSource screenshot;

        public ScreenshotViewModel()
        {
            ScreenshotCommand = new Command(async () => await CaptureScreenshot(), () => Screenshot.IsCaptureSupported);
            EmailCommand = new Command(async () => await EmailScreenshot(), () => Screenshot.IsCaptureSupported);
        }

        public ICommand ScreenshotCommand { get; }

        public ICommand EmailCommand { get; }

        public ImageSource Image
        {
            get => screenshot;
            set => SetProperty(ref screenshot, value);
        }

        async Task CaptureScreenshot()
        {
            var mediaFile = await Screenshot.CaptureAsync();

            if (await mediaFile.OpenReadAsync(ScreenshotFormat.Png) is Stream stream)
            {
                var imageResult = new BitmapImage();
                var randomStream = stream.AsRandomAccessStream();
                await imageResult.SetSourceAsync(randomStream);
                Image = imageResult;
            }
        }

        async Task EmailScreenshot()
        {
            var mediaFile = await Screenshot.CaptureAsync();

            var temp = Path.Combine(FileSystem.CacheDirectory, "screenshot.jpg");
            using (var stream = await mediaFile.OpenReadAsync(ScreenshotFormat.Jpeg))
            {
                using (var file = File.Create(temp))
                {
                    await stream.CopyToAsync(file);
                }
            }
            await Email.ComposeAsync(new EmailMessage
            {
                Attachments = { new EmailAttachment(temp) }
            });
        }
    }
}
