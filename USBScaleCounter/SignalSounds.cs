using ScaleCounter.Core;
using System;
using System.Threading.Tasks;

namespace USBScaleCounter
{
    /// <summary>Plays distinct beeps for count signals (off the UI thread; Console.Beep is blocking).</summary>
    internal static class SignalSounds
    {
        public static void Play(CountSignal signal)
        {
            Task.Run(() =>
            {
                try
                {
                    switch (signal)
                    {
                        case CountSignal.Success:
                            Console.Beep(988, 120);
                            Console.Beep(1319, 180);
                            break;
                        case CountSignal.Error:
                            Console.Beep(220, 500);
                            break;
                        case CountSignal.Warning:
                            Console.Beep(660, 200);
                            break;
                    }
                }
                catch
                {
                    // No audio device / not supported.
                }
            });
        }
    }
}
