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

        private void OnShareFileFocusLost(Control sender, FocusDisengagedEventArgs args)
        {
            if (sender is FrameworkElement element)
            {
                var fileName = _shareFileNameTextBox.Text;
                var fileContent = _shareFileContentTextBox.Text;

                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    var shareFile = CreateFile(_shareFileNameTextBox.Text, _shareFileContentTextBox.Text, "fileShareDemoFile.txt");
                    var request = new Xamarin.Essentials.ShareFileRequest
                    {
                        Title = _shareFileTitleTextBox.Text,
                        File = shareFile
                    };
                    element.SetShareRequestPayload(request);
                }
                else
                {
                    element.SetShareRequestPayload(null);
                }
            }
        }

        static ShareFile CreateShareFile(string fileName, string fileContents, string emptyName)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) ? emptyName : fileName.Trim();
            var path = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, fileName);
            File.WriteAllText(path, fileContents);
            return new ShareFile(path, "text/plain");
        }

    }
}
