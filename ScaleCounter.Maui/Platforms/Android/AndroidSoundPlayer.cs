using Android.Media;
using ScaleCounter.Core;
using ScaleCounter.Maui.Services;

namespace ScaleCounter.Maui;

/// <summary>
/// Plays distinct beeps for count signals by synthesizing short sine tones and playing them
/// via AudioTrack. Success/warning use the desktop's Console.Beep frequencies; the error tone
/// is shifted up into a lower-but-audible band (a loud descending two-tone) because phone
/// speakers barely reproduce the desktop's 220 Hz buzz, which came out very quiet.
/// </summary>
public sealed class AndroidSoundPlayer : ISoundPlayer
{
	private const int SampleRate = 44100;

	public void Play(CountSignal signal)
	{
		Task.Run(() =>
		{
			try
			{
				switch (signal)
				{
					case CountSignal.Success:
						PlayTone(988, 120);
						PlayTone(1319, 180);
						break;
					case CountSignal.Error:
						// Descending, louder, and kept above ~330 Hz so the small
						// speaker actually renders it (220 Hz was inaudible).
						PlayTone(494, 260, 0.9);
						PlayTone(330, 380, 0.9);
						break;
					case CountSignal.Warning:
						PlayTone(660, 200);
						break;
				}
			}
			catch
			{
				// Audio unavailable — signalling is best-effort.
			}
		});
	}

	private static void PlayTone(int frequencyHz, int durationMs, double amplitude = 0.6)
	{
		int sampleCount = durationMs * SampleRate / 1000;
		var samples = new short[sampleCount];
		int fade = SampleRate / 100; // ~10 ms fade in/out to avoid clicks
		double step = 2.0 * Math.PI * frequencyHz / SampleRate;

		for (int i = 0; i < sampleCount; i++)
		{
			double envelope = 1.0;
			if (i < fade)
				envelope = (double)i / fade;
			else if (i > sampleCount - fade)
				envelope = (double)(sampleCount - i) / fade;

			samples[i] = (short)(Math.Sin(step * i) * short.MaxValue * amplitude * envelope);
		}

		var buffer = new byte[sampleCount * 2];
		Buffer.BlockCopy(samples, 0, buffer, 0, buffer.Length);

#pragma warning disable CA1422 // the simple ctor is fine for a short one-shot tone
		var track = new AudioTrack(
			Android.Media.Stream.Music,
			SampleRate,
			ChannelOut.Mono,
			Android.Media.Encoding.Pcm16bit,
			buffer.Length,
			AudioTrackMode.Static);
#pragma warning restore CA1422

		track.Write(buffer, 0, buffer.Length);
		track.Play();
		System.Threading.Thread.Sleep(durationMs + 40);
		track.Stop();
		track.Release();
	}
}
