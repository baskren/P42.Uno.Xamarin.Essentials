using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
#if __WASM__
using Newtonsoft.Json;
using Uno.Foundation;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Essentials
{
    public static class SharingExtensions
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

#if __WASM__
        static readonly Dictionary<string, WeakReference<FrameworkElement>> shareRequestElements = new Dictionary<string, WeakReference<FrameworkElement>>();

        public static string GetShareRequestJsonForHtmlElement(string id)
        {
            System.Diagnostics.Debug.WriteLine("SharingExtensions.GetShareRequestJsonForHtmlElement ENTER");
            if (GetShareRequestForHtmlElement(id) is ShareRequestBase request)
            {
                var json = JsonConvert.SerializeObject(request);
                System.Diagnostics.Debug.WriteLine("SharingExtensions.GetShareRequestJsonForHtmlElement EXIT");
                return json;
            }
            System.Diagnostics.Debug.WriteLine("SharingExtensions.GetShareRequestJsonForHtmlElement EXIT");
            return null;
        }

        internal static ShareRequestBase GetShareRequestForHtmlElement(string id)
        {
            if (GetShareRequestElementForHtmlElement(id) is FrameworkElement element)
            {
                if (element.GetShareRequestPayload() is ShareRequestBase request)
                {
                    return request;
                }
            }
            return null;
        }

        internal static FrameworkElement GetShareRequestElementForHtmlElement(string id)
        {
            if (shareRequestElements.TryGetValue(id, out var weakRef))
            {
                if (weakRef.TryGetTarget(out var element))
                {
                    return element;
                }
            }
            return null;
        }

        static void EnableShareOnTapped(this FrameworkElement element)
        {
            System.Diagnostics.Debug.WriteLine("ButtonExtensions.EnableShareOnTapped ENTER");

            if (element.IsLoaded)
                AddShareOnTappedHandler(element);
            else
                element.Loaded += AddShareOnTappedUponElementLoaded;

            System.Diagnostics.Debug.WriteLine("ButtonExtensions.EnableShareOnTapped EXIT");
        }

        static void DisableShareOnTapped(this FrameworkElement element)
        {
            element.SetShareRequestPayload(null);
            if (element.IsLoaded)
                RemoveShareOnTappedHandler(element);
            else
                element.Loaded -= AddShareOnTappedUponElementLoaded;
        }

        static void AddShareOnTappedUponElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
                element.AddShareOnTappedHandler();
        }

        static void AddShareOnTappedHandler(this FrameworkElement element)
        {
            var id = element.GetHtmlId();
            shareRequestElements[id] = new WeakReference<FrameworkElement>(element);
            System.Diagnostics.Debug.WriteLine("ButtonExtensions.EnableShareOnTapped id:[" + id + "]");

            //var javascript = $"$('#{id}')[0].onclick = function() {{ alert('pizza'); }} ";
            var javascript = $"$('#{id}')[0].onclick = function() {{ UnoShare_ShareFromElement('{id}'); }} ";
            System.Diagnostics.Debug.WriteLine("ButtonExtensions.EnableShareOnTapped javascript: [" + javascript + "]");

            WebAssemblyRuntime.InvokeJS(javascript);
        }

        static void RemoveShareOnTappedHandler(this FrameworkElement element)
        {
            var id = element.GetHtmlId();
            shareRequestElements.Remove(id);
            System.Diagnostics.Debug.WriteLine("ButtonExtensions.EnableShareOnTapped id:[" + id + "]");

            var javascript = $"$('#{id}')[0].onclick = null; ";
            System.Diagnostics.Debug.WriteLine("ButtonExtensions.EnableShareOnTapped javascript: [" + javascript + "]");

            WebAssemblyRuntime.InvokeJS(javascript);
        }

#else

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
