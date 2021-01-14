namespace Xamarin.Essentials
{
    static class SensorSpeedExtensions
    {
        /// <summary>
        /// Returns sampling frequency (in Hz)
        /// </summary>
        /// <param name="sensorSpeed"></param>
        /// <returns></returns>
        internal static uint ToPlatform(this SensorSpeed sensorSpeed)
        {
            switch (sensorSpeed)
            {
                case SensorSpeed.Fastest:
                    return 50;
                case SensorSpeed.Game:
                    return 25;
                case SensorSpeed.UI:
                    return 80;
                default:
                    return 5;
            }
        }
    }
}
