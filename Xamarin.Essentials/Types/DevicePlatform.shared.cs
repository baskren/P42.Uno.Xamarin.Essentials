using System;

namespace Xamarin.Essentials;

public readonly struct DevicePlatform : IEquatable<DevicePlatform>
{
    readonly string devicePlatform;

    public static DevicePlatform Android { get; } = new(nameof(Android));

    public static DevicePlatform iOS { get; } = new(nameof(iOS));

    public static DevicePlatform MacOS { get; } = new(nameof(MacOS));

    public static DevicePlatform MacCatalyst { get; } = new(nameof(MacCatalyst));

    public static DevicePlatform tvOS { get; } = new(nameof(tvOS));

    public static DevicePlatform Tizen { get; } = new(nameof(Tizen));

    public static DevicePlatform UWP { get; } = new(nameof(UWP));

    public static DevicePlatform Windows { get; } = new(nameof(Windows));

    public static DevicePlatform WatchOS { get; } = new(nameof(WatchOS));

    public static DevicePlatform Unknown { get; } = new(nameof(Unknown));

    public static DevicePlatform WASM { get; } = new(nameof(WASM));

    DevicePlatform(string devicePlatform)
    {
        ArgumentNullException.ThrowIfNull(devicePlatform, nameof(devicePlatform));

        if (devicePlatform.Length == 0)
            throw new ArgumentException(nameof(devicePlatform));

        this.devicePlatform = devicePlatform;
    }

    public static DevicePlatform Create(string devicePlatform) => new(devicePlatform);

    public bool Equals(DevicePlatform other) =>
        Equals(other.devicePlatform);

    internal bool Equals(string other) =>
        string.Equals(devicePlatform, other, StringComparison.Ordinal);

    public override bool Equals(object obj) =>
        obj is DevicePlatform && Equals((DevicePlatform)obj);

    public override int GetHashCode() =>
        devicePlatform == null ? 0 : devicePlatform.GetHashCode();

    public override string ToString() =>
        devicePlatform ?? string.Empty;

    public static bool operator ==(DevicePlatform left, DevicePlatform right) =>
        left.Equals(right);

    public static bool operator !=(DevicePlatform left, DevicePlatform right) =>
        !left.Equals(right);
}