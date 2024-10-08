﻿using System.Threading.Tasks;
using AppKit;
using Foundation;

namespace Xamarin.Essentials
{
    public static partial class Email
    {
        internal static bool IsComposeSupported =>
            MainThread.InvokeOnMainThread(() => NSWorkspace.SharedWorkspace.UrlForApplication(NSUrl.FromString("mailto:")) != null);

        internal static bool PlatformSupportsAttachments
            => false;

        static Task PlatformComposeAsync(EmailMessage message)
        {
            var url = GetMailToUri(message);

            using var nsurl = NSUrl.FromString(url);
            NSWorkspace.SharedWorkspace.OpenUrl(nsurl);
            return Task.CompletedTask;
        }
    }
}
