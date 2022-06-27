using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Essentials
{
    public static partial class SharingExtensions
    {
        public static readonly DependencyProperty ShareRequestPayloadProperty = DependencyProperty.RegisterAttached(
            "ShareRequestPayload",
            typeof(ShareRequestBase),
            typeof(SharingExtensions),
            new PropertyMetadata(default(ShareRequestBase), OnShareRequestPayloadChanged));

        static void OnShareRequestPayloadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (e.OldValue is null)
                {
                    if (e.NewValue != null)
                        element.EnableShareOnTapped();
                }
                else if (e.NewValue is null)
                {
                    element.DisableShareOnTapped();
                }
            }
        }

        public static ShareRequestBase GetShareRequestPayload(this UIElement element)
            => (ShareRequestBase)element.GetValue(ShareRequestPayloadProperty);

        public static void SetShareRequestPayload(this UIElement element, ShareRequestBase value)
            => element.SetValue(ShareRequestPayloadProperty, value);

#if !__WASM__

        static void EnableShareOnTapped(this FrameworkElement element)
        {
            element.Tapped += OnElementTappedAsync;
        }

        static void DisableShareOnTapped(this FrameworkElement element)
        {
            element.SetShareRequestPayload(null);
            element.Tapped -= OnElementTappedAsync;
        }

        static async void OnElementTappedAsync(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.GetShareRequestPayload() is ShareRequestBase shareRequest)
            {
                shareRequest.PresentationSourceBounds = ToSystemRectangle(element.GetAbsoluteBounds());

                if (shareRequest is ShareTextRequest shareTextRequest)
                {
                    await Share.RequestAsync(shareTextRequest);
                }
                else if (shareRequest is ShareMultipleFilesRequest shareMultipleFilesRequest)
                {
                    await Share.RequestAsync(shareMultipleFilesRequest);
                }
                else if (shareRequest is ShareFileRequest shareFileRequest)
                {
                    await Share.RequestAsync(shareFileRequest);
                }
            }
        }

        static Rect GetAbsoluteBounds(this FrameworkElement element)
        {
            var ttv = element.TransformToVisual(Windows.UI.Xaml.Window.Current.Content);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
        }

        static System.Drawing.Rectangle ToSystemRectangle(this Rect rect) =>
            new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

#endif
    }
}
