namespace ScaleCounter.Core;

/// <summary>
/// A single calibration measurement: a known item count and the total weight it produced
/// (including any container). A sample with <see cref="Quantity"/> 0 measures the tare.
/// Mutable positional record struct so it round-trips through JSON serializers.
/// </summary>
public record struct CalibrationSample(int Quantity, double WeightGrams);
