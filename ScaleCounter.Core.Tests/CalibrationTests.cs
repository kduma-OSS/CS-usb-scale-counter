using Xunit;

namespace ScaleCounter.Core.Tests;

public class CalibrationTests
{
    [Fact]
    public void Fit_PerfectLinearData_RecoversPerItemAndTare()
    {
        // weight = 50 (tare) + quantity * 23.2
        var samples = new[]
        {
            new CalibrationSample(0, 50),
            new CalibrationSample(10, 282),
            new CalibrationSample(25, 630)
        };

        var r = Calibration.Fit(samples);

        Assert.True(r.IsValid);
        Assert.Equal(23.2, r.PerItemWeightGrams, 6);
        Assert.Equal(50, r.TareGrams, 6);
        Assert.Equal(1.0, r.RSquared, 6);
    }

    [Fact]
    public void Fit_ZeroPointPlusOne_MeasuresTareDirectly()
    {
        var samples = new[]
        {
            new CalibrationSample(0, 40),   // empty container
            new CalibrationSample(10, 272)  // + 10 items
        };

        var r = Calibration.Fit(samples);

        Assert.True(r.IsValid);
        Assert.Equal(23.2, r.PerItemWeightGrams, 6);
        Assert.Equal(40, r.TareGrams, 6);
    }

    [Fact]
    public void Fit_SingleNonZeroPoint_AssumesNoTare()
    {
        var r = Calibration.Fit(new[] { new CalibrationSample(10, 232) });

        Assert.True(r.IsValid);
        Assert.Equal(23.2, r.PerItemWeightGrams, 6);
        Assert.Equal(0, r.TareGrams, 6);
    }

    [Fact]
    public void Fit_NoisyData_IsCloseAndWellFit()
    {
        var samples = new[]
        {
            new CalibrationSample(10, 234),
            new CalibrationSample(20, 465),
            new CalibrationSample(30, 699),
            new CalibrationSample(40, 931)
        };

        var r = Calibration.Fit(samples);

        Assert.True(r.IsValid);
        Assert.InRange(r.PerItemWeightGrams, 22.5, 24.0);
        Assert.True(r.RSquared > 0.99);
    }

    [Fact]
    public void Fit_NoSamples_IsInvalid()
    {
        var r = Calibration.Fit(System.Array.Empty<CalibrationSample>());
        Assert.False(r.IsValid);
        Assert.NotNull(r.Error);
    }

    [Fact]
    public void Fit_OnlyTarePoints_IsInvalid()
    {
        var r = Calibration.Fit(new[]
        {
            new CalibrationSample(0, 40),
            new CalibrationSample(0, 41)
        });

        Assert.False(r.IsValid);
    }

    [Fact]
    public void Fit_DecreasingWeights_IsInvalid()
    {
        // Physically impossible (more items, less weight) -> non-positive per-item.
        var r = Calibration.Fit(new[]
        {
            new CalibrationSample(10, 50),
            new CalibrationSample(20, 40)
        });

        Assert.False(r.IsValid);
    }

    [Fact]
    public void Fit_SameNonZeroQuantity_AveragesWeights()
    {
        var r = Calibration.Fit(new[]
        {
            new CalibrationSample(10, 232),
            new CalibrationSample(10, 234)
        });

        Assert.True(r.IsValid);
        Assert.Equal(23.3, r.PerItemWeightGrams, 6); // (233 avg) / 10
        Assert.Equal(0, r.TareGrams, 6);
    }
}
