﻿using System.Threading.Tasks;

namespace Xamarin.Essentials
{
    public static partial class Email
    {
        internal static bool IsComposeSupported =>
            throw ExceptionUtils.NotSupportedOrImplementedException;

        internal static bool PlatformSupportsAttachments
            => false;

        static Task PlatformComposeAsync(EmailMessage message) =>
            throw ExceptionUtils.NotSupportedOrImplementedException;
    }

#if NETSTANDARD1_0 || NET7_0
    public partial class EmailAttachment
    {
        string PlatformGetContentType(string extension) =>
            throw ExceptionUtils.NotSupportedOrImplementedException;
    }
#endif
}
