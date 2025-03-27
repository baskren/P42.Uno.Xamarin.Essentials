using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Xamarin.Essentials;

public static partial class FileSystem
{
    static string PlatformCacheDirectory
        => ApplicationData.Current.LocalCacheFolder.Path;

    static string PlatformAppDataDirectory
        => ApplicationData.Current.LocalFolder.Path;

    static Task<Stream> PlatformOpenAppPackageFileAsync(string filename)
    {
        System.Diagnostics.Debug.WriteLine("FileSystem.PlatformOpenAppPackageFileAsync UWP ENTER");
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));

        return Package.Current.InstalledLocation.OpenStreamForReadAsync(NormalizePath(filename));
    }

    internal static string NormalizePath(string path)
        => path.Replace('/', Path.DirectorySeparatorChar);
}

public partial class FileBase
{
    // we can't do anything here, but shared method has a fallback that will take care of it
    internal static string PlatformGetContentType(string extension) => null;

    internal void PlatformInit(FileBase file)
    {
    }

    internal virtual Task<Stream> PlatformOpenReadAsync()
    {
        if (StorageFile != null)
            return StorageFile.OpenStreamForReadAsync();
        return Task.FromResult((Stream)File.OpenRead(FullPath));
    }
}