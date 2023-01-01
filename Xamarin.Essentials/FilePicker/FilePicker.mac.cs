using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using MobileCoreServices;

namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        static Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        {
            var openPanel = new NSOpenPanel
            {
                CanChooseFiles = true,
                AllowsMultipleSelection = allowMultiple,
                CanChooseDirectories = false
            };

            if (options.PickerTitle != null)
                openPanel.Title = options.PickerTitle;

            SetFileTypes(options, openPanel);

            var resultList = new List<FileResult>();
            var panelResult = openPanel.RunModal();
            if (panelResult == (nint)(long)NSModalResponse.OK)
            {
                foreach (var url in openPanel.Urls)
                    resultList.Add(new FileResult(url.Path));
            }

            return Task.FromResult<IEnumerable<FileResult>>(resultList);
        }

        static void SetFileTypes(PickOptions options, NSOpenPanel panel)
        {
            var allowedFileTypes = new List<string>();

            if (options?.FileTypes?.Value != null)
            {
                foreach (var type in options.FileTypes.Value)
                {
                    allowedFileTypes.Add(type.TrimStart('*', '.'));
                }
            }

            panel.AllowedFileTypes = allowedFileTypes.ToArray();
        }

        static async Task<string> PlatformExportAsync(byte[] bytes, SaveOptions options)
            => await PlatformExportAsync(options, async (url) => await System.IO.File.WriteAllBytesAsync(url.Path, bytes));

        static async Task<string> PlatformExportAsync(string text, SaveOptions options)
            => await PlatformExportAsync(options, async (url) => await System.IO.File.WriteAllTextAsync(url.Path, text));

        static async Task<string> PlatformExportAsync(SaveOptions options, Func<NSUrl, Task> writeAction)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (var folderUrl = new NSUrl(folderPath))
            {
                using (var panel = new NSOpenPanel
                {
                    CanCreateDirectories = true,
                    CanChooseDirectories = false,
                    CanChooseFiles = true,
                    FloatingPanel = true,
                    AllowsMultipleSelection = false,
                    ResolvesAliases = true,
                    DirectoryUrl = folderUrl,

                    // Prompt = "THIS IS THE PROMPT!",
                    Title = options?.PickerTitle ?? string.Empty,
                })
                {
                    // if (!string.IsNullOrWhiteSpace(message))
                    //    panel.Message = message;

                    if (panel.RunModal(folderUrl.Path, options?.SuggestedFileName, new string[] { options?.ContentType ?? FileSystem.MimeTypes.TextPlain }) == 1)
                    {
                        await writeAction?.Invoke(panel.Url);
                        return panel.Url.Path;
                    }
                }
            }
            return null;
        }
    }

    public partial class FilePickerFileType
    {
        static FilePickerFileType PlatformImageFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.macOS, new string[] { UTType.PNG, UTType.JPEG, "jpeg" } }
            });

        static FilePickerFileType PlatformPngFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.macOS, new string[] { UTType.PNG } }
            });

        static FilePickerFileType PlatformJpegFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.macOS, new string[] { UTType.JPEG } }
            });

        static FilePickerFileType PlatformVideoFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.macOS, new string[] { UTType.MPEG4, UTType.Video, UTType.AVIMovie, UTType.AppleProtectedMPEG4Video, "mp4", "m4v", "mpg", "mpeg", "mp2", "mov", "avi", "mkv", "flv", "gifv", "qt" } }
            });

        static FilePickerFileType PlatformPdfFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.macOS, new string[] { UTType.PDF } }
            });
    }
}
