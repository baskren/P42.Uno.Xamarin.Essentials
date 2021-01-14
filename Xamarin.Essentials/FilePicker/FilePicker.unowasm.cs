using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        static async Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        {
            System.Diagnostics.Debug.WriteLine("FilePicker. options.Title: [" + options.PickerTitle + "]");
            System.Diagnostics.Debug.WriteLine("FilePicker. options.FileTypes: [" + string.Join(", ", options.FileTypes) + "]");
            var jsonOptions = JsonConvert.SerializeObject(options);
            System.Diagnostics.Debug.WriteLine("FilePicker.PlatformPickAsync: jsonOptions: " + jsonOptions);
            var javascript = $"UnoFilePicker_Pick('{jsonOptions}', {allowMultiple.ToString().ToLower()})";
            System.Diagnostics.Debug.WriteLine("FilePicker.PlatformPickAsync: javascript: " + javascript);

            var jsonResult = await WebAssemblyRuntime.InvokeAsync(javascript);
            System.Diagnostics.Debug.WriteLine("FilePicker.result: " + jsonResult);

            var payload = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(jsonResult);

            var results = new List<FileResult>();
            var files = payload["Files"];
            foreach (var file in files)
            {
                if (file.TryGetValue("FullPath", out var path))
                {
                    System.Diagnostics.Debug.WriteLine("FilePicker. path [" + path + "]");
                    var storageFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
                    var fileResult = new FileResult(storageFile);
                    System.Diagnostics.Debug.WriteLine($"FilePicker fileResult: " + fileResult);
                    results.Add(fileResult);
                }
            }
            return results;
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
