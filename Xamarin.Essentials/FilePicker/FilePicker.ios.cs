using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;
using UIKit;

#pragma warning disable CA1422 // Call site reachable on all platforms
namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        static async Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        {
            var allowedUtis = options?.FileTypes?.Value?.ToArray() ?? new string[]
            {
                UTType.Content,
                UTType.Item,
                "public.data"
            };

            var tcs = new TaskCompletionSource<IEnumerable<FileResult>>();

            // Use Open instead of Import so that we can attempt to use the original file.
            // If the file is from an external provider, then it will be downloaded.
            using var documentPicker = new UIDocumentPickerViewController(allowedUtis, UIDocumentPickerMode.Open);
            if (Platform.HasOSVersion(11, 0))
                documentPicker.AllowsMultipleSelection = allowMultiple;
            documentPicker.Delegate = new PickerDelegate
            {
                PickHandler = urls => GetFileResults(urls, tcs)
            };

            if (documentPicker.PresentationController != null)
            {
                documentPicker.PresentationController.Delegate = new Platform.UIPresentationControllerDelegate
                {
                    DismissHandler = () => GetFileResults(null, tcs)
                };
            }

            var parentController = Platform.GetCurrentViewController();

            parentController.PresentViewController(documentPicker, true, null);

            return await tcs.Task;
        }

        static async void GetFileResults(NSUrl[] urls, TaskCompletionSource<IEnumerable<FileResult>> tcs)
        {
            try
            {
                var results = await FileSystem.EnsurePhysicalFileResultsAsync(urls);

                tcs.TrySetResult(results);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        class PickerDelegate : UIDocumentPickerDelegate
        {
            public Action<NSUrl[]> PickHandler { get; set; }

            public override void WasCancelled(UIDocumentPickerViewController controller)
                => PickHandler?.Invoke(null);

            public override void DidPickDocument(UIDocumentPickerViewController controller, NSUrl[] urls)
                => PickHandler?.Invoke(urls);

            public override void DidPickDocument(UIDocumentPickerViewController controller, NSUrl url)
                => PickHandler?.Invoke(new NSUrl[] { url });
        }

        class PickerPresentationControllerDelegate : UIAdaptivePresentationControllerDelegate
        {
            public Action<NSUrl[]> PickHandler { get; set; }

            public override void DidDismiss(UIPresentationController presentationController) =>
                PickHandler?.Invoke(null);
        }

        static string GetTmpDir()
        {
            if (!System.IO.Directory.Exists(FileSystem.CacheDirectory))
                System.IO.Directory.CreateDirectory(FileSystem.CacheDirectory);
            var tmpDir = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString());
            if (!System.IO.Directory.Exists(tmpDir))
                System.IO.Directory.CreateDirectory(tmpDir);
            return tmpDir;
        }

        static async Task<string> PlatformExportAsync(byte[] bytes, SaveOptions options)
        {
            var tmpPath = Path.Combine(GetTmpDir(), options?.SuggestedFileName ?? "data.bin");
            await File.WriteAllBytesAsync(tmpPath, bytes);
            using (var exportUrl = new NSUrl(new System.Uri(tmpPath).AbsoluteUri))
            {
                var result = await PlatformExportAsync(exportUrl, options);
                return result;
            }
        }

        static async Task<string> PlatformExportAsync(string text, SaveOptions options)
        {
            var tmpPath = Path.Combine(GetTmpDir(), options?.SuggestedFileName ?? "data.txt");
            await File.WriteAllTextAsync(tmpPath, text);
            using (var exportUrl = new NSUrl(new System.Uri(tmpPath).AbsoluteUri))
            {
                return await PlatformExportAsync(exportUrl, options);
            }
        }

        static async Task<string> PlatformExportAsync(NSUrl exportUrl, SaveOptions options)
        {
            // get directory into which the file will be placed

            var tcs = new TaskCompletionSource<IEnumerable<FileResult>>();

            // using (var documentPicker = new UIDocumentPickerViewController(exportUrl, UIDocumentPickerMode.MoveToService)
            // using (var documentPicker = new UIDocumentPickerViewController(exportUrl, UIDocumentPickerMode.ExportToService)
            using (var documentPicker = new UIDocumentPickerViewController(new string[] { UTType.Folder }, UIDocumentPickerMode.Open)
            {
                Delegate = new PickerDelegate
                {
                    PickHandler = urls => GetFileResults(urls, tcs)
                }
            })
            {
                if (documentPicker.PresentationController != null)
                {
                    documentPicker.PresentationController.Delegate = new PickerPresentationControllerDelegate
                    {
                        PickHandler = urls => GetFileResults(urls, tcs)
                    };
                }

                var parentController = Platform.GetCurrentViewController();
                parentController.PresentViewController(documentPicker, true, null);

                if ((await tcs.Task).FirstOrDefault() is FileResult result)
                {
                    var fileName = Path.GetFileNameWithoutExtension(exportUrl.Path);
                    var extension = Path.GetExtension(exportUrl.Path);
                    var destFileName = Path.Combine(result.Url.Path, fileName + (string.IsNullOrWhiteSpace(extension) ? null : extension));

                    var attempt = 0;
                    while (NSFileManager.DefaultManager.FileExists(destFileName))
                    {
                        attempt++;
                        destFileName = Path.Combine(result.Url.Path, fileName + "(" + attempt + ")" + (string.IsNullOrWhiteSpace(extension) ? null : extension));
                    }

                    // var destUrl = new NSUrl(destFileName);
                    NSFileManager.DefaultManager.Move(exportUrl.Path, destFileName, out var error);

                    if (error != null)
                        throw new Exception(error.LocalizedDescription);

                    return destFileName;
                }

                return null;
            }
        }
    }

    public partial class FilePickerFileType
    {
        static FilePickerFileType PlatformImageFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { (string)UTType.Image } }
            });

        static FilePickerFileType PlatformPngFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { (string)UTType.PNG } }
            });

        static FilePickerFileType PlatformJpegFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { (string)UTType.JPEG } }
            });

        static FilePickerFileType PlatformVideoFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new string[] { UTType.MPEG4, UTType.Video, UTType.AVIMovie, UTType.AppleProtectedMPEG4Video, "mp4", "m4v", "mpg", "mpeg", "mp2", "mov", "avi", "mkv", "flv", "gifv", "qt" } }
            });

        static FilePickerFileType PlatformPdfFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { (string)UTType.PDF } }
            });
    }
}
#pragma warning restore CA1422 // Call site reachable on all platforms
