using System;
using System.Threading.Tasks;
using P42.Uno.AsyncNavigation;
using Samples.ViewModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

namespace Samples.View
{
    public partial class BasePage : Page
    {
        public string Title { get; set; }

        public BasePage()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            NavigationPage.SetBackButtonTitle(this,"Back");
            if (DeviceInfo.Idiom == DeviceIdiom.Watch)
                NavigationPage.SetHasNavigationBar(this, false);
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
            name = name.Replace("ViewModel", "Page");

            var ns = GetType().Namespace;
            var pageType = Type.GetType($"{ns}.{name}");

            var page = (Page)Activator.CreateInstance(pageType);
            page.DataContext = vm;

            /*
            return showModal
                ? Navigation.PushModalAsync(page)
                : Navigation.PushAsync(page);
            */
            await this.PushAsync(page);
        }
    }
}
