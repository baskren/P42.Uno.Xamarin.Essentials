using System;

namespace Xamarin.Essentials;

public readonly struct DeviceIdiom : IEquatable<DeviceIdiom>
{
    readonly string deviceIdiom;

    public static DeviceIdiom Phone { get; } = new(nameof(Phone));

    public static DeviceIdiom Tablet { get; } = new(nameof(Tablet));

    public static DeviceIdiom Desktop { get; } = new(nameof(Desktop));

    public static DeviceIdiom TV { get; } = new(nameof(TV));

    public static DeviceIdiom Watch { get; } = new(nameof(Watch));

    public static DeviceIdiom Unknown { get; } = new(nameof(Unknown));

    public static DeviceIdiom Console { get; } = new(nameof(Console));

    public static DeviceIdiom IoT { get; } = new(nameof(IoT));

    DeviceIdiom(string deviceIdiom)
    {
        if (deviceIdiom == null)
            throw new ArgumentNullException(nameof(deviceIdiom));

        if (deviceIdiom.Length == 0)
            throw new ArgumentException(nameof(deviceIdiom));

        this.deviceIdiom = deviceIdiom;
    }

    public static DeviceIdiom Create(string deviceIdiom) => new(deviceIdiom);

    public bool Equals(DeviceIdiom other) =>
        Equals(other.deviceIdiom);

    internal bool Equals(string other) =>
        string.Equals(deviceIdiom, other, StringComparison.Ordinal);

    public override bool Equals(object obj) =>
        obj is DeviceIdiom && Equals((DeviceIdiom)obj);

    public override int GetHashCode() =>
        deviceIdiom == null ? 0 : deviceIdiom.GetHashCode();

    public override string ToString() =>
        deviceIdiom ?? string.Empty;

    public static bool operator ==(DeviceIdiom left, DeviceIdiom right) =>
        left.Equals(right);

    public static bool operator !=(DeviceIdiom left, DeviceIdiom right) =>
        !left.Equals(right);
}