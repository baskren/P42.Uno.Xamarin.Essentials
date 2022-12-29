using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Photos;

namespace Xamarin.Essentials
{
    public static partial class Permissions
    {
        public partial class Photos : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredInfoPlistKeys => () =>
            {
                if (!Permissions.IsKeyDeclaredInInfoPlist("NSPhotoLibraryAddUsageDescription"))
                    Debug.WriteLine("You may need to set `NSPhotoLibraryAddUsageDescription` in your Info.plist file to use the Photo permission.");

                return new string[] { "NSPhotoLibraryUsageDescription" };
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
                switch (PHPhotoLibrary.AuthorizationStatus)
                {
                    case PHAuthorizationStatus.Authorized:
                        return PermissionStatus.Granted;
                    case PHAuthorizationStatus.Denied:
                        return PermissionStatus.Denied;
                    case PHAuthorizationStatus.Restricted:
                        return PermissionStatus.Restricted;
                    default:
                        return PermissionStatus.Unknown;
                }
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
}
