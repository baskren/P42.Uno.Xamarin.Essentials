using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.Foundation;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Xamarin.Essentials
{
    public static partial class FileSystem
    {
        static string PlatformCacheDirectory
        {
            get
            {
                var folder = ApplicationData.Current.LocalCacheFolder.Path;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return folder;
            }
        }

        static string PlatformAppDataDirectory
        {
            get
            {
                var folder = ApplicationData.Current.LocalFolder.Path;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return folder;
            }
        }

        static async Task<Stream> PlatformOpenAppPackageFileAsync(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            var json = await WebAssemblyRuntime.InvokeAsync($"UnoFileSystem_FileForAsset('{filename}')");
            var result = JsonConvert.DeserializeObject<FileForAssetResult>(json);

            if (result.Abort || !string.IsNullOrWhiteSpace(result.Error))
                return null;

            // return Package.Current.InstalledLocation.OpenStreamForReadAsync(NormalizePath(filename));
            return File.OpenRead(result.Path);
        }

        internal static string NormalizePath(string path)
            => path.Replace('/', Path.DirectorySeparatorChar);
    }

    public class FileForAssetResult
    {
        public string Error { get; set; }

        public bool Abort { get; set; }

        public string Path { get; set; }

        public string IsText { get; set; }

        [Preserve]
        public FileForAssetResult()
        {
        }
    }

    public partial class FileBase
    {
        // we can't do anything here, but Windows will take care of it
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

        public override string ToString()
        {
            return "{ type: " + GetType() + ", Path: " + FullPath + ", contentType: " + contentType + ", name: " + FileName + " }";
        }
    }
}
