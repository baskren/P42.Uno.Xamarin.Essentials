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
using P42.Uno.AsyncNavigation;
using Samples.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class HomePage : BasePage
    {
        public HomePage()
        {
            this.InitializeComponent();
            _listView.IsItemClickEnabled = true;
        }

        bool pushing;

        async void OnSampleClicked(object sender, ItemClickEventArgs e)
        {
            if (!pushing)
            {
                pushing = true;
                var item = e.ClickedItem as SampleItem;
                if (item == null)
                    return;

                Page page;
                if (item.PageType == typeof(TextToSpeechPage))
                    page = new TextToSpeechPage();
                else
                    page = (Page)Activator.CreateInstance(item.PageType);
                await this.PushAsync(page);

                // deselect Item
                ((ListView)sender).SelectedItem = null;
                pushing = false;
            }
        }
    }
}
