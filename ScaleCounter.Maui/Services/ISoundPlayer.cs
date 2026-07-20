using ScaleCounter.Core;

namespace ScaleCounter.Maui.Services;

/// <summary>Plays a short audible signal for a count-state change.</summary>
public interface ISoundPlayer
{
	void Play(CountSignal signal);
}
