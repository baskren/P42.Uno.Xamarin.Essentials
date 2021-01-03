using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using P42.Uno.AsyncNavigation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#if NETFX_CORE
using Popup = Windows.UI.Xaml.Controls.Primitives.Popup;
#else
using Popup = Windows.UI.Xaml.Controls.Popup;
#endif

namespace Samples.Helpers
{
    public static class ActionSheet
    {
        public static async Task<string> Display(string title, string cancel, string desctruct, params string[] buttons)
        {

            if (Windows.UI.Xaml.Window.Current.Content is Frame frame && frame.Content is NavigationPage navPage)
            {
                var tcs = new TaskCompletionSource<string>();
                var content = new ActionSheetPopupContent(tcs)
                {
                    TitleLabel = title,
                    CancelLabel = cancel,
                    DestructionLabel = desctruct,
                    ButtonsLabels = buttons
                };
                {
                    var popup = new Popup
                    {
                        Child = content,
                        IsOpen = true,
                        LightDismissOverlayMode = LightDismissOverlayMode.On,
                        IsLightDismissEnabled = true
                    };
                    content.WeakParentPopup = new WeakReference<Popup>(popup);

                    if (navPage.Content is Grid grid)
                    {
                        grid.Children.Add(popup);
                    }
                    await tcs.Task;
                    popup.IsOpen = false;
                    return tcs.Task.Result;

                }
            }
            return null;
        }


    }
}
