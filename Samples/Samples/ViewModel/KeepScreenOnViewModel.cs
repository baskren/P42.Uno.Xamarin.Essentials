using System.Windows.Input;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class KeepScreenOnViewModel : BaseViewModel
{
    public KeepScreenOnViewModel()
    {
        RequestActiveCommand = new Xamarin.Essentials.Command(OnRequestActive);
        RequestReleaseCommand = new Xamarin.Essentials.Command(OnRequestRelease);
    }

    public bool IsActive => DeviceDisplay.KeepScreenOn;

    public bool IsNotActive => !IsActive;

    public ICommand RequestActiveCommand { get; }

    public ICommand RequestReleaseCommand { get; }

    void OnRequestActive()
    {
        DeviceDisplay.KeepScreenOn = true;

        OnPropertyChanged(nameof(IsActive));
    }

    void OnRequestRelease()
    {
        DeviceDisplay.KeepScreenOn = false;

        OnPropertyChanged(nameof(IsActive));
    }
}
