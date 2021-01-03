using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
#if UNO_PLATFORM
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using ImageSource = Windows.UI.Xaml.Media.Imaging.BitmapImage;
#endif

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
#if UNO_PLATFORM
            if (await mediaFile.OpenReadAsync(ScreenshotFormat.Png) is IRandomAccessStream stream)
            {
                var imageResult = new BitmapImage();
                await (Task)imageResult.SetSourceAsync(stream);
                Image = imageResult;
            }
#else
            var stream = await mediaFile.OpenReadAsync(ScreenshotFormat.Png);

            Image = ImageSource.FromStream(() => stream);
#endif
        }

        async Task EmailScreenshot()
        {
            var mediaFile = await Screenshot.CaptureAsync();

            var temp = Path.Combine(FileSystem.CacheDirectory, "screenshot.jpg");
            using (var stream = await mediaFile.OpenReadAsync(ScreenshotFormat.Jpeg))
            using (var file = File.Create(temp))
            {
                await stream.CopyToAsync(file);
            }

            await Email.ComposeAsync(new EmailMessage
            {
                Attachments = { new EmailAttachment(temp) }
            });
        }
    }
}
