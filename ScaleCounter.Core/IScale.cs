using System.ComponentModel;
using UnitsNet;

namespace ScaleCounter.Core;

/// <summary>
/// Platform-agnostic view of a connected scale. Mirrors the public surface of the
/// desktop ScaleLib.Scale (weight / status / connection + change events) so that
/// view-models can stay platform-independent. The Android transport implements this
/// on top of the USB Host API; tests can implement it trivially.
/// </summary>
public interface IScale : INotifyPropertyChanged, IDisposable
{
    /// <summary>Latest measured weight.</summary>
    Mass Weight { get; }

    /// <summary>Latest reported status.</summary>
    StatusEnum Status { get; }

    /// <summary>Whether a scale is currently connected and open.</summary>
    bool IsConnected { get; }

    event EventHandler<Mass> WeightChanged;
    event EventHandler<StatusEnum> StatusChanged;
    event EventHandler<bool> IsConnectedChanged;

    /// <summary>Begin device discovery, permission handling and attach/detach wiring.</summary>
    void Start();

    /// <summary>Stop reading and release the device.</summary>
    void Stop();
}
