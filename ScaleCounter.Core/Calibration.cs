namespace ScaleCounter.Core;

/// <summary>
/// Estimates per-item weight and tare from calibration samples using the linear model
/// <c>weight = tare + quantity · perItem</c> (ordinary least squares).
///
/// Measuring several different quantities lets the fit recover BOTH the per-item weight
/// (slope) and the tare (intercept), and averages out per-item variance and measurement noise.
/// </summary>
public static class Calibration
{
    public static CalibrationResult Fit(IReadOnlyList<CalibrationSample> samples)
    {
        if (samples == null || samples.Count == 0)
            return CalibrationResult.Invalid("Add at least one measurement.");

        // Keep only usable points (non-negative quantity, finite weight).
        var points = new List<CalibrationSample>();
        foreach (var s in samples)
        {
            if (s.Quantity >= 0 && !double.IsNaN(s.WeightGrams) && !double.IsInfinity(s.WeightGrams))
                points.Add(s);
        }

        if (points.Count == 0)
            return CalibrationResult.Invalid("Add at least one valid measurement.");

        // We need at least one point with items on the scale to learn a per-item weight.
        var quantities = new HashSet<int>();
        bool hasNonZero = false;
        foreach (var p in points)
        {
            quantities.Add(p.Quantity);
            if (p.Quantity > 0) hasNonZero = true;
        }
        if (!hasNonZero)
            return CalibrationResult.Invalid("Add a measurement with a known item count (quantity > 0).");

        double perItem, tare;

        if (quantities.Count >= 2)
        {
            // Ordinary least squares: slope = perItem, intercept = tare.
            int n = points.Count;
            double sumX = 0, sumY = 0, sumXX = 0, sumXY = 0;
            foreach (var p in points)
            {
                double x = p.Quantity;
                sumX += x;
                sumY += p.WeightGrams;
                sumXX += x * x;
                sumXY += x * p.WeightGrams;
            }

            double denom = n * sumXX - sumX * sumX;
            if (Math.Abs(denom) < 1e-9)
                return CalibrationResult.Invalid("Measurements are degenerate; vary the item counts.");

            perItem = (n * sumXY - sumX * sumY) / denom;
            tare = (sumY - perItem * sumX) / n;
        }
        else
        {
            // Only one distinct quantity (> 0): assume no tare, average the weights.
            double sumY = 0;
            foreach (var p in points) sumY += p.WeightGrams;
            perItem = (sumY / points.Count) / points[0].Quantity;
            tare = 0;
        }

        if (perItem <= 0)
            return CalibrationResult.Invalid(
                "Calibration produced a non-positive item weight; check the measurements.");

        return new CalibrationResult(true, perItem, tare, ComputeRSquared(points, perItem, tare), null);
    }

    private static double ComputeRSquared(List<CalibrationSample> points, double perItem, double tare)
    {
        double sumY = 0;
        foreach (var p in points) sumY += p.WeightGrams;
        double meanY = sumY / points.Count;

        double ssRes = 0, ssTot = 0;
        foreach (var p in points)
        {
            double predicted = tare + perItem * p.Quantity;
            double res = p.WeightGrams - predicted;
            double dev = p.WeightGrams - meanY;
            ssRes += res * res;
            ssTot += dev * dev;
        }

        if (ssTot < 1e-12) return 1.0; // all weights equal — perfect fit by convention
        return 1.0 - ssRes / ssTot;
    }
}
