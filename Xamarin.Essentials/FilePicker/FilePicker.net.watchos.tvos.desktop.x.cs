using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xamarin.Essentials;

public static partial class FilePicker
{
    static Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        => throw new NotImplementedInReferenceAssemblyException();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    static async Task<string> PlatformExportAsync(byte[] bytes, SaveOptions options)
        => throw new NotImplementedInReferenceAssemblyException();

    static async Task<string> PlatformExportAsync(string text, SaveOptions options)
        => throw new NotImplementedInReferenceAssemblyException();
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}

public partial class FilePickerFileType
{
    static FilePickerFileType PlatformImageFileType()
        => throw new NotImplementedInReferenceAssemblyException();

    static FilePickerFileType PlatformPngFileType()
        => throw new NotImplementedInReferenceAssemblyException();

    static FilePickerFileType PlatformJpegFileType()
        => throw new NotImplementedInReferenceAssemblyException();

    static FilePickerFileType PlatformVideoFileType()
        => throw new NotImplementedInReferenceAssemblyException();

    static FilePickerFileType PlatformPdfFileType()
        => throw new NotImplementedInReferenceAssemblyException();
}