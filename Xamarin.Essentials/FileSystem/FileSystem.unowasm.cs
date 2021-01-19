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
            if (false)
            {
                var x = new FileForAssetResult();
            }

            System.Diagnostics.Debug.WriteLine("FileSystem.PlatformOpenAppPackageFileAsync WASM ENTER");
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            System.Diagnostics.Debug.WriteLine("FileSystem.PlatformOpenAppPackageFileAsync A");
            var json = await WebAssemblyRuntime.InvokeAsync($"UnoFileSystem_FileForAsset('{filename}')");
            System.Diagnostics.Debug.WriteLine("FileSystem.PlatformOpenAppPackageFileAsync b json: " + json);

            var result = JsonConvert.DeserializeObject<FileForAssetResult>(json);

            System.Diagnostics.Debug.WriteLine("FileSystem.PlatformOpenAppPackageFileAsync C");
            if (result.abort || !string.IsNullOrWhiteSpace(result.error))
                return null;
            //return Package.Current.InstalledLocation.OpenStreamForReadAsync(NormalizePath(filename));
            System.Diagnostics.Debug.WriteLine("FileSystem.PlatformOpenAppPackageFileAsync D");
            return File.OpenRead(result.path);
        }

        internal static string NormalizePath(string path)
            => path.Replace('/', Path.DirectorySeparatorChar);

    }

    public class FileForAssetResult
    {
        public string error { get; set; }

        public bool abort { get; set; }

        public string path { get; set; }

        public string isText { get; set; }

        [Preserve]
        public FileForAssetResult() { }
    }


    public partial class FileBase
    {
        internal FileBase(IStorageFile file)
            : this(file?.Path)
        {
            File = file;
            ContentType = file?.ContentType;
        }

        internal void PlatformInit(FileBase file)
        {
            File = file.File;
        }

        internal IStorageFile File { get; set; }

        // we can't do anything here, but Windows will take care of it
        internal static string PlatformGetContentType(string extension) => null;

        internal virtual Task<Stream> PlatformOpenReadAsync() =>
            File.OpenStreamForReadAsync();

        public override string ToString()
        {
            return "{ type: " + GetType() + ", path: " + FullPath + ", contentType: " + contentType + ", name: " + FileName + " }";
        }
    }

    public partial class ReadOnlyFile
    {
        public ReadOnlyFile(IStorageFile file)
            : base(file)
        {
        }
    }

    public partial class FileResult
    {
        internal FileResult(IStorageFile file)
            : base(file)
        {
        }
    }
}
