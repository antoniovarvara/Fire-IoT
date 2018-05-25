using System;
using System.Threading;
using Microsoft.SPOT;


namespace NETMFBook.Sensors
{

    public class ToneBuzzer : IDisposable
    {
        // Let's define the frequencies in the 5th Octave to get the most precision
        public enum Note
        {
            A = 880, Asharp = 932,
            B = 988,
            C = 1047, Csharp = 1109,
            D = 1175, Dsharp = 1244,
            E = 1319,
            F = 1397, Fsharp = 1480,
            G = 1568, Gsharp = 1661,
            Rest = 0
        }

        public enum Duration { Whole = 1000, Half = 500, Quarter = 250, Eigth = 125, Sixteenth = 67 }

        public class Tone
        {
            public Note note { get; set; }
            public Duration duration { get; set; }
            public int octave { get; set; }
            public int frequency { get; set; }

            public Tone(Note n, Duration t)
            {
                note = n;
                duration = t;
                octave = 0;
                frequency = (int)n >> 1;
            }

            public Tone(Note n, int o, Duration t)
            {
                note = n;
                duration = t;
                octave = o;
                frequency = (int)n >> (5 - o);
            }
        }

        Gadgeteer.SocketInterfaces.PwmOutput myPWM;

        public ToneBuzzer(Gadgeteer.SocketInterfaces.PwmOutput myPwm)
        {
            this.myPWM = myPwm;
            this.myPWM.Set(0, 0);
        }

        public void Dispose()
        {
        }

        public int Frequency(Note n, int octave)
        {
            return ((int)n >> (5 - octave));
        }

        public int Frequency(Note n)
        {
            return ((int)n >> 1); // Assume octave 4
        }

        public void Play(int freq_Hz, int duration_mSec)
        {
            myPWM.Set(freq_Hz, 50);
            Thread.Sleep(duration_mSec);
            myPWM.Set(0, 0);
        }

        public void Play(Note n, Duration t)
        {
            myPWM.Set(Frequency(n), 50);
            Thread.Sleep((int)t);
            myPWM.Set(0, 0);
        }

        public void Play(Note n, int octave, Duration t)
        {
            myPWM.Set(Frequency(n, octave), 50);
            Thread.Sleep((int)t);
            myPWM.Set(0, 0);
        }

        public void Play(Tone t)
        {
            Play(t.frequency, (int)t.duration);
        }

        public void Play(Tone[] tones)
        {
            foreach (Tone t in tones) Play(t);
        }
    }
}