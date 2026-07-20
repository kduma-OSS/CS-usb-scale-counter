using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnitsNet;

namespace ScaleCounter.Core;

/// <summary>
/// Platform-agnostic scale state machine. Holds the current weight/status/connection,
/// applies the same weight-change debounce as the desktop app, and raises events.
/// Platform transports (e.g. the Android USB Host reader) derive from this and feed
/// raw HID report buffers via <see cref="OnBufferRead"/>.
///
/// The only platform seam is <see cref="Dispatch"/>, which marshals event delivery
/// onto the UI thread. The default is synchronous, which keeps this class fully
/// unit-testable without any platform dependency.
/// </summary>
public abstract class ScaleBase : IScale
{
    /// <summary>A weight update is ignored unless it differs by more than this (desktop parity).</summary>
    private static readonly Mass WeightChangeTolerance = Mass.FromOunces(0.05);

    // Start from an explicit zero (equivalent to the desktop's default(Mass)); a genuine
    // 0-weight reading therefore stays within tolerance and raises nothing.
    private Mass _weight = Mass.FromGrams(0);
    private StatusEnum _status;
    private bool _isConnected;

    public Mass Weight => _weight;
    public StatusEnum Status => _status;
    public bool IsConnected => _isConnected;

    public event EventHandler<Mass>? WeightChanged;
    public event EventHandler<StatusEnum>? StatusChanged;
    public event EventHandler<bool>? IsConnectedChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract void Start();
    public abstract void Stop();

    /// <summary>
    /// Marshals <paramref name="action"/> to the UI thread. Default is synchronous;
    /// the Android transport overrides this with MAUI's <c>IDispatcher</c>.
    /// </summary>
    protected virtual void Dispatch(Action action) => action();

    /// <summary>Parses a raw HID input report and updates status + weight.</summary>
    protected void OnBufferRead(byte[] buffer)
    {
        if (!ScaleReport.TryParse(buffer, out var report))
            return;

        SetStatus(report.Status);
        SetWeight(report.Weight);
    }

    protected void SetWeight(Mass value)
    {
        if (_weight.Equals(value, WeightChangeTolerance))
            return;

        _weight = value;
        Dispatch(() =>
        {
            WeightChanged?.Invoke(this, _weight);
            OnPropertyChanged(nameof(Weight));
        });
    }

    protected void SetStatus(StatusEnum value)
    {
        if (_status == value)
            return;

        _status = value;
        Dispatch(() =>
        {
            StatusChanged?.Invoke(this, _status);
            OnPropertyChanged(nameof(Status));
        });
    }

    protected void SetConnected(bool value)
    {
        if (_isConnected == value)
            return;

        _isConnected = value;
        Dispatch(() =>
        {
            IsConnectedChanged?.Invoke(this, _isConnected);
            OnPropertyChanged(nameof(IsConnected));
        });
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected virtual void Dispose(bool disposing) { }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
