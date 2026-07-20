namespace ScaleCounter.Core;

/// <summary>High-level state of the current count relative to the expected quantity.</summary>
public enum CountState
{
    /// <summary>No usable calibration (per-item weight not set).</summary>
    Uncalibrated,

    /// <summary>Nothing (meaningful) on the scale.</summary>
    Empty,

    /// <summary>Fewer items than expected.</summary>
    NotEnough,

    /// <summary>Exactly the expected quantity.</summary>
    Exact,

    /// <summary>More items than expected.</summary>
    TooMany
}

/// <summary>
/// Outcome of an <see cref="ItemCounter"/> calculation. <see cref="Message"/> and
/// <see cref="Diff"/> reproduce the exact strings shown by the desktop app.
/// </summary>
public readonly record struct CountResult(
    int Count,
    int Expected,
    CountState State,
    string Message,
    string Diff);
