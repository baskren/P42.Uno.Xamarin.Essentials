using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class PhoneDialerViewModel : BaseViewModel
{
    string? phoneNumber;

    public PhoneDialerViewModel()
    {
        OpenPhoneDialerCommand = new Xamarin.Essentials.Command(OnOpenPhoneDialer);
    }

    public ICommand OpenPhoneDialerCommand { get; }

    public string? PhoneNumber
    {
        get => phoneNumber;
        set => SetProperty(ref phoneNumber, value);
    }

    async void OnOpenPhoneDialer()
    {
        try
        {
            PhoneDialer.Open(PhoneNumber);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync($"Dialer is not supported: {ex.Message}");
        }
    }
}
