using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Uno.Foundation;

namespace Xamarin.Essentials
{
    // IMPORTANT NOTE:
    // Requires the use of Feature-Policy or Permissions-Policy in the HTTP response header
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Feature-Policy
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Feature_Policy
    public static partial class OrientationSensor
    {
        // keep around a reference so we can stop this same instance

        internal static bool IsSupported
        {
            get
            {
                return WebAssemblyRuntime.InvokeJS($"UnoOrientation_IsAvailable()") == "true";
            }
        }

        internal static void PlatformStart(SensorSpeed sensorSpeed)
        {
            var frequency = sensorSpeed.ToPlatform();
            WebAssemblyRuntime.InvokeJS($"UnoOrientation_Start({frequency})");
        }

        public static void DataUpdated(string json)
        {
            var reading = JsonConvert.DeserializeObject<List<double>>(json);
            var data = new OrientationSensorData(reading[0], reading[1], reading[2], reading[3]);
            OnChanged(data);
        }

        public static void LegacyDataUpdated(string json)
        {
            var reading = JsonConvert.DeserializeObject(json);

            // var data = new OrientationSensorData(reading.Quaternion.X, reading.Quaternion.Y, reading.Quaternion.Z, reading.Quaternion.W);
            // OnChanged(data);
        }

        internal static void PlatformStop()
        {
            WebAssemblyRuntime.InvokeJS("UnoOrientation_Stop()");
        }
    }
}
