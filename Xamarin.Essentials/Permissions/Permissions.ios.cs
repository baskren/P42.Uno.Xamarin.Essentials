using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AddressBook;
using AVFoundation;
using MediaPlayer;
using Speech;

#pragma warning disable CA1422 // Call site reachable on all platforms
namespace Xamarin.Essentials;

public static partial class Permissions
{
    static partial class AVPermissions
    {
        internal static PermissionStatus CheckPermissionsStatus(AVAuthorizationMediaType mediaType)
        {
            var status = AVCaptureDevice.GetAuthorizationStatus(mediaType);
            return status switch
            {
                AVAuthorizationStatus.Authorized => PermissionStatus.Granted,
                AVAuthorizationStatus.Denied => PermissionStatus.Denied,
                AVAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static async Task<PermissionStatus> RequestPermissionAsync(AVAuthorizationMediaType mediaType)
        {
            try
            {
                var auth = await AVCaptureDevice.RequestAccessForMediaTypeAsync(mediaType);
                return auth ? PermissionStatus.Granted : PermissionStatus.Denied;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get {mediaType} permission: {ex}");
                return PermissionStatus.Unknown;
            }
        }
    }

    public partial class Camera : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => ["NSCameraUsageDescription"];

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Video);
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Video);
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await AVPermissions.RequestPermissionAsync(AVAuthorizationMediaType.Video);
        }
    }

    public partial class ContactsRead : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => ["NSContactsUsageDescription"];

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return GetAddressBookPermissionStatus();
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = GetAddressBookPermissionStatus();
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await RequestAddressBookPermission();
        }

        internal static PermissionStatus GetAddressBookPermissionStatus()
        {
            var status = ABAddressBook.GetAuthorizationStatus();
            return status switch
            {
                ABAuthorizationStatus.Authorized => PermissionStatus.Granted,
                ABAuthorizationStatus.Denied => PermissionStatus.Denied,
                ABAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static Task<PermissionStatus> RequestAddressBookPermission()
        {
            var addressBook = ABAddressBook.Create(out var createError);

            // if the permission was denied, then we can't create the object
            if (createError?.Code == (int)ABAddressBookError.OperationNotPermittedByUserError)
                return Task.FromResult(PermissionStatus.Denied);

            var tcs = new TaskCompletionSource<PermissionStatus>();

            addressBook.RequestAccess((success, error) =>
            {
                tcs.TrySetResult(success ? PermissionStatus.Granted : PermissionStatus.Denied);

                addressBook?.Dispose();
                addressBook = null;
            });

            return tcs.Task;
        }
    }

    public partial class ContactsWrite : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => ["NSContactsUsageDescription"];

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return ContactsRead.GetAddressBookPermissionStatus();
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = ContactsRead.GetAddressBookPermissionStatus();
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await ContactsRead.RequestAddressBookPermission();
        }
    }

    public partial class Media : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => ["NSAppleMusicUsageDescription"];

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return GetMediaPermissionStatus();
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = GetMediaPermissionStatus();
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await RequestMediaPermission();
        }

        internal static PermissionStatus GetMediaPermissionStatus()
        {
            // Only available in 9.3+
            if (!Platform.HasOSVersion(9, 3))
                return PermissionStatus.Unknown;

            var status = MPMediaLibrary.AuthorizationStatus;
            return status switch
            {
                MPMediaLibraryAuthorizationStatus.Authorized => PermissionStatus.Granted,
                MPMediaLibraryAuthorizationStatus.Denied => PermissionStatus.Denied,
                MPMediaLibraryAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static Task<PermissionStatus> RequestMediaPermission()
        {
            // Only available in 9.3+
            if (!Platform.HasOSVersion(9, 3))
                return Task.FromResult(PermissionStatus.Unknown);

            var tcs = new TaskCompletionSource<PermissionStatus>();

            MPMediaLibrary.RequestAuthorization(s =>
            {
                switch (s)
                {
                    case MPMediaLibraryAuthorizationStatus.Authorized:
                        tcs.TrySetResult(PermissionStatus.Granted);
                        break;
                    case MPMediaLibraryAuthorizationStatus.Denied:
                        tcs.TrySetResult(PermissionStatus.Denied);
                        break;
                    case MPMediaLibraryAuthorizationStatus.Restricted:
                        tcs.TrySetResult(PermissionStatus.Restricted);
                        break;
                    default:
                        tcs.TrySetResult(PermissionStatus.Unknown);
                        break;
                }
            });

            return tcs.Task;
        }
    }

    public partial class Microphone : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => ["NSMicrophoneUsageDescription"];

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Audio);
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Audio);
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await AVPermissions.RequestPermissionAsync(AVAuthorizationMediaType.Audio);
        }
    }

    public partial class Speech : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => ["NSSpeechRecognitionUsageDescription"];

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return GetSpeechPermissionStatus();
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = GetSpeechPermissionStatus();
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await RequestSpeechPermission();
        }

        internal static PermissionStatus GetSpeechPermissionStatus()
        {
            var status = SFSpeechRecognizer.AuthorizationStatus;
            return status switch
            {
                SFSpeechRecognizerAuthorizationStatus.Authorized => PermissionStatus.Granted,
                SFSpeechRecognizerAuthorizationStatus.Denied => PermissionStatus.Denied,
                SFSpeechRecognizerAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static Task<PermissionStatus> RequestSpeechPermission()
        {
            if (!Platform.HasOSVersion(10, 0))
                return Task.FromResult(PermissionStatus.Unknown);

            var tcs = new TaskCompletionSource<PermissionStatus>();

            SFSpeechRecognizer.RequestAuthorization(s =>
            {
                switch (s)
                {
                    case SFSpeechRecognizerAuthorizationStatus.Authorized:
                        tcs.TrySetResult(PermissionStatus.Granted);
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Denied:
                        tcs.TrySetResult(PermissionStatus.Denied);
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Restricted:
                        tcs.TrySetResult(PermissionStatus.Restricted);
                        break;
                    default:
                        tcs.TrySetResult(PermissionStatus.Unknown);
                        break;
                }
            });

            return tcs.Task;
        }
    }
}
#pragma warning restore CA1422 // Call site reachable on all platforms
