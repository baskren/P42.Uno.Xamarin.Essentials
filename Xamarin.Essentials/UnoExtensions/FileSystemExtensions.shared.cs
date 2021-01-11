using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Xamarin.Essentials
{
    public static class FileSystemExtensions
    {
        public static async Task<IStorageFile> AsWindowsStorageFileAsync(this FileBase fileBase)
            => await StorageFile.GetFileFromPathAsync(fileBase.FullPath);

        public static FileResult AsXamarinEssentialsFileResult(this IStorageFile storageFile)
            => new FileResult(storageFile.Path, storageFile.ContentType);

        public static ReadOnlyFile AsXamarinEssentialsReadOnlyFile(this IStorageFile storageFile)
            => new ReadOnlyFile(storageFile.Path, storageFile.ContentType);

        public static ShareFile AsXamarinEssentialsShareFile(this IStorageFile storageFile)
            => new ShareFile(storageFile.Path, storageFile.ContentType);
    }
}
