using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class SecureStorageViewModel : BaseViewModel
{
    string? key;
    string? securedValue;

    public SecureStorageViewModel()
    {
        LoadCommand = new Xamarin.Essentials.Command(OnLoad);
        SaveCommand = new Xamarin.Essentials.Command(OnSave);
        RemoveCommand = new Xamarin.Essentials.Command(OnRemove);
        RemoveAllCommand = new Xamarin.Essentials.Command(OnRemoveAll);
    }

    public bool IsAvailable => SecureStorage.IsAvailable;

    public string? Key
    {
        get => key;
        set => SetProperty(ref key, value);
    }

    public string? SecuredValue
    {
        get => securedValue;
        set => SetProperty(ref securedValue, value);
    }

    public ICommand LoadCommand { get; }

    public ICommand SaveCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand RemoveAllCommand { get; }

    async void OnLoad()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            SecuredValue = await SecureStorage.GetAsync(Key) ?? string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
        }

        IsBusy = false;
    }

    async void OnSave()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            await SecureStorage.SetAsync(Key, SecuredValue);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
        }
        IsBusy = false;
    }

    async void OnRemove()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            SecureStorage.Remove(Key);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
        }
        IsBusy = false;
    }

    async void OnRemoveAll()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            SecureStorage.RemoveAll();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(ex.Message);
        }
        IsBusy = false;
    }
}
