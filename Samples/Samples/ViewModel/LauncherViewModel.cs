using System;
using System.IO;
using System.Windows.Input;
using Samples.Helpers;
using Xamarin.Essentials;

namespace Samples.ViewModel;

public class LauncherViewModel : BaseViewModel
{
    string? fileAttachmentName;
    string? fileAttachmentContents;

    public string? LaunchUri { get; set; }

    public ICommand LaunchCommand { get; }

    public ICommand CanLaunchCommand { get; }

    public ICommand LaunchMailCommand { get; }

    public ICommand LaunchBrowserCommand { get; }

    public ICommand LaunchFileCommand { get; }

    public LauncherViewModel()
    {
        LaunchCommand = new Xamarin.Essentials.Command(OnLaunch);
        LaunchMailCommand = new Xamarin.Essentials.Command(OnLaunchMail);
        LaunchBrowserCommand = new Xamarin.Essentials.Command(OnLaunchBrowser);
        CanLaunchCommand = new Xamarin.Essentials.Command(CanLaunch);
        LaunchFileCommand = new Xamarin.Essentials.Command<Microsoft.UI.Xaml.FrameworkElement>(OnFileRequest);
    }

    public string? FileAttachmentContents
    {
        get => fileAttachmentContents;
        set => SetProperty(ref fileAttachmentContents, value);
    }

    public string? FileAttachmentName
    {
        get => fileAttachmentName;
        set => SetProperty(ref fileAttachmentName, value);
    }

    async void OnLaunchBrowser()
    {
        await Launcher.OpenAsync("https://github.com/xamarin/Essentials");
    }

    async void OnLaunch()
    {
        try
        {
            await Launcher.OpenAsync(LaunchUri);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync($"Uri {LaunchUri} could not be launched: {ex}");
        }
    }

    async void OnLaunchMail()
    {
        await Launcher.OpenAsync("mailto:");
    }

    async void CanLaunch()
    {
        try
        {
            var canBeLaunched = await Launcher.CanOpenAsync(LaunchUri);
            await DisplayAlertAsync($"Uri {LaunchUri} can be launched: {canBeLaunched}");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync($"Uri {LaunchUri} could not be verified as launchable: {ex}");
        }
    }

    async void OnFileRequest(Microsoft.UI.Xaml.FrameworkElement? element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        if (!string.IsNullOrWhiteSpace(FileAttachmentContents))
        {
            // create a temprary file
            var fn = string.IsNullOrWhiteSpace(FileAttachmentName) ? "Attachment.txt" : FileAttachmentName.Trim();
            var file = Path.Combine(FileSystem.CacheDirectory, fn);
            File.WriteAllText(file, FileAttachmentContents);

            var rect = Xamarin.Essentials.RectangleExtensions.ToSystemRectangle(element.GetAbsoluteBounds());
            rect.Y += 40;
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(file),
                PresentationSourceBounds = rect
            });
        }
    }
}
