using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        public static async Task<FileResult> PickAsync(PickOptions options = null) =>
            (await PlatformPickAsync(options))?.FirstOrDefault();

        public static Task<IEnumerable<FileResult>> PickMultipleAsync(PickOptions options = null) =>
            PlatformPickAsync(options ?? PickOptions.Default, true);

        public static Task<string> ExportAsync(string text, SaveOptions options = null) =>
            PlatformExportAsync(text, options);

        public static Task<string> ExportAsync(byte[] bytes, SaveOptions options = null) =>
            PlatformExportAsync(bytes, options);

        public static async Task<string> ExportAsync(FileBase fileBase, SaveOptions options = null)
        {
            options = options ?? new SaveOptions();
            options.ContentType = fileBase.ContentType;
            if (string.IsNullOrWhiteSpace(options.SuggestedFileName))
                options.SuggestedFileName = fileBase.FileName;
            using (var stream = await fileBase.OpenReadAsync())
            {
                if (fileBase.ContentType.StartsWith("text"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var text = reader.ReadToEnd();
                        return await ExportAsync(text, options);
                    }
                }
                else
                {
                    var bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    return await ExportAsync(bytes, options);
                }
            }
        }

    }

    public partial class FilePickerFileType
    {
        public static readonly FilePickerFileType Images = PlatformImageFileType();
        public static readonly FilePickerFileType Png = PlatformPngFileType();
        public static readonly FilePickerFileType Jpeg = PlatformJpegFileType();
        public static readonly FilePickerFileType Videos = PlatformVideoFileType();
        public static readonly FilePickerFileType Pdf = PlatformPdfFileType();

        readonly IDictionary<DevicePlatform, IEnumerable<string>> fileTypes;

        protected FilePickerFileType()
        {
        }

        public FilePickerFileType(IDictionary<DevicePlatform, IEnumerable<string>> fileTypes) =>
            this.fileTypes = fileTypes;

        public IEnumerable<string> Value => GetPlatformFileType(DeviceInfo.Platform);

        protected virtual IEnumerable<string> GetPlatformFileType(DevicePlatform platform)
        {
            if (fileTypes.TryGetValue(platform, out var type))
                return type;

            throw new PlatformNotSupportedException("This platform does not support this file type.");
        }
    }

    public class PickOptions
    {
        public static PickOptions Default =>
            new PickOptions
            {
                FileTypes = null,
            };

        public static PickOptions Images =>
            new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            };

        public string PickerTitle { get; set; }

        public FilePickerFileType FileTypes { get; set; }

        public override string ToString()
        {
            return "{ PickerTitle: " + PickerTitle + ", FileTypes: [" + string.Join(", ", FileTypes) + "] }";
        }
    }

    public class SaveOptions
    {
        public string PickerTitle { get; set; }

        public string SuggestedFileName { get; set; }

        public string ContentType { get; set; }

        public string FileTypeDisplayName { get; set; }

    }


}
