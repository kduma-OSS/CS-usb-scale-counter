using UnitsNet;
using Xunit;

namespace ScaleCounter.Core.Tests;

public class ItemCounterTests
{
    // 23.2 g/item, no tare, target 25 (the seeded default).
    private static ItemCounter Default() =>
        new() { PerItemWeightGrams = 23.2, TareGrams = 0, TargetQuantity = 25 };

    [Fact]
    public void Count_ExactQuantity()
    {
        var r = Default().Count(Mass.FromGrams(580)); // 580 / 23.2 = 25

        Assert.Equal(25, r.Count);
        Assert.Equal(CountState.Exact, r.State);
        Assert.Equal("There is 25 items on scale!", r.Message);
        Assert.Equal("OK", r.Diff);
    }

    [Fact]
    public void Count_Empty()
    {
        var r = Default().Count(Mass.FromGrams(0));

        Assert.Equal(0, r.Count);
        Assert.Equal(CountState.Empty, r.State);
        Assert.Equal("Place something on scale!", r.Message);
    }

    [Fact]
    public void Count_NotEnough()
    {
        var r = Default().Count(Mass.FromGrams(232)); // 10 items

        Assert.Equal(10, r.Count);
        Assert.Equal(CountState.NotEnough, r.State);
        Assert.Equal("+ 15", r.Diff);
    }

    [Fact]
    public void Count_TooMany()
    {
        var r = Default().Count(Mass.FromGrams(696)); // 30 items

        Assert.Equal(30, r.Count);
        Assert.Equal(CountState.TooMany, r.State);
        Assert.Equal("- 5", r.Diff);
    }

    [Fact]
    public void Count_RoundsToNearest()
    {
        // Boundary between 24 and 25 is 24.5 * 23.2 = 568.4 g.
        Assert.Equal(24, Default().Count(Mass.FromGrams(568)).Count);
        Assert.Equal(25, Default().Count(Mass.FromGrams(569)).Count);
    }

    [Fact]
    public void Count_LightItemsAreNotInflated()
    {
        // Regression for the old fixed +5 g bias: with 4 g items an empty pan used to read 1.
        var counter = new ItemCounter { PerItemWeightGrams = 4, TareGrams = 0, TargetQuantity = 25 };

        Assert.Equal(0, counter.Count(Mass.FromGrams(0)).Count);
        Assert.Equal(1, counter.Count(Mass.FromGrams(4)).Count);
        Assert.Equal(0, counter.Count(Mass.FromGrams(1)).Count); // 0.25 -> 0
    }

    [Fact]
    public void Count_SubtractsTare()
    {
        var counter = new ItemCounter { PerItemWeightGrams = 23.2, TareGrams = 50, TargetQuantity = 25 };

        Assert.Equal(0, counter.Count(Mass.FromGrams(50)).Count);   // just the container
        Assert.Equal(25, counter.Count(Mass.FromGrams(630)).Count); // 50 + 25*23.2
        Assert.Equal(CountState.Exact, counter.Count(Mass.FromGrams(630)).State);
    }

    [Fact]
    public void Count_UncalibratedWhenNoPerItemWeight()
    {
        var counter = new ItemCounter { PerItemWeightGrams = 0, TargetQuantity = 25 };
        var r = counter.Count(Mass.FromGrams(580));

        Assert.Equal(CountState.Uncalibrated, r.State);
    }

    [Fact]
    public void Apply_CopiesPresetCalibration()
    {
        var preset = new WeighedItemPreset
        {
            PerItemWeightGrams = 12.5,
            TareGrams = 40,
            TargetQuantity = 100
        };
        var counter = new ItemCounter();
        counter.Apply(preset);

        Assert.Equal(12.5, counter.PerItemWeightGrams);
        Assert.Equal(40, counter.TareGrams);
        Assert.Equal(100, counter.TargetQuantity);
    }
}
