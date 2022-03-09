using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;

namespace Xamarin.Essentials
{
    public static partial class FilePicker
    {
        static async Task<IEnumerable<FileResult>> PlatformPickAsync(PickOptions options, bool allowMultiple = false)
        {
            // we only need the permission when accessing the file, but it's more natural
            // to ask the user first, then show the picker.
            await Permissions.EnsureGrantedAsync<Permissions.StorageRead>();

            // Essentials supports >= API 19 where this action is available
            var action = Intent.ActionOpenDocument;

            var intent = new Intent(action);
            intent.SetType(FileSystem.MimeTypes.All);
            intent.PutExtra(Intent.ExtraAllowMultiple, allowMultiple);

            var allowedTypes = options?.FileTypes?.Value?.ToArray();
            if (allowedTypes?.Length > 0)
                intent.PutExtra(Intent.ExtraMimeTypes, allowedTypes);

            var pickerIntent = Intent.CreateChooser(intent, options?.PickerTitle ?? "Select file");

            try
            {
                var resultList = new List<FileResult>();
                void OnResult(Intent intent)
                {
                    // The uri returned is only temporary and only lives as long as the Activity that requested it,
                    // so this means that it will always be cleaned up by the time we need it because we are using
                    // an intermediate activity.

                    if (intent.ClipData == null)
                    {
                        var path = FileSystem.EnsurePhysicalPath(intent.Data);
                        resultList.Add(new FileResult(path));
                    }
                    else
                    {
                        for (var i = 0; i < intent.ClipData.ItemCount; i++)
                        {
                            var uri = intent.ClipData.GetItemAt(i).Uri;
                            var path = FileSystem.EnsurePhysicalPath(uri);
                            resultList.Add(new FileResult(path));
                        }
                    }
                }

                await IntermediateActivity.StartAsync(pickerIntent, Platform.requestCodeFilePicker, onResult: OnResult);

                return resultList;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        static async Task<string> PlatformExportAsync(byte[] bytes, SaveOptions options)
            => await PlatformExportAsync(options, (stream) => stream.Write(bytes, 0, bytes.Length));

        static async Task<string> PlatformExportAsync(string text, SaveOptions options)
            => await PlatformExportAsync(options, (stream) =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(text);
                }
            });

