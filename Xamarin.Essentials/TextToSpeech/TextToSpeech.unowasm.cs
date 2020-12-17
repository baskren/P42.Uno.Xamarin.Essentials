#if __WASM__
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.Foundation;

namespace Xamarin.Essentials.TextToSpeech
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
            if (await WebAssemblyRuntime.InvokeAsync("GetVoicesPromise()") is string javascriptResult && !string.IsNullOrWhiteSpace(javascriptResult))
            {
                var voices = javascriptResult.Split(';');
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
            }
            return locales = result;
        }

#pragma warning disable CC0057 // Unused parameters
        internal static async Task PlatformSpeakAsync(string text, SpeechOptions options, CancellationToken cancelToken = default)
#pragma warning restore CC0057 // Unused parameters
        {
            var lang = options?.Locale.Language ?? "en";
            var country = options?.Locale.Country ?? "US";
            var name = options?.Locale.Name;
            var volume = options?.Volume.ToString() ?? string.Empty;
            var pitch = options?.Pitch.ToString() ?? string.Empty;
            var script = $"UnoTextToSpeech_PerformSpeekPromise'({text}', '{name}', '{lang}-{country}', '{volume}', '{pitch}')";
            await WebAssemblyRuntime.InvokeAsync(script);
            return;
        }
    }
}
#endif
