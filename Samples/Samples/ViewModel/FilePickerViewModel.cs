using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class FilePickerViewModel : BaseViewModel
    {
        string text;
        ImageSource image;
        bool isImageVisible;
        FileResult lastSingleFileResult;

        public FilePickerViewModel()
        {
            PickFileCommand = new Command(() => DoPickFile());
            PickImageCommand = new Command(() => DoPickImage());
            PickPdfCommand = new Command(() => DoPickPdf());
            PickCustomTypeCommand = new Command(() => DoPickCustomType());
            PickAndSendCommand = new Command(() => DoPickAndSend());
            PickMultipleFilesCommand = new Command(() => DoPickMultipleFiles());
        }

        public ICommand PickFileCommand { get; }

        public ICommand PickImageCommand { get; }

        public ICommand PickPdfCommand { get; }

        public ICommand PickCustomTypeCommand { get; }

        public ICommand PickAndSendCommand { get; }

        public ICommand PickMultipleFilesCommand { get; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        public FileResult LastSingleFileResult 
        { 
            get => lastSingleFileResult;
            set => SetProperty(ref lastSingleFileResult, value); 
        }

        public bool IsImageVisible
        {
            get => isImageVisible;
            set => SetProperty(ref isImageVisible, value);
        }

        async void DoPickFile()
        {
            await PickAndShow(PickOptions.Default);
        }

        async void DoPickImage()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images,
            };

            await PickAndShow(options);
        }

        async void DoPickPdf()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a pdf",
                FileTypes = FilePickerFileType.Pdf,
            };

            await PickAndShow(options);
        }

        async void DoPickCustomType()
        {
            var customFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "application/comics" } },
                    { DevicePlatform.UWP, new[] { ".cbr", ".cbz" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // or general UTType values
                });

            var options = new PickOptions
            {
                PickerTitle = "Please select a comic file",
                FileTypes = customFileType,
            };

            await PickAndShow(options);
        }

        async void DoPickAndSend()
        {
            // pick a file
            LastSingleFileResult = await PickAndShow(PickOptions.Images);
            if (LastSingleFileResult == null)
                return;

            // copy it locally
            var copyPath = Path.Combine(FileSystem.CacheDirectory, LastSingleFileResult.FileName);
            using (var destination = File.Create(copyPath))
            using (var source = await LastSingleFileResult.OpenReadAsync())
                await source.CopyToAsync(destination);

            // send it via an email
            await Email.ComposeAsync(new EmailMessage
            {
                Subject = "Test Subject",
                Body = "This is the body. There should be an image attached.",
                Attachments =
                {
                    new EmailAttachment(copyPath)
                }
            });
        }

        async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                LastSingleFileResult = await FilePicker.PickAsync(options);

                if (LastSingleFileResult != null)
                {
                    var size = await GetStreamSizeAsync(LastSingleFileResult);

                    Text = $"File Name: {LastSingleFileResult.FileName} ({size:0.00} KB)";

                    var ext = Path.GetExtension(LastSingleFileResult.FileName).ToLowerInvariant();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                    {
#if UNO_PLATFORM
                        var asm = GetType().Assembly;
                        /*
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel. asm:");
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel.   .CodeBase: " + asm.CodeBase);
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel.   .FullName: " + asm.FullName);
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel.   .ImageRuntimeVersion: " + asm.ImageRuntimeVersion);
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel.   .CustomAttributes");
                        foreach (var attribute in asm.CustomAttributes)
                        {
                            System.Diagnostics.Debug.WriteLine("FilePickerViewModel.          " + attribute);
                        }
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel.        ");
                        var asmName = asm.FullName.Split(',')[0].Trim();
                        var resourceId = asmName + ".Assets.doodle.png";
                        System.Diagnostics.Debug.WriteLine("FilePickerViewModel. resourceId: " + resourceId);

                        var resources = GetType().Assembly.GetManifestResourceNames();
                        foreach (var resource in resources)
                            System.Diagnostics.Debug.WriteLine("                " + resource);

                        using (var aStream = GetType().Assembly.GetManifestResourceStream(resourceId))
                        {
                            System.Diagnostics.Debug.WriteLine("FilePickerViewModel. FROM /ASSETS/doodle.png");
                            var log = "";
                            for (var i = 0; i < 16; i++)
                            {
                                for (var j = 0; j < 4; j++)
                                {
                                    var buf = new byte[4];
                                    aStream.Read(buf, 0, 4);
                                    foreach (var b in buf)
                                        log += b.ToString("X2");
                                    log += " ";
                                }
                                log += "\n";
                            }
                            System.Diagnostics.Debug.WriteLine(log);
                        }
                        */

                        //System.Diagnostics.Debug.WriteLine("FilePickerViewModel. IS IMAGE");
                        using (var stream = await LastSingleFileResult.OpenReadAsync())
                        {
                            /*
                            var log = "";
                            for (var i = 0; i < 16; i++)
                            {
                                for (var j = 0; j < 4; j++)
                                {
                                    var buf = new byte[4];
                                    stream.Read(buf, 0, 4);
                                    foreach (var b in buf)
                                        log += b.ToString("X2");
                                    log += " ";
                                }
                                log += "\n";
                            }
                            System.Diagnostics.Debug.WriteLine(log);
                            */
                            stream.Position = 0;

                            var randomStream = stream.AsRandomAccessStream();
                            var imageResult = new ImageSource();
                            await imageResult.SetSourceAsync(randomStream);
                            Image = imageResult;
                            IsImageVisible = true;
                        }
#else
                        using (var stream = await LastSingleFileResult.OpenReadAsync())
                        {
                            Image = ImageSource.FromStream(() => stream);
                            IsImageVisible = true;
                        }
#endif
                    }
                    else
                    {
                        IsImageVisible = false;
                    }
                }
                else
                {
                    Text = $"Pick cancelled.";
                }

                return LastSingleFileResult;
            }
            catch (Exception ex)
            {
                Text = ex.ToString();
                IsImageVisible = false;
                return null;
            }
        }

        async Task<double> GetStreamSizeAsync(FileResult result)
        {
            try
            {
                using var stream = await result.OpenReadAsync();
                return stream.Length / 1024.0;
            }
            catch
            {
                return 0.0;
            }
        }

        async void DoPickMultipleFiles()
        {
            try
            {
                var resultList = await FilePicker.PickMultipleAsync();

                if (resultList != null && resultList.Any())
                {
                    LastSingleFileResult = null;
                    Text = "File Names: " + string.Join(", ", resultList.Select(result => result.FileName));

                    // only showing the first file's content
                    var firstResult = resultList.First();

                    if (firstResult.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        firstResult.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
#if UNO_PLATFORM
                        if (await firstResult.OpenReadAsync() is IRandomAccessStream stream)
                        {
                            var imageResult = new BitmapImage();
                            await imageResult.SetSourceAsync(stream);
                            Image = imageResult;
                            IsImageVisible = true;
                        }
#else
                        var stream = await firstResult.OpenReadAsync();
                        Image = ImageSource.FromStream(() => stream);
                        IsImageVisible = true;
#endif
                    }
                    else
                    {
                        IsImageVisible = false;
                    }
                }
                else
                {
                    Text = $"Pick cancelled.";
                    IsImageVisible = false;
                }
            }
            catch (Exception ex)
            {
                Text = ex.ToString();
                IsImageVisible = false;
            }
        }
    }
}
