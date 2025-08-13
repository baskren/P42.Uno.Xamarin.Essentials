using Xamarin.Essentials;

namespace Samples.ViewModel;

public class AppInfoViewModel : BaseViewModel
{
    public string AppPackageName => Xamarin.Essentials.AppInfo.PackageName;

    public string AppName => Xamarin.Essentials.AppInfo.Name;

    public string AppVersion => Xamarin.Essentials.AppInfo.VersionString;

    public string AppBuild => Xamarin.Essentials.AppInfo.BuildString;

    public string AppTheme => Xamarin.Essentials.AppInfo.RequestedTheme.ToString();

    public Xamarin.Essentials.Command ShowSettingsUICommand { get; }

    public AppInfoViewModel()
    {
        ShowSettingsUICommand = new Xamarin.Essentials.Command(() => Xamarin.Essentials.AppInfo.ShowSettingsUI());
    }
}
