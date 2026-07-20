namespace ScaleCounter.Core;

/// <summary>An audible signal to play when the count state changes.</summary>
public enum CountSignal
{
    None,
    Success, // reached the target
    Error,   // went over the target
    Warning  // dropped from exact/too-many back below the target
}

/// <summary>Maps count-state transitions to an audible signal (pure, platform-independent).</summary>
public static class CountSignals
{
    /// <summary>
    /// Decides which signal (if any) a transition warrants:
    /// entering Exact → Success, entering TooMany → Error, and dropping from
    /// Exact/TooMany down to NotEnough → Warning. Everything else is silent.
    /// </summary>
    public static CountSignal ForTransition(CountState previous, CountState next)
    {
        if (previous == next)
            return CountSignal.None;

        switch (next)
        {
            case CountState.Exact:
                return CountSignal.Success;
            case CountState.TooMany:
                return CountSignal.Error;
            case CountState.NotEnough:
                return previous == CountState.Exact || previous == CountState.TooMany
                    ? CountSignal.Warning
                    : CountSignal.None;
            default:
                return CountSignal.None;
        }
    }
}
