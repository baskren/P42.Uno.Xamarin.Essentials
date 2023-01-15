using Microsoft.UI.Xaml.Controls;

namespace Samples.View
{
    public partial class AppInfoPage : Page
    {
        public AppInfoPage()
        {
            InitializeComponent();

            _stackPanel.Children.Add(new TextBlock { Text = ".Id.FamilyName = " + Windows.ApplicationModel.Package.Current.Id.FamilyName });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.FullName = " + Windows.ApplicationModel.Package.Current.Id.FullName });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.Name = " + Windows.ApplicationModel.Package.Current.Id.Name });
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.Version = " + $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}" });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.Publisher = " + Windows.ApplicationModel.Package.Current.Id.Publisher });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.PublisherId = " + Windows.ApplicationModel.Package.Current.Id.PublisherId });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.ResourceId = " + Windows.ApplicationModel.Package.Current.Id.ResourceId });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.Author = " + Windows.ApplicationModel.Package.Current.Id.Author });
            _stackPanel.Children.Add(new TextBlock { Text = ".Id.ProductId = " + Windows.ApplicationModel.Package.Current.Id.ProductId });
        }
    }
}
