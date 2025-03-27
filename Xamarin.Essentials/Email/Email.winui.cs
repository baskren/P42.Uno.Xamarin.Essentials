using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using NativeEmailAttachment = Windows.ApplicationModel.Email.EmailAttachment;
using NativeEmailMessage = Windows.ApplicationModel.Email.EmailMessage;

namespace Xamarin.Essentials
{
    public static partial class Email
    {
        internal static bool IsComposeSupported
            => ApiInformation.IsTypePresent("Windows.ApplicationModel.Email.EmailManager");

        internal static bool PlatformSupportsAttachments
            => true;

        static async Task PlatformComposeAsync(EmailMessage message)
        {
            var nativeMessage = new NativeEmailMessage();
            if (!string.IsNullOrEmpty(message?.Body))
                nativeMessage.Body = message.Body;
            if (!string.IsNullOrEmpty(message?.Subject))
                nativeMessage.Subject = message.Subject;
            Sync(message?.To, nativeMessage.To);
            Sync(message?.Cc, nativeMessage.CC);
            Sync(message?.Bcc, nativeMessage.Bcc);

            if (message?.Attachments?.Count > 0)
            {
                foreach (var attachment in message.Attachments)
                {
                    var path = FileSystem.NormalizePath(attachment.FullPath);
                    var file = attachment.StorageFile ?? await StorageFile.GetFileFromPathAsync(path);

                    var stream = RandomAccessStreamReference.CreateFromFile(file);
                    var nativeAttachment = new NativeEmailAttachment(attachment.FileName, stream);

                    if (!string.IsNullOrEmpty(attachment.ContentType))
                        nativeAttachment.MimeType = attachment.ContentType;
                    else if (!string.IsNullOrWhiteSpace(file?.ContentType))
                        nativeAttachment.MimeType = file.ContentType;

                    nativeMessage.Attachments.Add(nativeAttachment);
                }
            }

            await EmailManager.ShowComposeNewEmailAsync(nativeMessage);

        }

        static void Sync(List<string> recipients, IList<EmailRecipient> nativeRecipients)
        {
            if (recipients == null)
                return;

            foreach (var recipient in recipients)
            {
                if (string.IsNullOrWhiteSpace(recipient))
                    continue;

                nativeRecipients.Add(new EmailRecipient(recipient));
            }
        }
        /// <summary>
        /// The MAPISendMail function sends a message.
        ///
        /// This function differs from the MAPISendDocuments function in that it allows greater
        /// flexibility in message generation.
        /// </summary>
        [DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
        public static extern uint MAPISendMail(IntPtr lhSession, IntPtr ulUIParam,
          MapiMessage lpMessage, uint flFlags, uint ulReserved);

        [DllImport("MAPI32.DLL")]
        static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

        static IntPtr GetAttachments(string fileName, out int fileCount)
        {
            var size = Marshal.SizeOf(typeof(MapiFileDesc));
            var intPtr = Marshal.AllocHGlobal(size);

            var mapiFileDesc = new MapiFileDesc();
            //An integer used to indicate where in the message text to render the attachment.
            mapiFileDesc.position = -1;
            var ptr = (int)intPtr;

            mapiFileDesc.name = Path.GetFileName(fileName);
            mapiFileDesc.path = fileName;
            Marshal.StructureToPtr(mapiFileDesc, (IntPtr)ptr, false);
            ptr += size;

            fileCount = 1;
            return intPtr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class MapiMessage
        {
            public int reserved;
            public string subject;
            public string noteText;
            public string messageType;
            public string dateReceived;
            public string conversationID;
            public int flags;
            public IntPtr originator;
            public int recipCount;
            public IntPtr recips;
            public int fileCount;
            public IntPtr files;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class MapiFileDesc
        {
            public int reserved;
            public int flags;
            public int position;
            public string path;
            public string name;
            public IntPtr type;
        }
    }
}
