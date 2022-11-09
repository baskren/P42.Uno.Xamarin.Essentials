using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Xamarin.Essentials;
using Samples.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SharePage : Page
    {
        public SharePage()
        {
            this.InitializeComponent();

            DataContextChanged += SharePage_DataContextChanged;
        }

        private void SharePage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"SharePage. : ");
            if (args.NewValue is ShareViewModel vm)
            {
                System.Diagnostics.Debug.WriteLine($"SharePage. : ");
            }
        }

        ShareFileRequest GetShareFileRequest()
        {
            var shareFile = CreateShareFile(_shareFileNameTextBox.Text, _shareFileContentTextBox.Text, "fileShareDemoFile.txt");
            var request = new Xamarin.Essentials.ShareFileRequest
            {
                Title = _shareFileTitleTextBox.Text,
                File = shareFile
            };
            return request;
        }

        ShareMultipleFilesRequest GetShareFilesRequest()
        {
            var file1Name = _shareFile1Name.Text;
            var file1Content = _shareFile1Content.Text;

            var file2Name = _shareFile2Name.Text;
            var file2Content = _shareFile2Content.Text;

            var request = new Xamarin.Essentials.ShareMultipleFilesRequest
            {
                Title = _shareFilesTitle.Text,
                Files = new List<ShareFile>()
            };
            if (!string.IsNullOrWhiteSpace(file1Content))
            {
                var shareFile = CreateShareFile(file1Name, file1Content, "fileShareDemoFile1.txt");
                request.Files.Add(shareFile);
            }
            if (!string.IsNullOrWhiteSpace(file2Content))
            {
                var shareFile = CreateShareFile(file2Name, file2Content, "fileShareDemoFile2.txt");
                request.Files.Add(shareFile);
            }
            return request;
        }

        void OnCanShareTextClick(object sender, RoutedEventArgs e)
        {
            var request = new ShareTextRequest(_shareTextText.Text, _shareTextTitle.Text);
            _canShareTextResult.Text = Share.CanShare(request).ToString();
        }

        void OnCanShareFileClick(object sender, RoutedEventArgs e)
        {
            var request = GetShareFileRequest();
            _canShareFileResult.Text = Share.CanShare(request).ToString();
        }

        void OnCanShareFilesClick(object sender, RoutedEventArgs e)
        {
            var request = GetShareFilesRequest();
            _canShareFilesResult.Text = Share.CanShare(request).ToString();
        }

        static ShareFile CreateShareFile(string fileName, string fileContents, string emptyName)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) ? emptyName : fileName.Trim();
            var path = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, fileName);
            File.WriteAllText(path, fileContents);

            var txt = File.ReadAllText(path);
            System.Diagnostics.Debug.WriteLine("SharePage.CreateShareFile: path[" + path + "] content[" + txt + "]");
            return new ShareFile(path, "text/plain");
        }

        void OnShareFileInputLostFocus(object sender, RoutedEventArgs e)
        {
            if (_shareFilesButton is Button shareButton)
            {
                var request = GetShareFilesRequest();
                shareButton.SetShareRequestPayload(request);
            }
        }



        void OnShareFilesInputLostFocus(object sender, RoutedEventArgs e)
        {
            if (_shareFilesButton is Button shareButton)
            {
                var request = GetShareFilesRequest();
                shareButton.SetShareRequestPayload(request);
            }
        }

        async void OnShareFileClick(object sender, RoutedEventArgs e)
        {
            var request = GetShareFileRequest();
            await Share.RequestAsync(request);
        }

        async void OnShareFilesClick(object sender, RoutedEventArgs e)
        {
            var request = GetShareFilesRequest();
            await Share.RequestAsync(request);
        }
    }

}
