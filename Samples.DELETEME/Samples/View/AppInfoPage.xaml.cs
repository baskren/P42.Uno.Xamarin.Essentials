using Microsoft.UI.Xaml.Controls;

namespace Samples.View;

public partial class AppInfoPage : Page
{
    public AppInfoPage()
    {
        InitializeComponent();

        var id = Windows.ApplicationModel.Package.Current.Id;
        _stackPanel.Children.Add(new TextBlock { Text = ".Id.FamilyName = " + id.FamilyName });
        _stackPanel.Children.Add(new TextBlock { Text = ".Id.FullName = " + id.FullName });
        _stackPanel.Children.Add(new TextBlock { Text = ".Id.Name = " + id.Name });
        var version = id.Version;
        _stackPanel.Children.Add(new TextBlock { Text = ".Id.Version = " + $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}" });

        // _stackPanel.Children.Add(new TextBlock { Text = ".Id.Publisher = " + id.Publisher });
        // _stackPanel.Children.Add(new TextBlock { Text = ".Id.PublisherId = " + id.PublisherId });
        // _stackPanel.Children.Add(new TextBlock { Text = ".Id.ResourceId = " + id.ResourceId });
        // _stackPanel.Children.Add(new TextBlock { Text = ".Id.Author = " + id.Author });
        // _stackPanel.Children.Add(new TextBlock { Text = ".Id.ProductId = " + id.ProductId });
    }
}
