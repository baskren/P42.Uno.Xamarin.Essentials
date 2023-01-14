using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace Xamarin.Essentials
{
    public static partial class Permissions
    {
        public static bool IsCapabilityDeclared(string capabilityName)
        {
            throw new NotImplementedException();
            //return IsCapabilityDeclaredAsync(capabilityName).Result;
            /*
            var doc = XDocument.Load("ms-appx:///" + Platform.AppManifestFilename, LoadOptions.None);
            var reader = doc.CreateReader();
            var namespaceManager = new XmlNamespaceManager(reader.NameTable);
            namespaceManager.AddNamespace("x", Platform.AppManifestXmlns);
            namespaceManager.AddNamespace("uap", Platform.AppManifestUapXmlns);

            // If the manifest doesn't contain a capability we need, throw
            return (doc.Root.XPathSelectElements($"//x:DeviceCapability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false) ||
                (doc.Root.XPathSelectElements($"//x:Capability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false) ||
                (doc.Root.XPathSelectElements($"//uap:Capability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false);
            */
        }

        public static async Task<bool> IsCapabilityDeclaredAsync(string capabilityName)
        {
            try
            {
                var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{Platform.AppManifestFilename}"));
                using (var storageStream = await xmlfile.OpenReadAsync())
                {
                    using (var stream = storageStream.AsStreamForRead())
                    {
                        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
                        var reader = doc.CreateReader();
                        var namespaceManager = new XmlNamespaceManager(reader.NameTable);
                        namespaceManager.AddNamespace("x", Platform.AppManifestXmlns);
                        namespaceManager.AddNamespace("uap", Platform.AppManifestUapXmlns);

                        try
                        {
                            var result = (doc.Root.XPathSelectElements($"//x:DeviceCapability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false) ||
                            (doc.Root.XPathSelectElements($"//x:Capability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false) ||
                            (doc.Root.XPathSelectElements($"//uap:Capability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false);
                            return result;
                        }
                        catch (Exception ex)
                        {
                            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => throw ex);
                        }
                    }
                }
            } catch (Exception ex1)
            {
                System.Diagnostics.Debug.WriteLine($"Xamarin.Essentials.Share : {ex1.GetType()} : {ex1.Message} \n {ex1.StackTrace}");
                Console.WriteLine($"Xamarin.Essentials.Share : {ex1.GetType()} : {ex1.Message} \n {ex1.StackTrace}");
            }
            return false;
        }

        public abstract partial class BasePlatformPermission : BasePermission
        {
            protected virtual Func<IEnumerable<string>> RequiredDeclarations { get; } = () => Array.Empty<string>();

            public override async Task<PermissionStatus> CheckStatusAsync()
            {
                await EnsureDeclaredAsync();
                return PermissionStatus.Granted;
            }

            public override Task<PermissionStatus> RequestAsync()
                => CheckStatusAsync();

            public override async Task EnsureDeclaredAsync()
            {
                foreach (var d in RequiredDeclarations())
                {
                    if (!await IsCapabilityDeclaredAsync(d))
                        throw new PermissionException($"You need to declare the capability `{d}` in your AppxManifest.xml file");
                }
            }

            public override bool ShouldShowRationale() => false;
        }

        public partial class Battery : BasePlatformPermission
        {
        }

        public partial class CalendarRead : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                new[] { "appointments" };
        }

        public partial class CalendarWrite : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                new[] { "appointments" };
        }

        public partial class Camera : BasePlatformPermission
        {
        }

        public partial class ContactsRead : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                new[] { "contacts" };

            public override async Task<PermissionStatus> CheckStatusAsync()
            {
                await EnsureDeclaredAsync();
                var accessStatus = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

                if (accessStatus == null)
                    return PermissionStatus.Denied;

                return PermissionStatus.Granted;
            }
        }

        public partial class ContactsWrite : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                   new[] { "contacts" };

            public override async Task<PermissionStatus> CheckStatusAsync()
            {
                await EnsureDeclaredAsync();
                var accessStatus = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);

                if (accessStatus == null)
                    return PermissionStatus.Denied;

                return PermissionStatus.Granted;
            }
        }

        public partial class Flashlight : BasePlatformPermission
        {
        }

        public partial class LaunchApp : BasePlatformPermission
        {
        }

        public partial class LocationWhenInUse : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                new[] { "location" };

            public override async Task<PermissionStatus> CheckStatusAsync()
            {
                await EnsureDeclaredAsync();
                return await RequestLocationPermissionAsync();
            }

            internal static async Task<PermissionStatus> RequestLocationPermissionAsync()
            {
                if (!MainThread.IsMainThread)
                    throw new PermissionException("Permission request must be invoked on main thread.");

                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        return PermissionStatus.Granted;
                    case GeolocationAccessStatus.Unspecified:
                        return PermissionStatus.Unknown;
                    default:
                        return PermissionStatus.Denied;
                }
            }
        }

        public partial class LocationAlways : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                new[] { "location" };

            public override async Task<PermissionStatus> CheckStatusAsync()
            {
                await EnsureDeclaredAsync();
                return await LocationWhenInUse.RequestLocationPermissionAsync();
            }
        }

        public partial class Maps : BasePlatformPermission
        {
        }

        public partial class Media : BasePlatformPermission
        {
        }

        public partial class Microphone : BasePlatformPermission
        {
            protected override Func<IEnumerable<string>> RequiredDeclarations => () =>
                new[] { "microphone" };
        }

        public partial class NetworkState : BasePlatformPermission
        {
        }

        public partial class Phone : BasePlatformPermission
        {
        }

        public partial class Photos : BasePlatformPermission
        {
        }

        public partial class Reminders : BasePlatformPermission
        {
        }

        public partial class Sensors : BasePlatformPermission
        {
            static readonly Guid activitySensorClassId = new Guid("9D9E0118-1807-4F2E-96E4-2CE57142E196");

            public override Task<PermissionStatus> CheckStatusAsync()
            {
                // Determine if the user has allowed access to activity sensors
                var deviceAccessInfo = DeviceAccessInformation.CreateFromDeviceClassId(activitySensorClassId);
                switch (deviceAccessInfo.CurrentStatus)
                {
                    case DeviceAccessStatus.Allowed:
                        return Task.FromResult(PermissionStatus.Granted);
                    case DeviceAccessStatus.DeniedBySystem:
                    case DeviceAccessStatus.DeniedByUser:
                        return Task.FromResult(PermissionStatus.Denied);
                    default:
                        return Task.FromResult(PermissionStatus.Unknown);
                }
            }
        }

        public partial class Sms : BasePlatformPermission
        {
        }

        public partial class Speech : BasePlatformPermission
        {
        }

        public partial class StorageRead : BasePlatformPermission
        {
        }

        public partial class StorageWrite : BasePlatformPermission
        {
        }

        public partial class Vibrate : BasePlatformPermission
        {
        }
    }
}
