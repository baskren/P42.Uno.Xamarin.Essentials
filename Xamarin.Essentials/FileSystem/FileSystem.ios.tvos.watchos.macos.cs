using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using MobileCoreServices;

namespace Xamarin.Essentials
{
    public static partial class FileSystem
    {
        static string PlatformCacheDirectory
            => GetDirectory(NSSearchPathDirectory.CachesDirectory);

        static string PlatformAppDataDirectory
            => GetDirectory(NSSearchPathDirectory.LibraryDirectory);

        static Task<Stream> PlatformOpenAppPackageFileAsync(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            filename = filename.Replace('\\', Path.DirectorySeparatorChar);
            var root = NSBundle.MainBundle.BundlePath;
#if __MACOS__
            root = Path.Combine(root, "Contents", "Resources");
#endif
            var file = Path.Combine(root, "Contents", "Resources", filename);
            return Task.FromResult((Stream)File.OpenRead(file));
        }

        static string GetDirectory(NSSearchPathDirectory directory)
        {
            var dirs = NSSearchPath.GetDirectories(directory, NSSearchPathDomain.User);
            if (dirs == null || dirs.Length == 0)
            {
                // this should never happen...
                return null;
            }
            return dirs[0];
        }
    }

    public partial class FileBase
    {
        NSUrl url;

        public NSUrl Url
        {
            get => url ?? new NSUrl(new System.Uri(FullPath).AbsoluteUri);
            private set => url = value;
        }

        internal FileBase(NSUrl file)
            : this(file?.Path)
        {
            Url = file;
            FileName = NSFileManager.DefaultManager.DisplayName(file?.Path);
        }

        internal static string PlatformGetContentType(string extension)
        {
            // ios does not like the extensions
            extension = extension?.TrimStart('.');

            var id = UTType.CreatePreferredIdentifier(UTType.TagClassFilenameExtension, extension, null);

            /*
            string?[] mimeTypes = null;

            if ((DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 14)
                || (DeviceInfo.Platform == DevicePlatform.macCatalyst && DeviceInfo.Version.Major >= 14)
                || (DeviceInfo.Platform == DevicePlatform.macOS && DeviceInfo.Version.Major >= 11)
                )
            {
                var utType = UniformTypeIdentifiers.UTType.CreateFromIdentifier(id);
                return utType.PreferredMimeType;
            }
            mimeTypes = UTType.CopyAllTags(id, UTType.TagClassMIMEType);
            return mimeTypes?.Length > 0 ? mimeTypes[0] : null;
            */
            var utType = UniformTypeIdentifiers.UTType.CreateFromIdentifier(id);
            return utType?.PreferredMimeType;
        }

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

    public partial class ReadOnlyFile
    {
        public ReadOnlyFile(NSUrl file)
            : base(file)
        {
        }
    }

    public partial class FileResult
    {
        public FileResult(NSUrl nSUrl)
            : base(nSUrl)
        {
        }
    }
}
