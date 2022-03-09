using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        static async Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        {
            var jsonOptions = JsonConvert.SerializeObject(options);
            var javascript = $"UnoFilePicker_Pick('{jsonOptions}', {allowMultiple.ToString().ToLower()})";
            var jsonResult = await WebAssemblyRuntime.InvokeAsync(javascript);

            var payload = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(jsonResult);

            var results = new List<FileResult>();
            var files = payload["Files"];
            foreach (var file in files)
            {
                if (file.TryGetValue("FullPath", out var path))
                {
                    var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
                    var fileResult = new FileResult(storageFile);
                    results.Add(fileResult);
                }
            }
            return results;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async Task<string> PlatformExportAsync(byte[] bytes, SaveOptions options)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var path = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, Guid.NewGuid().ToString());
            File.WriteAllBytes(path, bytes);

            var contentType = options?.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = FileSystem.MimeTypes.OctetStream;

            var shareFile = new ShareFile(path, contentType);
            var json = JsonConvert.SerializeObject(shareFile);

            var fileName = options?.SuggestedFileName;
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "data.bin";
            WebAssemblyRuntime.InvokeJS($"UnoFilePicker_Export('{json}', '{fileName}')");
            return fileName;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async Task<string> PlatformExportAsync(string text, SaveOptions options)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // System.Diagnostics.Debug.WriteLine("FilePicker. TEXT");
            var path = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, Guid.NewGuid().ToString());
            File.WriteAllText(path, text);

            var contentType = options?.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = FileSystem.MimeTypes.TextPlain;

            var shareFile = new ShareFile(path, contentType);
            var json = JsonConvert.SerializeObject(shareFile);

            var fileName = options?.SuggestedFileName;
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "data.text";
            WebAssemblyRuntime.InvokeJS($"UnoFilePicker_Export('{json}', '{fileName}')");
            return fileName;
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
