using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class VibrationViewModel : BaseViewModel
{
    int duration = 500;
    bool isSupported = true;

    public VibrationViewModel()
    {
        VibrateCommand = new Xamarin.Essentials.Command(OnVibrate);
        CancelCommand = new Xamarin.Essentials.Command(OnCancel);
    }

    public ICommand VibrateCommand { get; }

    public ICommand CancelCommand { get; }

    public int Duration
    {
        get => duration;
        set => SetProperty(ref duration, value);
    }

    public bool IsSupported
    {
        get => isSupported;
        set => SetProperty(ref isSupported, value);
    }

    public override void OnDisappearing()
    {
        OnCancel();

        base.OnDisappearing();
    }

    void OnVibrate()
    {
        try
        {
            Vibration.Vibrate(duration);
        }
        catch (FeatureNotSupportedException)
        {
            IsSupported = false;
        }
        catch (Exception ex)
        {
            DisplayAlertAsync($"Unable to vibrate: {ex.Message}");
        }
    }

    void OnCancel()
    {
        try
        {
            Vibration.Cancel();
        }
        catch (FeatureNotSupportedException)
        {
            IsSupported = false;
        }
        catch (Exception ex)
        {
            DisplayAlertAsync($"Unable to cancel: {ex.Message}");
        }
    }
}
