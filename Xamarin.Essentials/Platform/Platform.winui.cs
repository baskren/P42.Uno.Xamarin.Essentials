﻿using Windows.ApplicationModel.Activation;

namespace Xamarin.Essentials
{
    public static partial class Platform
    {
        internal const string AppManifestFilename = "AppxManifest.xml";
        //internal const string AppManifestFilename = "Package.appxmanifest";
        internal const string AppManifestXmlns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
        internal const string AppManifestUapXmlns = "http://schemas.microsoft.com/appx/manifest/uap/windows10";

        public static string MapServiceToken { get; set; }

        public static async void OnLaunched(LaunchActivatedEventArgs e)
            => await AppActions.OnLaunched(e);
    }
}
