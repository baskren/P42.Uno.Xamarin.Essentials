using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Samples.ViewModel;
using Windows.UI.Popups;
using Xamarin.Essentials;

namespace Samples.View;

public partial class Page<T> : Page
    where T : Page
{
    public string? Title { get; set; }

    public Page()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        // NavigationPage.SetBackButtonTitle(this, "Back");
        // if (DeviceInfo.Idiom == DeviceIdiom.Watch)
        //    NavigationPage.SetHasNavigationBar(this, false);

    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        OnAppearing();
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        OnDisappearing();
    }

    protected virtual void OnAppearing()
    {
        SetupBinding(DataContext);
    }

    protected virtual void OnDisappearing()
    {
        TearDownBinding(DataContext);
    }

    protected void SetupBinding(object bindingContext)
    {
        if (bindingContext is BaseViewModel vm)
        {
            vm.DoDisplayAlert += OnDisplayAlert;
            vm.DoNavigate += OnNavigate;
            vm.OnAppearing();
        }
    }

    protected void TearDownBinding(object bindingContext)
    {
        if (bindingContext is BaseViewModel vm)
        {
            vm.OnDisappearing();
            vm.DoDisplayAlert -= OnDisplayAlert;
            vm.DoNavigate -= OnNavigate;
        }
    }

    async Task OnDisplayAlert(string message)
    {
        var dialog = new MessageDialog(message)
        {
            Title = Title
        };
        await dialog.ShowAsync();
        await Task.Delay(2000);
    }

    async Task OnNavigate(BaseViewModel vm, bool showModal)
    {
        var name = vm.GetType().Name;
        name = name.Replace(nameof(ViewModel), "Page");

        var ns = GetType().Namespace;
        if (Type.GetType($"{ns}.{name}") is not Type pageType)
            throw new Exception($"could not find type matching {ns}.{name}.");

        if (Activator.CreateInstance(pageType) is not Page page)
            throw new Exception($"could not activate instance of {pageType}.");

        page.DataContext = vm;

        /*
        return showModal
            ? Navigation.PushModalAsync(page)
            : Navigation.PushAsync(page);
        */

        // await this.PushAsync(page);

        await Task.Delay(5);
    }
}
