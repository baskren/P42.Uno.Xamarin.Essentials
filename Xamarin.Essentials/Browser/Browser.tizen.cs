using System;
using System.Linq;
using System.Threading.Tasks;
using Tizen.Applications;

namespace Xamarin.Essentials
{
    public static partial class Browser
    {
        static async Task<bool> PlatformOpenAsync(Uri uri, BrowserLaunchOptions launchMode)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            await Permissions.EnsureDeclaredAsync<Permissions.LaunchApp>();

            var appControl = new AppControl
            {
                Operation = AppControlOperations.View,
                Uri = uri.AbsoluteUri
            };

            var hasMatches = AppControl.GetMatchedApplicationIds(appControl).Any();

            if (hasMatches)
                AppControl.SendLaunchRequest(appControl);

            return hasMatches;
        }
    }
}
