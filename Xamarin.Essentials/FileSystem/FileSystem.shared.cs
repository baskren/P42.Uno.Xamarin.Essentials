﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Xamarin.Essentials
{
    public static partial class FileSystem
    {
        public static string CacheDirectory
            => PlatformCacheDirectory;

        public static string AppDataDirectory
            => PlatformAppDataDirectory;

        public static Task<Stream> OpenAppPackageFileAsync(string filename)
            => PlatformOpenAppPackageFileAsync(filename);

        internal static class MimeTypes
        {
            internal const string All = "*/*";

            internal const string ImageAll = "image/*";
            internal const string ImagePng = "image/png";
            internal const string ImageJpg = "image/jpeg";

            internal const string VideoAll = "video/*";

            internal const string EmailMessage = "message/rfc822";

            internal const string Pdf = "application/pdf";

            internal const string TextPlain = "text/plain";

            internal const string OctetStream = "application/octet-stream";
        }

        internal static class Extensions
        {
            internal const string Png = ".png";
            internal const string Jpg = ".jpg";
            internal const string Jpeg = ".jpeg";
            internal const string Gif = ".gif";
            internal const string Bmp = ".bmp";

            internal const string Avi = ".avi";
            internal const string Flv = ".flv";
            internal const string Gifv = ".gifv";
            internal const string Mp4 = ".mp4";
            internal const string M4v = ".m4v";
            internal const string Mpg = ".mpg";
            internal const string Mpeg = ".mpeg";
            internal const string Mp2 = ".mp2";
            internal const string Mkv = ".mkv";
            internal const string Mov = ".mov";
            internal const string Qt = ".qt";
            internal const string Wmv = ".wmv";

            internal const string Pdf = ".pdf";

            internal static string[] AllImage =>
                new[] { Png, Jpg, Jpeg, Gif, Bmp };

            internal static string[] AllJpeg =>
                new[] { Jpg, Jpeg };

            internal static string[] AllVideo =>
                new[] { Mp4, Mov, Avi, Wmv, M4v, Mpg, Mpeg, Mp2, Mkv, Flv, Gifv, Qt };

            internal static string Clean(string extension, bool trimLeadingPeriod = false)
            {
                if (string.IsNullOrWhiteSpace(extension))
                    return string.Empty;

                extension = extension.TrimStart('*');
                extension = extension.TrimStart('.');

                if (!trimLeadingPeriod)
                    extension = "." + extension;

                return extension;
            }
        }

        internal static DirectoryInfo AssureExists(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));
            DirectoryInfo info = Directory.Exists(fullPath)
                ? info = new DirectoryInfo(fullPath)
                : info = Directory.CreateDirectory(fullPath);
            if (!info.Exists)
                throw new Exception("Could not assure existence of directory [" + info + "]");
            return info;
        }

    }

    public abstract partial class FileBase
    {
        internal const string DefaultContentType = FileSystem.MimeTypes.OctetStream;

        string contentType;

        internal IStorageFile StorageFile { get; set; }

        internal FileBase()
        {
        }

        internal FileBase(IStorageFile storageFile)
            : this(storageFile.Path)
        {
            StorageFile = storageFile;
        }

        // The caller must setup FullPath at least!!!
        internal FileBase(string fullPath)
        {
            if (fullPath == null)
                throw new ArgumentNullException(nameof(fullPath));
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file Path cannot be an empty string.", nameof(fullPath));
            if (string.IsNullOrWhiteSpace(Path.GetFileName(fullPath)))
                throw new ArgumentException("The file Path must be a file Path.", nameof(fullPath));
            FullPath = fullPath;
        }

        public FileBase(FileBase file)
        {
            StorageFile = file.StorageFile;
            FullPath = file.FullPath;
            ContentType = file.ContentType;
            FileName = file.FileName;
            PlatformInit(file);
        }

        internal FileBase(string fullPath, string contentType)
            : this(fullPath)
        {
            FullPath = fullPath;
            ContentType = contentType;
        }

        public string FullPath { get; internal set; }

        public string ContentType
        {
            get => GetContentType();
            set => contentType = value;
        }

        internal string GetContentType()
        {
            // try the provided type
            if (!string.IsNullOrWhiteSpace(contentType))
                return contentType;

            if (!string.IsNullOrWhiteSpace(StorageFile?.ContentType))
                return StorageFile.ContentType;

            // try get from the file extension
            var ext = Path.GetExtension(FullPath);
            if (!string.IsNullOrWhiteSpace(ext))
            {
                var content = PlatformGetContentType(ext);
                if (string.IsNullOrWhiteSpace(content))
                    content = MimeTypeConverter.GetMimeType(ext);
                if (!string.IsNullOrWhiteSpace(content))
                    return content;
            }

            return DefaultContentType;
        }

        string fileName;

        public string FileName
        {
            get => GetFileName();
            set => fileName = value;
        }

        internal string GetFileName()
        {
            // try the provided file name
            if (!string.IsNullOrWhiteSpace(fileName))
                return fileName;

            // try get from the Path
            if (!string.IsNullOrWhiteSpace(FullPath))
                return Path.GetFileName(FullPath);

            // this should never happen as the Path is validated in the constructor
            throw new InvalidOperationException($"Unable to determine the file name from '{FullPath}'.");
        }

        public Task<Stream> OpenReadAsync()
            => PlatformOpenReadAsync();
    }

    public partial class ReadOnlyFile : FileBase
    {
        public ReadOnlyFile(IStorageFile storageFile)
            : base(storageFile)
        {
        }

        public ReadOnlyFile(string fullPath)
            : base(fullPath)
        {
        }

        public ReadOnlyFile(string fullPath, string contentType)
            : base(fullPath, contentType)
        {
        }

        public ReadOnlyFile(FileBase file)
            : base(file)
        {
        }

        public static explicit operator ReadOnlyFile(Windows.Storage.StorageFile storageFile)
            => new ReadOnlyFile(storageFile);
    }

    public partial class FileResult : FileBase
    {
        // The caller must setup FullPath at least!!!
        internal FileResult()
        {
        }

        public FileResult(IStorageFile storageFile)
            : base(storageFile)
        {
        }

        public FileResult(string fullPath)
            : base(fullPath)
        {
        }

        public FileResult(string fullPath, string contentType)
            : base(fullPath, contentType)
        {
        }

        public FileResult(FileBase file)
            : base(file)
        {
        }

        public static explicit operator FileResult(Windows.Storage.StorageFile storageFile)
            => new FileResult(storageFile);
    }
}
