namespace ScaleCounter.Core;

/// <summary>
/// A saved profile for one kind of weighed item: its calibrated per-item weight and tare,
/// the target quantity, and the raw calibration samples (kept so the preset can be
/// re-calibrated or refined later). Persisted per-app as JSON.
/// </summary>
public sealed class WeighedItemPreset
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "New preset";
    public double PerItemWeightGrams { get; set; }
    public double TareGrams { get; set; }
    public int TargetQuantity { get; set; } = 25;
    public List<CalibrationSample> Samples { get; set; } = new();
}
