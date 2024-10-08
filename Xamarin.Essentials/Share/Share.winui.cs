﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Xamarin.Essentials
{
    public static partial class Share
    {
        static Task PlatformRequestAsync(ShareTextRequest request)
        {
            try
            {
                var dataTransferManager = Platform.MainWindow.GetDataTransferManager();
                dataTransferManager.DataRequested += ShareTextHandler;
                dataTransferManager.ShowShareUI();

                void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
                {
                    var newRequest = e.Request;
                    newRequest.Data.Properties.Title = request.Title ?? AppInfo.Name;

                    if (!string.IsNullOrWhiteSpace(request.Text))
                        newRequest.Data.SetText(request.Text);

                    if (!string.IsNullOrWhiteSpace(request.Uri))
                        newRequest.Data.SetWebLink(new Uri(request.Uri));

                    dataTransferManager.DataRequested -= ShareTextHandler;
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"Xamarin.Essentials.Share : {ex.GetType()} : {ex.Message} \n {ex.StackTrace}");
                Console.WriteLine($"Xamarin.Essentials.Share : {ex.GetType()} : {ex.Message} \n {ex.StackTrace}");
            }
            return Task.CompletedTask;
        }

        static async Task PlatformRequestAsync(ShareMultipleFilesRequest request)
        {
            var storageFiles = new List<IStorageFile>();
            foreach (var file in request.Files)
                storageFiles.Add(file.StorageFile ?? await StorageFile.GetFileFromPathAsync(file.FullPath));

            //var dataTransferManager = DataTransferManager.GetForCurrentView();
            var dataTransferManager = Platform.MainWindow.GetDataTransferManager();
            dataTransferManager.DataRequested += ShareTextHandler;
            dataTransferManager.ShowShareUI();

            void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
            {
                var newRequest = e.Request;
                newRequest.Data.SetStorageItems(storageFiles.ToArray());
                newRequest.Data.Properties.Title = request.Title ?? AppInfo.Name;

                dataTransferManager.DataRequested -= ShareTextHandler;
            }
        }

        static bool PlatformCanShare(ShareRequestBase request) => true;

        static bool PlatformIsAvailable() => true;
    }
}
