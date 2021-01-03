using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    public static partial class TextToSpeech
    {
        static List<Locale> locales;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal static async Task<IEnumerable<Locale>> PlatformGetLocalesAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (locales != null)
                return locales;
            var result = new List<Locale>();
            //if (await WebAssemblyRuntime.InvokeAsync("UnoTextToSpeech_GetVoicesPromise()") is string javascriptResult && !string.IsNullOrWhiteSpace(javascriptResult))
            System.Diagnostics.Debug.WriteLine("TextToSpeech.PlatformGetLocalesAsync 1");
            if (WebAssemblyRuntime.InvokeJS("UnoTextToSpeech_GetVoices()") is string javascriptResult && !string.IsNullOrWhiteSpace(javascriptResult))
            {
                System.Diagnostics.Debug.WriteLine("TextToSpeech.PlatformGetLocalesAsync 2");
                var voices = javascriptResult.Split(';');
                System.Diagnostics.Debug.WriteLine("TextToSpeech.PlatformGetLocalesAsync 3");
                foreach (var voice in voices)
                {
                    var parts = voice.Split(':');
                    if (parts.Length > 1)
                    {
                        if (parts[1].Split('-') is string[] isoLang && isoLang.Length > 1)
                        {
                            var locale = new Locale(isoLang[0].Trim(), isoLang[1].Trim(), parts[0].Trim(), Guid.NewGuid().ToString());
                            if (parts[1].EndsWith("-DEFAULT"))
                                result.Insert(0, locale);
                            else
                                result.Add(locale);
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("TextToSpeech.PlatformGetLocalesAsync 4");
            }
            System.Diagnostics.Debug.WriteLine("TextToSpeech.PlatformGetLocalesAsync 5");
            return locales = result;
        }

#pragma warning disable CC0057 // Unused parameters
        internal static async Task PlatformSpeakAsync(string text, SpeechOptions options, CancellationToken cancelToken = default)
#pragma warning restore CC0057 // Unused parameters
        {
            var lang = options?.Locale?.Language ?? "en";
            var country = options?.Locale?.Country ?? "US";
            var name = options?.Locale?.Name;
            var volume = options?.Volume?.ToString() ?? "1";
            var pitch = options?.Pitch?.ToString() ?? "1";
            var script = $"UnoTextToSpeech_PerformSpeekPromise('{text}', '{name}', '{lang}-{country}', {volume}, {pitch})";
            //System.Console.WriteLine("TextToSpeech.PlatformSpeakAsync: script=["+script+"]");
            await WebAssemblyRuntime.InvokeAsync(script);
            //System.Diagnostics.Debug.WriteLine("TextToSpeech.PlatformSpeakAsync DONE");
            return;
        }
    }
}
