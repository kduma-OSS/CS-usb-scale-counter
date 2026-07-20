using Android.Content;
using Android.Hardware.Usb;

namespace ScaleCounter.Maui;

internal static class UsbIntentExtensions
{
	/// <summary>
	/// Reads the <see cref="UsbDevice"/> extra from an intent, using the typed API-33+
	/// overload where available and the legacy overload on older devices.
	/// </summary>
	public static UsbDevice? GetUsbDevice(this Intent intent)
	{
		if (OperatingSystem.IsAndroidVersionAtLeast(33))
			return intent.GetParcelableExtra(UsbManager.ExtraDevice, Java.Lang.Class.FromType(typeof(UsbDevice))) as UsbDevice;

#pragma warning disable CA1422 // legacy overload, intentional for < API 33
		return intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;
#pragma warning restore CA1422
	}
}
