#if BROWSERWASM
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Xamarin.Essentials.JavaScripts.Uno
{
    public static partial class Page
    {
        [JSImport("globalThis.UnoGotoPage")]
        public static partial string Load(string url);

    }

    public static partial class FilePicker
    {
        [JSImport("globalThis.UnoFilePicker_Pick")]
        public static partial string Pick(string jsonOptions, string allowMultiple);

        [JSImport("globalThis.UnoFilePicker_Export")]
        public static partial string Export(string json, string fileName);
    }

    public static partial class FileSystem
    {
        [JSImport("globalThis.UnoFileSystem_GetDataFromJsFile")]
        [return: JSMarshalAs<JSType.Promise<JSType.String>>()]
        public static partial Task<string> GetDataFromJsFileAsync([JSMarshalAs<JSType.String>] string file);

        [JSImport("globalThis.UnoFileSystem_FileForAsset")]
        [return: JSMarshalAs<JSType.Promise<JSType.String>>()]
        public static partial Task<string> FileForAssetAsync([JSMarshalAs<JSType.String>] string assetPath);

        [JSImport("globalThis.UnoFileSystem_FileFromUrl")]
        [return: JSMarshalAs<JSType.Promise<JSType.String>>()]
        public static partial Task<string> FileForUrlAsync([JSMarshalAs<JSType.String>] string assetPath);
    }

    public static partial class Orientation
    {
        [JSImport("globalThis.UnoOrientation_IsAvailable")]
        public static partial bool IsAvailable();

        [JSImport("globalThis.UnoOrientation_Start")]
        public static partial string Start(int frequency);

        [JSImport("globalThis.UnoOrientation_Stop")]
        public static partial string Stop();
    }

    public static partial class ScreenShot
    {
        [JSImport("globalThis.UnoScreenshot_GetUrlPromise")]
        [return: JSMarshalAs<JSType.Promise<JSType.String>>()]
        public static partial Task<string> GetUrlAsync();
    }

    public static partial class JsonShare
    {
        [JSImport("globalThis.UnoShare_IsAvailable")]
        public static partial bool IsAvailable();

        [JSImport("globalThis.UnoShare_CanShare")]
        public static partial bool CanShare(string json);

        [JSImport("globalThis.UnoShare_Share")]
        public static partial string Share(string json);
    }

    public static partial class Sms
    {
        [JSImport("globalThis.UnoSms_Compose")]
        public static partial string Compose(string recipient, string message);
    }

    public static partial class TextToSpeech
    {
        [JSImport("globalThis.UnoTextToSpeech_GetVoices")]
        public static partial string GetVoices();

        [JSImport("globalThis.UnoTextToSpeech_PerformSpeakPromise")]
        [return: JSMarshalAs<JSType.Promise<JSType.String>>()]
        public static partial Task<string> SpeakAsync(string text, string name, string isoLang, string volume, string pitch);

    }
}

#endif
