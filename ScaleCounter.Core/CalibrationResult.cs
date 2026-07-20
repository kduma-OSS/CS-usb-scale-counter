namespace ScaleCounter.Core;

/// <summary>
/// Outcome of <see cref="Calibration.Fit"/>: the estimated per-item weight and tare
/// (container weight), plus an R² goodness-of-fit. <see cref="IsValid"/> is false when the
/// samples don't determine a positive per-item weight — see <see cref="Error"/> for why.
/// </summary>
public readonly record struct CalibrationResult(
    bool IsValid,
    double PerItemWeightGrams,
    double TareGrams,
    double RSquared,
    string? Error)
{
    public static CalibrationResult Invalid(string error) => new(false, 0, 0, 0, error);
}
