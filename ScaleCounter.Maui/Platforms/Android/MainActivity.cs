using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using ScaleCounter.Core;
using ScaleCounter.Maui.Services;

namespace ScaleCounter.Maui;

[Activity(
	Theme = "@style/Maui.SplashTheme",
	MainLauncher = true,
	LaunchMode = LaunchMode.SingleTask,
	ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached })]
[MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
// Open a ".uscpreset" file from a file manager / share sheet. Android matches on MIME type or
// on the URI path, so we register both a typed and an untyped variant, with a few path-depth
// patterns (a documented workaround for Android's greedy pathPattern with multiple dots).
[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
	DataSchemes = new[] { "content", "file" },
	DataHost = "*",
	DataPathPatterns = new[] { ".*\\.uscpreset", ".*\\..*\\.uscpreset", ".*\\..*\\..*\\.uscpreset" })]
[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
	DataSchemes = new[] { "content", "file" },
	DataHost = "*",
	DataMimeType = "*/*",
	DataPathPatterns = new[] { ".*\\.uscpreset", ".*\\..*\\.uscpreset", ".*\\..*\\..*\\.uscpreset" })]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		HandleUsbIntent(Intent);
		HandleFileIntent(Intent);
	}

	protected override void OnNewIntent(Intent? intent)
	{
		base.OnNewIntent(intent);
		HandleUsbIntent(intent);
		HandleFileIntent(intent);
	}

	private static void HandleUsbIntent(Intent? intent)
	{
		if (intent?.Action != UsbManager.ActionUsbDeviceAttached)
			return;

		var device = intent.GetUsbDevice();
		if (device == null)
			return;

		var scale = IPlatformApplication.Current?.Services.GetService<IScale>() as AndroidScale;
		scale?.OnUsbDeviceAttached(device);
	}

	private static void HandleFileIntent(Intent? intent)
	{
		if (intent?.Action != Intent.ActionView || intent.Data == null)
			return;

		try
		{
			string json;
			using (var stream = Android.App.Application.Context.ContentResolver?.OpenInputStream(intent.Data))
			{
				if (stream == null)
					return;
				using var reader = new StreamReader(stream);
				json = reader.ReadToEnd();
			}

			var store = IPlatformApplication.Current?.Services.GetService<IPresetStore>();
			int count = store?.ImportJson(json) ?? 0;

			Android.Widget.Toast.MakeText(
				Android.App.Application.Context,
				count > 0 ? $"Imported {count} preset(s)" : "No presets found in the file",
				Android.Widget.ToastLength.Long)?.Show();
		}
		catch
		{
			// Ignore a malformed or unreadable file; the manual Import picker still works.
		}
	}
}
