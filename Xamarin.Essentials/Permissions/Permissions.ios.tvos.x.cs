using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Photos;

#pragma warning disable CA1422 // Call site reachable on all platforms
namespace Xamarin.Essentials;

public static partial class Permissions
{
    public partial class Photos : BasePlatformPermission
    {
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys => () =>
        {
            if (!IsKeyDeclaredInInfoPlist("NSPhotoLibraryAddUsageDescription"))
                Debug.WriteLine("You may need to set `NSPhotoLibraryAddUsageDescription` in your Info.plist file to use the Photo permission.");

            return ["NSPhotoLibraryUsageDescription"];
        };

        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            await EnsureDeclaredAsync();

            return GetPhotoPermissionStatus();
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            await EnsureDeclaredAsync();

            var status = GetPhotoPermissionStatus();
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await RequestPhotoPermission();
        }

        static PermissionStatus GetPhotoPermissionStatus()
        {
            var status = PHPhotoLibrary.AuthorizationStatus;
            return status switch
            {
                PHAuthorizationStatus.Authorized => PermissionStatus.Granted,
                PHAuthorizationStatus.Denied => PermissionStatus.Denied,
                PHAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        static Task<PermissionStatus> RequestPhotoPermission()
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();

            PHPhotoLibrary.RequestAuthorization(s =>
            {
                switch (s)
                {
                    case PHAuthorizationStatus.Authorized:
                        tcs.TrySetResult(PermissionStatus.Granted);
                        break;
                    case PHAuthorizationStatus.Denied:
                        tcs.TrySetResult(PermissionStatus.Denied);
                        break;
                    case PHAuthorizationStatus.Restricted:
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
