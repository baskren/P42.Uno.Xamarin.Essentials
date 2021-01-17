using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilePickerPage : BasePage
    {
        public FilePickerPage()
        {
            this.InitializeComponent();

            if (DataContext is ViewModel.FilePickerViewModel viewModel)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

        }

        async void OnExportFileClicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.FilePickerViewModel viewModel && viewModel.LastSingleFileResult is FileResult fileResult)
            {
                await FilePicker.ExportAsync(fileResult, new SaveOptions
                {
                    SuggestedFileName = _exportFileName.Text.Trim()
                });
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DataContext is ViewModel.FilePickerViewModel viewModel)
            {
                if (e.PropertyName == nameof(ViewModel.FilePickerViewModel.LastSingleFileResult))
                {
                    _exportFileName.IsEnabled = _exportFileButton.IsEnabled = viewModel.LastSingleFileResult != null;
                    _exportFileName.Text = string.IsNullOrWhiteSpace(viewModel.LastSingleFileResult?.FileName)
                        ? string.Empty
                        : "export_" + viewModel.LastSingleFileResult.FileName;
                    _image.Source = viewModel.Image;
                }
                else if (e.PropertyName == nameof(ViewModel.FilePickerViewModel.Image))
                {
                    _image.Source = viewModel.Image;
                }
            }
        }
    }
}
