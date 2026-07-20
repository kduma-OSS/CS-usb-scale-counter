using UnitsNet;
using Xunit;

namespace ScaleCounter.Core.Tests;

public class ScaleDebounceTests
{
    /// <summary>Minimal concrete scale for exercising the debounce + event logic.</summary>
    private sealed class TestScale : ScaleBase
    {
        public override void Start() { }
        public override void Stop() { }
        public void Feed(byte[] buffer) => OnBufferRead(buffer);
        public void FeedWeight(Mass weight) => SetWeight(weight);
        public void FeedStatus(StatusEnum status) => SetStatus(status);
    }

    private static byte[] GramsReport(byte status, int raw) =>
        new byte[] { 3, status, 2, 0, (byte)(raw & 0xFF), (byte)((raw >> 8) & 0xFF) };

    [Fact]
    public void WeightChanged_SuppressedWithinTolerance()
    {
        var scale = new TestScale();
        int raised = 0;
        scale.WeightChanged += (_, _) => raised++;

        scale.FeedWeight(Mass.FromGrams(100)); // 0 -> 100 g: fires
        scale.FeedWeight(Mass.FromGrams(101)); // +1 g < 0.05 oz (~1.42 g): suppressed

        Assert.Equal(1, raised);
    }

    [Fact]
    public void WeightChanged_FiresBeyondTolerance()
    {
        var scale = new TestScale();
        int raised = 0;
        scale.WeightChanged += (_, _) => raised++;

        scale.FeedWeight(Mass.FromGrams(100));
        scale.FeedWeight(Mass.FromGrams(110)); // +10 g > tolerance: fires

        Assert.Equal(2, raised);
    }

    [Fact]
    public void WeightChanged_InitialZeroDoesNotFire()
    {
        var scale = new TestScale();
        int raised = 0;
        scale.WeightChanged += (_, _) => raised++;

        scale.FeedWeight(Mass.FromGrams(0)); // equal to initial 0 within tolerance

        Assert.Equal(0, raised);
    }

    [Fact]
    public void StatusChanged_FiresOnChangeOnly()
    {
        var scale = new TestScale();
        int raised = 0;
        StatusEnum? last = null;
        scale.StatusChanged += (_, s) => { raised++; last = s; };

        scale.FeedStatus(StatusEnum.Stable);   // Unknown -> Stable: fires
        scale.FeedStatus(StatusEnum.Stable);   // no change: suppressed
        scale.FeedStatus(StatusEnum.InMotion); // fires

        Assert.Equal(2, raised);
        Assert.Equal(StatusEnum.InMotion, last);
    }

    [Fact]
    public void OnBufferRead_ParsesAndRaises()
    {
        var scale = new TestScale();
        Mass? weight = null;
        StatusEnum? status = null;
        scale.WeightChanged += (_, w) => weight = w;
        scale.StatusChanged += (_, s) => status = s;

        scale.Feed(GramsReport(4, 580)); // Stable, 580 g

        Assert.Equal(StatusEnum.Stable, status);
        Assert.NotNull(weight);
        Assert.Equal(580d, weight!.Value.Grams, 3);
    }

    [Fact]
    public void OnBufferRead_IgnoresNonDataReport()
    {
        var scale = new TestScale();
        int raised = 0;
        scale.WeightChanged += (_, _) => raised++;
        scale.StatusChanged += (_, _) => raised++;

        scale.Feed(new byte[] { 0, 4, 2, 0, 0x44, 0x02 }); // report id != 3

        Assert.Equal(0, raised);
    }
}
