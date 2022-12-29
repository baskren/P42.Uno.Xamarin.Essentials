using System.Globalization;
using System.Threading.Tasks;
using Tizen.Applications;

namespace Xamarin.Essentials
{
    public static partial class Map
    {
        internal static async Task PlatformOpenMapsAsync(double latitude, double longitude, MapLaunchOptions options)
        {
            await Permissions.EnsureDeclaredAsync<Permissions.LaunchApp>();

            var appControl = new AppControl
            {
                Operation = AppControlOperations.View,
                Uri = "geo:",
            };

            appControl.Uri += $"{latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}";

            AppControl.SendLaunchRequest(appControl);

            return;
        }

        internal static async Task PlatformOpenMapsAsync(Placemark placemark, MapLaunchOptions options)
        {
            await Permissions.EnsureDeclaredAsync<Permissions.LaunchApp>();

            var appControl = new AppControl
            {
                Operation = AppControlOperations.Pick,
                Uri = "geo:",
            };

            appControl.Uri += $"0,0?q={placemark.GetEscapedAddress()}";

            AppControl.SendLaunchRequest(appControl);

            return;
        }
    }
}
