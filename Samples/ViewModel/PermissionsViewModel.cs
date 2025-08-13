using System.Collections.Generic;
using Samples.Model;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class PermissionsViewModel : BaseViewModel
{
    public static List<PermissionItem> PermissionItems =>
        [
            new("Battery", new Permissions.Battery()),
            new("Calendar (Read)", new Permissions.CalendarRead()),
            new("Calendar (Write)", new Permissions.CalendarWrite()),
            new("Camera", new Permissions.Camera()),
            new("Contacts (Read)", new Permissions.ContactsRead()),
            new("Contacts (Write)", new Permissions.ContactsWrite()),
            new("Flashlight", new Permissions.Flashlight()),
            new("Launch Apps", new Permissions.LaunchApp()),
            new("Location (Always)", new Permissions.LocationAlways()),
            new("Location (Only When In Use)", new Permissions.LocationWhenInUse()),
            new("Maps", new Permissions.Maps()),
            new("Media Library", new Permissions.Media()),
            new("Microphone", new Permissions.Microphone()),
            new("Network State", new Permissions.NetworkState()),
            new("Phone", new Permissions.Phone()),
            new("Photos", new Permissions.Photos()),
            new("Reminders", new Permissions.Reminders()),
            new("Sensors", new Permissions.Sensors()),
            new("SMS", new Permissions.Sms()),
            new("Speech", new Permissions.Speech()),
            new("Storage (Read)", new Permissions.StorageRead()),
            new("Storage (Write)", new Permissions.StorageWrite()),
            new("Vibrate", new Permissions.Vibrate())
        ];
}
