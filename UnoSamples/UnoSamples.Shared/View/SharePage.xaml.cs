using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SharePage : BasePage
    {
        public SharePage()
        {
            this.InitializeComponent();
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
            if (_shareFileButton is Button shareButton)
            {
                var fileName = _shareFileNameTextBox.Text;
                var fileContent = _shareFileContentTextBox.Text;

                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    var shareFile = CreateShareFile(_shareFileNameTextBox.Text, _shareFileContentTextBox.Text, "fileShareDemoFile.txt");
                    var request = new Xamarin.Essentials.ShareFileRequest
                    {
                        Title = _shareFileTitleTextBox.Text,
                        File = shareFile
                    };
                    shareButton.SetShareRequestPayload(request);
                }
                else
                {
                    shareButton.SetShareRequestPayload(null);
                }
            }
            System.Diagnostics.Debug.WriteLine("SharePage.OnShareFileInputLosingFocus EXIT");
        }

        void OnShareFilesInputLostFocus(object sender, RoutedEventArgs e)
        {
            if (_shareFilesButton is Button shareButton)
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
                shareButton.SetShareRequestPayload(request);
            }
            System.Diagnostics.Debug.WriteLine("SharePage.OnShareFileInputLosingFocus EXIT");
        }

    }
}
