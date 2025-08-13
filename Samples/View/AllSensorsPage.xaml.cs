using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AllSensorsPage : Page
{
    public AllSensorsPage()
    {
        this.InitializeComponent();
    }

    /*protected override void OnAppearing()
    {
        base.OnAppearing();

        SetupBinding(GridAccelerometer.DataContext);
        SetupBinding(GridCompass.DataContext);
        SetupBinding(GridGyro.DataContext);
        SetupBinding(GridMagnetometer.DataContext);
        SetupBinding(GridOrientation.DataContext);
        SetupBinding(GridBarometer.DataContext);
    }

    protected override void OnDisappearing()
    {
        TearDownBinding(GridAccelerometer.DataContext);
        TearDownBinding(GridCompass.DataContext);
        TearDownBinding(GridGyro.DataContext);
        TearDownBinding(GridMagnetometer.DataContext);
        TearDownBinding(GridOrientation.DataContext);
        TearDownBinding(GridBarometer.DataContext);
        base.OnDisappearing();
    }
    */
}
