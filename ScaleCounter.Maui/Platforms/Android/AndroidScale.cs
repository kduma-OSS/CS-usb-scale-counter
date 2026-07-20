using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Microsoft.Maui.Dispatching;
using ScaleCounter.Core;

namespace ScaleCounter.Maui;

/// <summary>
/// Android USB Host implementation of <see cref="IScale"/> for the Dymo M10 scale.
///
/// Enumerates the device by VID/PID, requests USB permission, claims the HID
/// interface and reads 6-byte HID input reports off the interrupt IN endpoint on a
/// background task, feeding them to the shared <see cref="ScaleBase"/> parser.
/// All state changes are marshalled to the UI thread via MAUI's <see cref="IDispatcher"/>.
///
/// This replaces the desktop HidClient/HidSharp transport, which has no Android backend.
/// </summary>
public sealed class AndroidScale : ScaleBase
{
	private const int DymoVendorId = 0x0922;
	private const int DymoProductId = 0x8003;
	private const string UsbPermissionAction = "sh.duma.scalecounter.USB_PERMISSION";

	private readonly IDispatcher _dispatcher;
	private readonly Context _context;
	private readonly UsbManager _usbManager;

	private UsbReceiver? _permissionReceiver;
	private UsbReceiver? _attachDetachReceiver;
	private UsbDeviceConnection? _connection;
	private UsbInterface? _interface;
	private UsbEndpoint? _endpointIn;
	private CancellationTokenSource? _readCts;
	private bool _started;

	public AndroidScale(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
		_context = Android.App.Application.Context;
		_usbManager = (UsbManager)_context.GetSystemService(Context.UsbService)!;
	}

	protected override void Dispatch(Action action) => _dispatcher.Dispatch(action);

	public override void Start()
	{
		if (_started)
			return;
		_started = true;

		_permissionReceiver = new UsbReceiver(OnPermissionResult);
		RegisterReceiver(_permissionReceiver, new IntentFilter(UsbPermissionAction));

		_attachDetachReceiver = new UsbReceiver(OnAttachDetach);
		var attachDetachFilter = new IntentFilter();
		attachDetachFilter.AddAction(UsbManager.ActionUsbDeviceAttached);
		attachDetachFilter.AddAction(UsbManager.ActionUsbDeviceDetached);
		RegisterReceiver(_attachDetachReceiver, attachDetachFilter);

		TryConnectExistingDevice();
	}

	public override void Stop()
	{
		if (!_started)
			return;
		_started = false;

		CloseDevice();

		if (_permissionReceiver != null)
		{
			_context.UnregisterReceiver(_permissionReceiver);
			_permissionReceiver = null;
		}

		if (_attachDetachReceiver != null)
		{
			_context.UnregisterReceiver(_attachDetachReceiver);
			_attachDetachReceiver = null;
		}
	}

	/// <summary>Called from <see cref="MainActivity"/> when a USB-attach intent launches the app.</summary>
	public void OnUsbDeviceAttached(UsbDevice device)
	{
		if (IsOurDevice(device))
			RequestOrOpen(device);
	}

	private void TryConnectExistingDevice()
	{
		var device = FindDevice();
		if (device != null)
			RequestOrOpen(device);
	}

	private UsbDevice? FindDevice()
	{
		var list = _usbManager.DeviceList;
		if (list == null)
			return null;

		foreach (var device in list.Values)
		{
			if (IsOurDevice(device))
				return device;
		}

		return null;
	}

	private static bool IsOurDevice(UsbDevice device)
		=> device.VendorId == DymoVendorId && device.ProductId == DymoProductId;

	private void RequestOrOpen(UsbDevice device)
	{
		if (_usbManager.HasPermission(device))
		{
			Open(device);
			return;
		}

		var flags = PendingIntentFlags.UpdateCurrent;
		if (OperatingSystem.IsAndroidVersionAtLeast(31))
			flags |= PendingIntentFlags.Mutable; // system fills in the device extra

		var intent = new Intent(UsbPermissionAction);
		intent.SetPackage(_context.PackageName);
		var pending = PendingIntent.GetBroadcast(_context, 0, intent, flags);
		_usbManager.RequestPermission(device, pending);
	}

	private void OnPermissionResult(Intent intent)
	{
		if (intent.Action != UsbPermissionAction)
			return;

		var device = intent.GetUsbDevice();
		var granted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);

		if (granted && device != null)
			Open(device);
	}

	private void OnAttachDetach(Intent intent)
	{
		var device = intent.GetUsbDevice();
		if (device == null || !IsOurDevice(device))
			return;

		if (intent.Action == UsbManager.ActionUsbDeviceAttached)
			RequestOrOpen(device);
		else if (intent.Action == UsbManager.ActionUsbDeviceDetached)
			CloseDevice();
	}

	private void Open(UsbDevice device)
	{
		var usbInterface = device.GetInterface(0); // HID interface

		UsbEndpoint? endpointIn = null;
		for (int i = 0; i < usbInterface.EndpointCount; i++)
		{
			var endpoint = usbInterface.GetEndpoint(i);
			if (endpoint == null)
				continue;

			if (endpoint.Direction == UsbAddressing.In && endpoint.Type == UsbAddressing.XferInterrupt)
			{
				endpointIn = endpoint;
				break;
			}
		}

		var connection = _usbManager.OpenDevice(device);
		if (connection == null || endpointIn == null)
			return;

		// forceClaim: steal the HID interface from the kernel input driver, otherwise reads are empty.
		connection.ClaimInterface(usbInterface, true);

		_connection = connection;
		_interface = usbInterface;
		_endpointIn = endpointIn;

		SetConnected(true);
		StartReadLoop();
	}

	private void StartReadLoop()
	{
		var connection = _connection;
		var endpoint = _endpointIn;
		if (connection == null || endpoint == null)
			return;

		_readCts = new CancellationTokenSource();
		var token = _readCts.Token;
		int packetSize = Math.Max(endpoint.MaxPacketSize, 8);

		_ = Task.Run(() =>
		{
			var buffer = new byte[packetSize];
			while (!token.IsCancellationRequested)
			{
				int read = connection.BulkTransfer(endpoint, buffer, buffer.Length, 200);
				if (read >= 6)
				{
					var report = new byte[read];
					Array.Copy(buffer, report, read);
					OnBufferRead(report);
				}
				// read < 0 => timeout; simply loop again.
			}
		}, token);
	}

	private void CloseDevice()
	{
		_readCts?.Cancel();
		_readCts?.Dispose();
		_readCts = null;

		if (_connection != null)
		{
			if (_interface != null)
				_connection.ReleaseInterface(_interface);
			_connection.Close();
		}

		_connection = null;
		_interface = null;
		_endpointIn = null;

		SetConnected(false);
	}

	private void RegisterReceiver(BroadcastReceiver receiver, IntentFilter filter)
	{
		// Android 13+ requires an explicit export flag for runtime-registered receivers.
		if (OperatingSystem.IsAndroidVersionAtLeast(33))
			_context.RegisterReceiver(receiver, filter, ReceiverFlags.NotExported);
		else
			_context.RegisterReceiver(receiver, filter);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
			Stop();
		base.Dispose(disposing);
	}

	/// <summary>Runtime-registered broadcast receiver that forwards intents to a callback.</summary>
	private sealed class UsbReceiver : BroadcastReceiver
	{
		private readonly Action<Intent> _onReceive;

		public UsbReceiver(Action<Intent> onReceive) => _onReceive = onReceive;

		public override void OnReceive(Context? context, Intent? intent)
		{
			if (intent != null)
				_onReceive(intent);
		}
	}
}
