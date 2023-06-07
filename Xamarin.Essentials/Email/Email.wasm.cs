using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class Email
    {
        internal static bool IsComposeSupported
            => true;

        internal static bool PlatformSupportsAttachments
            => false;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async Task PlatformComposeAsync(EmailMessage message)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (message != null && message.BodyFormat != EmailBodyFormat.PlainText)
                throw new FeatureNotSupportedException("UWP can only compose plain text email messages.");

            var concatUsed = 0;
            Func<string> concatChar = () => concatUsed++ > 0 ? "&" : "?";

            var payload = "mailto:";
            if (message?.To != null && message.To.Count > 0)
                payload += string.Join(",", message.To);
            if (message?.Cc != null && message.Cc.Count > 0)
                payload += concatChar() + "cc=" + System.Web.HttpUtility.UrlEncode(string.Join(",", message.Cc), System.Text.Encoding.UTF8);
            if (message?.Bcc != null && message.Bcc.Count > 0)
                payload += concatChar() + "bcc=" + System.Web.HttpUtility.UrlEncode(string.Join(",", message.Bcc), System.Text.Encoding.UTF8);
            if (!string.IsNullOrWhiteSpace(message?.Subject))
                payload += concatChar() + "subject=" + System.Web.HttpUtility.UrlEncode(message.Subject, System.Text.Encoding.UTF8);
            if (!string.IsNullOrWhiteSpace(message?.Body))
                payload += concatChar() + "body=" + System.Web.HttpUtility.UrlEncode(message.Body, System.Text.Encoding.UTF8);

            if (message?.Attachments != null && message.Attachments.Count > 0)
                payload += System.Web.HttpUtility.UrlEncode("\n\n ATTACHEMENTS ARE NOT SUPPORTED", System.Text.Encoding.UTF8);


            WebAssemblyRuntime.InvokeJS($"location.replace('{payload}')");
        }
    }
}