        static async Task<string> PlatformExportAsync(SaveOptions options, Action<Stream> writeAction)
        {
            // we only need the permission when accessing the file, but it's more natural
            // to ask the user first, then show the picker.
            await Permissions.RequestAsync<Permissions.StorageWrite>();

            // Essentials supports >= API 19 where this action is available
            // var pickFolderIntent = new Intent(Intent.ActionOpenDocumentTree);

            var fileName = options?.SuggestedFileName;
            if (string.IsNullOrWhiteSpace(options?.SuggestedFileName))
                fileName = Xamarin.Essentials.AppInfo.Name + ".data";

            var tcs = new TaskCompletionSource<string>();

            // var pickerIntent = Intent.CreateChooser(pickFolderIntent, options?.PickerTitle ?? "Choose Folder to save " + fileName);

            try
            {
                /*
                void OnPickFolderResult(Intent intent)
                {
                    // The uri returned is only temporary and only lives as long as the Activity that requested it,
                    // so this means that it will always be cleaned up by the time we need it because we are using
                    // an intermediate activity.
                    string path = null;
                    if (intent.ClipData == null)
                    {
                        if (intent.Data is global::Android.Net.Uri uri)
                        {
                            if (global::Android.Provider.DocumentsContract.IsTreeUri(uri))
                            {

                                var id = global::Android.Provider.DocumentsContract.GetTreeDocumentId(uri);

                                var fileUri = global::Android.Provider.DocumentsContract.BuildChildDocumentsUriUsingTree(uri, fileName);

                                using (var fileOutputStream = global::Android.App.Application.Context.ContentResolver.OpenOutputStream(fileUri))
                                {
                                    writeAction?.Invoke(fileOutputStream);
                                    fileOutputStream.Flush();
                                    fileOutputStream.Close();
                                    tcs.SetResult(fileUri.Path);
                                }
                                System.Diagnostics.Debug.WriteLine($"FilePicker.");
                            }
                        }
                        //path = FileSystem.EnsurePhysicalPath(intent.Data);
                    }
                    else
                    {
                        for (var i = 0; i < intent.ClipData.ItemCount; i++)
                        {
                            var uri = intent.ClipData.GetItemAt(i).Uri;
                            path = FileSystem.EnsurePhysicalPath(uri);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(path))
                        tcs.SetResult(null);

                    var saveFileIntent = new Intent(Intent.ActionCreateDocument);
                    saveFileIntent.AddCategory(Intent.CategoryOpenable);
                    if (string.IsNullOrWhiteSpace(options?.ContentType))
                        saveFileIntent.SetType(FileSystem.MimeTypes.All);
                    else
                        saveFileIntent.SetType(options.ContentType);
                    saveFileIntent.PutExtra(Intent.ExtraTitle, fileName);

                    void OnSaveFileResult(Intent intent)
                    {
                        if (intent.Data is global::Android.Net.Uri uri)
                        {
                            using (var fileOutputStream = global::Android.App.Application.Context.ContentResolver.OpenOutputStream(uri))
                            {
                                writeAction?.Invoke(fileOutputStream);
                                fileOutputStream.Flush();
                                fileOutputStream.Close();
                                tcs.SetResult(uri.Path);
                            }
                        }
                        tcs.SetResult(null);
                    }

                    IntermediateActivity.StartAsync(saveFileIntent, Platform.requestCodeSaveFile, onResult: OnSaveFileResult);
                }

                await IntermediateActivity.StartAsync(pickerIntent, Platform.requestCodeFilePicker, onResult: OnPickFolderResult);
                */
                var saveFileIntent = new Intent(Intent.ActionCreateDocument);
                saveFileIntent.AddCategory(Intent.CategoryOpenable);
                if (string.IsNullOrWhiteSpace(options?.ContentType))
                    saveFileIntent.SetType(FileSystem.MimeTypes.All);
                else
                    saveFileIntent.SetType(options.ContentType);
                saveFileIntent.PutExtra(Intent.ExtraTitle, fileName);

                void OnSaveFileResult(Intent intent)
                {
                    if (intent.Data is global::Android.Net.Uri uri)
                    {
                        using (var fileOutputStream = global::Android.App.Application.Context.ContentResolver.OpenOutputStream(uri))
                        {
                            writeAction?.Invoke(fileOutputStream);
                            fileOutputStream.Flush();
                            fileOutputStream.Close();

                            // var x = global::Android.App.Application.Context.ContentResolver.OpenFileDescriptor(uri, "r");
                            var cursor = global::Android.App.Application.Context.ContentResolver.Query(uri, null, null, null, null);
                            var nameIndex = cursor.GetColumnIndex(global::Android.Provider.IOpenableColumns.DisplayName);
                            var sizeIndex = cursor.GetColumnIndex(global::Android.Provider.IOpenableColumns.Size);
                            cursor.MoveToFirst();
                            var name = cursor.GetString(nameIndex);
                            var size = cursor.GetString(sizeIndex);

                            var path = new List<string>(uri.PathSegments);
                            if (path.Any())
                                path.RemoveAt(path.Count - 1);
                            path.Add(name);
                            tcs.SetResult(Path.Combine(path.ToArray()));
                            return;
                        }
                    }

                    tcs.SetResult(null);
                }

                await IntermediateActivity.StartAsync(saveFileIntent, Platform.requestCodeSaveFile, onResult: OnSaveFileResult);

                return await tcs.Task;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }
    }

    public partial class FilePickerFileType
    {
        static FilePickerFileType PlatformImageFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { FileSystem.MimeTypes.ImagePng, FileSystem.MimeTypes.ImageJpg } }
            });

        static FilePickerFileType PlatformPngFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { FileSystem.MimeTypes.ImagePng } }
            });

        static FilePickerFileType PlatformJpegFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { FileSystem.MimeTypes.ImageJpg } }
            });

        static FilePickerFileType PlatformVideoFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { FileSystem.MimeTypes.VideoAll } }
            });

        static FilePickerFileType PlatformPdfFileType() =>
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { FileSystem.MimeTypes.Pdf } }
            });
    }
}
