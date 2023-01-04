using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using P42.Uno.AsyncNavigation;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#if !HAS_UNO
using Popup = Microsoft.UI.Xaml.Controls.Primitives.Popup;
#else
using Popup = Microsoft.UI.Xaml.Controls.Popup;
#endif

namespace Samples.Helpers
{
    public static class ActionSheet
    {
        public static async Task<string> Display(string title, string cancel, string desctruct, params string[] buttons)
        {

            if (Microsoft.UI.Xaml.Window.Current.Content is Frame frame && frame.Content is NavigationPage navPage)
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
