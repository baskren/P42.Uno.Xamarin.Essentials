﻿using System;
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
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class NavigationPage : Page
    {
        protected Grid _contentGrid;

        public NavigationPage()
        {
            InitializeComponent();

            _contentGrid = _grid;

            _backButton.Click += OnBackButtonClicked;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Type type && (type == typeof(Page) || type.IsSubclassOf(typeof(Page)) ) )
            {
                var content = (Page)Activator.CreateInstance(type);
                Grid.SetRow(content, 1);
                content.HorizontalAlignment = HorizontalAlignment.Stretch;
                content.VerticalAlignment = VerticalAlignment.Stretch;
                _contentGrid.Children.Add(content);

                if (!string.IsNullOrEmpty(type.Name))
                {
                    var name = type.Name.Replace("Page", "");
                    _titleTextBlock.Text = name;
                }
            }
        }


        void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Frame is Frame localFrame)
            {
                if (localFrame.CanGoBack)
                {
                    localFrame.GoBack();
                    return;
                }
            }
            if (Window.Current.Content is Frame rootFrame)
            {
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                    return;
                }
            }
            if (Xamarin.Essentials.Platform.Window.Content is Frame platformRootFrame)
            {
                if (platformRootFrame.CanGoBack)
                    platformRootFrame.GoBack();
            }
        }
    }

    public partial class NavigationPage<T> : NavigationPage
        where T : Page
    {
        public NavigationPage()
            : base()
        {
            var content = Activator.CreateInstance<T>();
            Grid.SetRow(content, 1);
            content.HorizontalAlignment = HorizontalAlignment.Stretch;
            content.VerticalAlignment = VerticalAlignment.Stretch;
            _contentGrid.Children.Add(content);
        }
    }
}