using Xunit;

namespace ScaleCounter.Core.Tests;

public class CountSignalsTests
{
    [Theory]
    [InlineData(CountState.NotEnough, CountState.Exact, CountSignal.Success)]
    [InlineData(CountState.Empty, CountState.Exact, CountSignal.Success)]
    [InlineData(CountState.Exact, CountState.TooMany, CountSignal.Error)]
    [InlineData(CountState.NotEnough, CountState.TooMany, CountSignal.Error)]
    [InlineData(CountState.Exact, CountState.NotEnough, CountSignal.Warning)]
    [InlineData(CountState.TooMany, CountState.NotEnough, CountSignal.Warning)]
    public void ForTransition_SignalsExpected(CountState previous, CountState next, CountSignal expected)
    {
        Assert.Equal(expected, CountSignals.ForTransition(previous, next));
    }

    [Theory]
    [InlineData(CountState.Empty, CountState.NotEnough)]   // pouring up: no beep yet
    [InlineData(CountState.Exact, CountState.Exact)]        // no change
    [InlineData(CountState.NotEnough, CountState.Empty)]    // emptying out
    [InlineData(CountState.Uncalibrated, CountState.Empty)]
    public void ForTransition_Silent(CountState previous, CountState next)
    {
        Assert.Equal(CountSignal.None, CountSignals.ForTransition(previous, next));
    }
}
