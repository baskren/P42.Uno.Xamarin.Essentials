using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        static async Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.InitializeWithWindow();

            SetFileTypes(options, picker);

            var resultList = new List<StorageFile>();

            if (allowMultiple)
            {
                var fileList = await picker.PickMultipleFilesAsync();
                if (fileList != null)
                    resultList.AddRange(fileList);
            }
            else
            {
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                    resultList.Add(file);
            }

            foreach (var file in resultList)
                StorageApplicationPermissions.FutureAccessList.Add(file);

            return resultList.Select(storageFile => new FileResult(storageFile));
        }

        static void SetFileTypes(PickOptions options, FileOpenPicker picker)
        {
            var hasAtLeastOneType = false;

            if (options?.FileTypes?.Value != null)
            {
                foreach (var type in options.FileTypes.Value)
                {
                    var ext = FileSystem.Extensions.Clean(type);
                    if (!string.IsNullOrWhiteSpace(ext))
                    {
                        picker.FileTypeFilter.Add(ext);
                        hasAtLeastOneType = true;
                    }
                }
            }

            if (!hasAtLeastOneType)
                picker.FileTypeFilter.Add("*");
        }

        static async Task<string> PlatformExportAsync(byte[] bytes, SaveOptions options)
            => await PlatformExportAsync(options, (writer) => writer.WriteBytes(bytes));

        static async Task<string> PlatformExportAsync(string text, SaveOptions options)
            => await PlatformExportAsync(options, (writer) => writer.WriteString(text));

        static async Task<string> PlatformExportAsync(SaveOptions options, Action<DataWriter> writeAction)
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.InitializeWithWindow();
            if (options != null)
            {
                if (!string.IsNullOrWhiteSpace(options.SuggestedFileName) && System.IO.Path.HasExtension(options.SuggestedFileName))
                {
                    picker.SuggestedFileName = options.SuggestedFileName;
                    var extension = System.IO.Path.GetExtension(options.SuggestedFileName);
                    picker.DefaultFileExtension = extension;
                    picker.FileTypeChoices.Add(options.FileTypeDisplayName ?? extension, new List<string> { extension });
                }
            }
            if (await picker.PickSaveFileAsync() is Windows.Storage.StorageFile windowsFile)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(windowsFile, windowsFile.Path);
                using (var stream = await windowsFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.AllowReadersAndWriters))
                {
                    using (var writer = new DataWriter(stream))
                    {
                        writeAction?.Invoke(writer);
                        await writer.StoreAsync();
                    }
                }
                return windowsFile.Path;
            }
            return null;
        }
    }

    public partial class FilePickerFileType
    {
        static FilePickerFileType PlatformImageFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.UWP, FileSystem.Extensions.AllImage }
            });

        static FilePickerFileType PlatformPngFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.UWP, new[] { FileSystem.Extensions.Png } }
            });

        static FilePickerFileType PlatformJpegFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.UWP, FileSystem.Extensions.AllJpeg }
            });

        static FilePickerFileType PlatformVideoFileType() =>
           new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
           {
                { DevicePlatform.UWP, FileSystem.Extensions.AllVideo }
           });

        static FilePickerFileType PlatformPdfFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.UWP, new[] { FileSystem.Extensions.Pdf } }
            });
    }
}
