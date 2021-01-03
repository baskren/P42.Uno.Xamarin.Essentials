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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextToSpeechPage : BasePage
    {
        public TextToSpeechPage()
        {
            this.InitializeComponent();
        }

        void OnSpeakClicked(object sender, RoutedEventArgs e)
        {
            /*
                    <!--
                    Command="{Binding SpeakCommand}"
                    CommandParameter="{Binding Source=SwitchMultiple, Path=IsToggled}"
                    Content="Speak"
                    -->
             */
            if (DataContext is ViewModel.TextToSpeechViewModel vm)
            {
                var command = vm.SpeakCommand;
                var parameter = SwitchMultiple.IsOn;
                command.Execute(parameter);
            }
        }
    }
}
