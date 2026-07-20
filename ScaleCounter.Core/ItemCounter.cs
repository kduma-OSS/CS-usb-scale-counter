using UnitsNet;

namespace ScaleCounter.Core;

/// <summary>
/// Counts identical items by weight from a calibrated per-item weight and tare:
/// <c>count = round((measured − tare) / perItem)</c>.
///
/// Round-to-nearest (rather than the desktop app's fixed +5 g bias) is scale-invariant —
/// it works for both heavy and very light items — and is centred on the true weight.
/// </summary>
public sealed class ItemCounter
{
    /// <summary>Calibrated weight of a single item, in grams. Must be &gt; 0 to count.</summary>
    public double PerItemWeightGrams { get; set; }

    /// <summary>Calibrated tare (container weight), in grams.</summary>
    public double TareGrams { get; set; }

    /// <summary>Target quantity of items.</summary>
    public int TargetQuantity { get; set; } = 25;

    /// <summary>Copies a preset's calibration into this counter.</summary>
    public void Apply(WeighedItemPreset preset)
    {
        PerItemWeightGrams = preset.PerItemWeightGrams;
        TareGrams = preset.TareGrams;
        TargetQuantity = preset.TargetQuantity;
    }

    /// <summary>Calculates the item count and UI feedback for a measured weight.</summary>
    public CountResult Count(Mass measured)
    {
        int target = TargetQuantity;

        if (PerItemWeightGrams <= 0)
            return new CountResult(0, target, CountState.Uncalibrated, "Calibrate a preset first", "-");

        double net = measured.Grams - TareGrams;
        int count = (int)Math.Round(net / PerItemWeightGrams, MidpointRounding.AwayFromZero);
        if (count < 0) count = 0;

        if (count == target)
            return new CountResult(count, target, CountState.Exact,
                $"There is {count} items on scale!", "OK");

        if (count == 0)
            return new CountResult(count, target, CountState.Empty,
                "Place something on scale!", "Empty");

        if (count < target)
            return new CountResult(count, target, CountState.NotEnough,
                "Not Enough!", $"+ {target - count}");

        return new CountResult(count, target, CountState.TooMany,
            "Too Much!", $"- {count - target}");
    }
}
