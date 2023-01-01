using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Newtonsoft.Json;
using Uno.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Xamarin.Essentials
{
    public static partial class SharingExtensions
    {

        static readonly Dictionary<string, WeakReference<FrameworkElement>> shareRequestElements = new Dictionary<string, WeakReference<FrameworkElement>>();

        public static string GetShareRequestJsonForHtmlElement(string id)
        {
            if (GetShareRequestForHtmlElement(id) is ShareRequestBase request)
                return JsonConvert.SerializeObject(request);
            return null;
        }

        internal static ShareRequestBase GetShareRequestForHtmlElement(string id)
        {
            if (GetShareRequestElementForHtmlElement(id) is FrameworkElement element)
            {
                if (element.GetShareRequestPayload() is ShareRequestBase request)
                    return request;
            }
            return null;
        }

        internal static FrameworkElement GetShareRequestElementForHtmlElement(string id)
        {
            if (shareRequestElements.TryGetValue(id, out var weakRef))
            {
                if (weakRef.TryGetTarget(out var element))
                    return element;
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
            var javascript = $"$('#{id}')[0].onclick = function() {{ UnoShare_ShareFromElement('{id}'); }} ";
            WebAssemblyRuntime.InvokeJS(javascript);
        }

        static void RemoveShareOnTappedHandler(this FrameworkElement element)
        {
            var id = element.GetHtmlId();
            shareRequestElements.Remove(id);
            var javascript = $"$('#{id}')[0].onclick = null; ";
            WebAssemblyRuntime.InvokeJS(javascript);
        }

    }
}
