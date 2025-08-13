using System.Collections.Generic;
using System.Linq;
using Samples.Model;
using Samples.View;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class HomeViewModel : BaseViewModel
{
    bool alreadyAppeared;
    SampleItem[] samples;
    IEnumerable<SampleItem> filteredItems;
    string filterText;

    public HomeViewModel()
    {
        alreadyAppeared = false;
        samples = new SampleItem[]
        {
            new SampleItem(
                "📏",
                "Accelerometer",
                typeof(AccelerometerPage),
                "Retrieve acceleration data of the device in 3D space.",
                ["accelerometer", "sensors", "hardware", "device"]),
            new SampleItem(
                "📏",
                "All Sensors",
                typeof(AllSensorsPage),
                "Have a look at the accelerometer, barometer, compass, gyroscope, magnetometer, and orientation sensors.",
                ["accelerometer", "barometer", "compass", "gyroscope", "magnetometer", "orientation", "sensors", "hardware", "device"]),
            new SampleItem(
                "📦",
                "App Info",
                typeof(AppInfoPage),
                "Find out about the app with ease.",
                ["app", "info"]),
            new SampleItem(
                "📏",
                "Barometer",
                typeof(BarometerPage),
                "Easily detect pressure level, via the device barometer.",
                ["barometer", "hardware", "device", "sensor"]),
            new SampleItem(
                "🔋",
                "Battery",
                typeof(BatteryPage),
                "Easily detect battery level, source, and state.",
                ["battery", "hardware", "device"]),
            new SampleItem(
                "🌐",
                "Browser",
                typeof(BrowserPage),
                "Quickly and easily open a browser to a specific website.",
                ["browser", "web", "internet"]),
            new SampleItem(
                "📋",
                "Clipboard",
                typeof(ClipboardPage),
                "Quickly and easily use the clipboard.",
                ["clipboard", "copy", "paste"]),
            new SampleItem(
                "🎨",
                "Color Converters",
                typeof(ColorConvertersPage),
                "Convert and adjust colors.",
                ["color", "drawing", "style"]),
            new SampleItem(
                "🧭",
                "Compass",
                typeof(CompassPage),
                "Monitor compass for changes.",
                new[] { "compass", "sensors", "hardware", "device" }),
            new SampleItem(
                "📶",
                "Connectivity",
                typeof(ConnectivityPage),
                "Check connectivity state and detect changes.",
                ["connectivity", "internet", "wifi"]),
            new SampleItem(
                "👶",
                "Contacts",
                typeof(ContactsPage),
                "Get and add contacts in your device.",
                ["contacts", "people", "device"]),
            new SampleItem(
                "📱",
                "Device Info",
                typeof(DeviceInfoPage),
                "Find out about the device with ease.",
                ["hardware", "device", "info", "screen", "display", "orientation", "rotation"]),
            
            new SampleItem(
                "📧",
                "Email",
                typeof(EmailPage),
                "Easily send email messages.",
                ["email", "share", "communication", "message"]),
            
            new SampleItem(
                "📁",
                "File Picker",
                typeof(FilePickerPage),
                "Easily pick files from storage.",
                ["files", "picking", "filesystem", "storage"]),
            new SampleItem(
                "📁",
                "File System",
                typeof(FileSystemPage),
                "Easily save files to app data.",
                ["files", "directory", "filesystem", "storage"]),

            new SampleItem(
                "🔦",
                "Flashlight",
                typeof(FlashlightPage),
                "A simple way to turn the flashlight on/off.",
                ["flashlight", "torch", "hardware", "flash", "device"]),
            new SampleItem(
                "📍",
                "Geocoding",
                typeof(GeocodingPage),
                "Easily geocode and reverse geocoding.",
                ["geocoding", "geolocation", "position", "address", "mapping"]),
            new SampleItem(
                "📍",
                "Geolocation",
                typeof(GeolocationPage),
                "Quickly get the current location.",
                ["geolocation", "position", "address", "mapping"]),
            new SampleItem(
                "💤",
                "Keep Screen On",
                typeof(KeepScreenOnPage),
                "Keep the device screen awake.",
                ["screen", "awake", "sleep"]),
            new SampleItem(
                "📏",
                "Launcher",
                typeof(LauncherPage),
                "Launch other apps via Uri",
                ["launcher", "app", "run"]),
            new SampleItem(
                "📏",
                "Gyroscope",
                typeof(GyroscopePage),
                "Retrieve rotation around the device's three primary axes.",
                ["gyroscope", "sensors", "hardware", "device"]),
            new SampleItem(
                "📏",
                "Magnetometer",
                typeof(MagnetometerPage),
                "Detect device's orientation relative to Earth's magnetic field.",
                ["compass", "magnetometer", "sensors", "hardware", "device"]),
            new SampleItem(
                "🗺",
                "Launch Maps",
                typeof(MapsPage),
                "Easily launch maps with coordinates.",
                ["geocoding", "geolocation", "position", "address", "mapping", "maps", "route", "navigation"]),

            new SampleItem(
                "📏",
                "Orientation Sensor",
                typeof(OrientationSensorPage),
                "Retrieve orientation of the device in 3D space.",
                ["orientation", "sensors", "hardware", "device"]),
            new SampleItem(
                "📷",
                "Media Picker",
                typeof(MediaPickerPage),
                "Pick or capture a photo or video.",
                ["media", "picker", "video", "picture", "photo", "image", "movie"]),
            new SampleItem(
                "🔒",
                "Permissions",
                typeof(PermissionsPage),
                "Request various permissions.",
                ["permissions"]),
            new SampleItem(
                "📞",
                "Phone Dialer",
                typeof(PhoneDialerPage),
                "Easily open the phone dialer.",
                ["phone", "dialer", "communication", "call"]),
            new SampleItem(
                "⚙️",
                "Preferences",
                typeof(PreferencesPage),
                "Quickly and easily add persistent preferences.",
                ["settings", "preferences", "prefs", "storage"]),
            new SampleItem(
                "📷",
                "Screenshot",
                typeof(ScreenshotPage),
                "Quickly and easily take screenshots of your app.",
                ["screenshot", "picture", "media", "display"]),
            new SampleItem(
                "🔒",
                "Secure Storage",
                typeof(SecureStoragePage),
                "Securely store data.",
                ["settings", "preferences", "prefs", "security", "storage"]),
            new SampleItem(
                "📲",
                "Share",
                typeof(SharePage),
                "Send text, website uris and files to other apps.",
                ["data", "transfer", "share", "communication"]),
            new SampleItem(
                "💬",
                "SMS",
                typeof(SMSPage),
                "Easily send SMS messages.",
                ["sms", "message", "text", "communication", "share"]),
            new SampleItem(
                "🔊",
                "Text To Speech",
                typeof(TextToSpeechPage),
                "Vocalize text on the device.",
                ["text", "message", "speech", "communication"]),
            new SampleItem(
                "🌡",
                "Unit Converters",
                typeof(UnitConvertersPage),
                "Easily converter different units.",
                ["units", "converters", "calculations"]),
            new SampleItem(
                "📳",
                "Vibration",
                typeof(VibrationPage),
                "Quickly and easily make the device vibrate.",
                ["vibration", "vibrate", "hardware", "device"]),
            new SampleItem(
                "📳",
                "Haptic Feedback",
                typeof(HapticFeedbackPage),
                "Quickly and easily make the device provide haptic feedback",
                ["haptic", "feedback", "hardware", "device"]),
            new SampleItem(
                "🔓",
                "Web Authenticator",
                typeof(WebAuthenticatorPage),
                "Quickly and easily authenticate and wait for a callback.",
                ["auth", "authenticate", "authenticator", "web", "webauth"]),
        };
        filteredItems = samples;
        filterText = string.Empty;
    }

    public IEnumerable<SampleItem> FilteredItems
    {
        get => filteredItems;
        private set => SetProperty(ref filteredItems, value);
    }

    public string FilterText
    {
        get => filterText;
        set
        {
            SetProperty(ref filterText, value);
            FilteredItems = Filter(samples, value);
        }
    }

    public override void OnAppearing()
    {
        base.OnAppearing();

        if (!alreadyAppeared)
        {
            alreadyAppeared = true;

            if (VersionTracking.IsFirstLaunchEver)
            {
                DisplayAlertAsync("Welcome to the Samples! This is the first time you are launching the app!");
            }
            else if (VersionTracking.IsFirstLaunchForCurrentVersion)
            {
                var count = VersionTracking.VersionHistory.Count();
                DisplayAlertAsync($"Welcome to the NEW Samples! You have tried {count} versions.");
            }
        }
    }

    static IEnumerable<SampleItem> Filter(IEnumerable<SampleItem> samples, string filterText)
    {
        if (!string.IsNullOrWhiteSpace(filterText))
        {
            var lower = filterText.ToLowerInvariant();
            samples = samples.Where(s =>
            {
                var tags = s.Tags
                    .Union(new[] { s.Name })
                    .Select(t => t.ToLowerInvariant());
                return tags.Any(t => t.Contains(lower));
            });
        }

        return samples.OrderBy(s => s.Name);
    }
}
