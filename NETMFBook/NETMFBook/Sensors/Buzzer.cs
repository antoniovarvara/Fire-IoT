using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using System.Text;
using System.Threading;

namespace NETMFBook.Sensors
{
    static class Buzzer
    {
        static private GT.SocketInterfaces.DigitalOutput output;
        static private ToneBuzzer tb;
        static private bool state;
        static private ToneBuzzer.Tone[] mytone ={
                    new ToneBuzzer.Tone(ToneBuzzer.Note.C,3,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.G,3,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.E,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.D,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.C,4,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.G,3,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.E,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.D,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.C,4,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.G,3,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.E,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,3,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.D,3,ToneBuzzer.Duration.Whole),
        };
        static private String[] notes = {"G4","G4", "G4", "D#4/Eb4", "A#4/Bb4", "G4", "D#4/Eb4","A#4/Bb4", "G4", "D5", "D5", "D5", "D#5/Eb5", "A#4/Bb4", "F#4/Gb4", "D#4/Eb4","A#4/Bb4", "G4", "G5","G4","G4","G5","F#5/Gb5", "F5","E5","D#5/Eb5","E5", "rest", "G4", "rest","C#5/Db5","C5","B4","A#4/Bb4","A4","A#4/Bb4", "rest", "D#4/Eb4", "rest", "F#4/Gb4", "D#4/Eb4","A#4/Bb4", "G4" ,"D#4/Eb4","A#4/Bb4", "G4"};
        static private int[] imperialMarch_tones_lazy = { 1275, 1275, 1275, 1607, 1072, 1275, 1607, 1072, 1275, 851, 851, 851, 803, 1072, 1351, 1607, 1072, 1275, 637, 1275, 1275, 637, 675, 715, 758, 803, 758, 0, 1275, 0, 901, 955, 1012, 1072, 1136, 1072, 0, 1607, 0, 1351, 1607, 1072, 1275, 1607, 1072, 1275 };
        
        static private String[] noteNames = { "D#4/Eb4", "E4", "F4", "F#4/Gb4", "G4", "G#4/Ab4", "A4", "A#4/Bb4", "B4", "C5", "C#5/Db5", "D5", "D#5/Eb5", "E5", "F5", "F#5/Gb5", "G5", "G#5/Ab5", "A5", "A#5/Bb5", "B5", "C6", "C#6/Db6", "D6", "D#6/Eb6", "E6", "F6", "F#6/Gb6", "G6" };
        //static private int[] imperialMarch_tones_lazy = { 1607, 1516, 1431, 1351, 1275, 1203, 1136, 1072, 1012, 955, 901, 851, 803, 758, 715, 675, 637, 601, 568, 536, 506, 477, 450, 425, 401, 379, 357, 337, 318 };
        static private int[] imperialMarch_beats_lazy = { 8, 8, 8, 6, 2, 8, 6, 2, 16, 8, 8, 8, 6, 2, 8, 6, 2, 16, 8, 6, 2, 8, 6, 2, 2, 2, 2, 6, 2, 2, 8, 6, 2, 2, 2, 2, 6, 2, 2, 9, 6, 2, 8, 6, 2, 16 };
  
        static public void init(GT.SocketInterfaces.PwmOutput o) {
            tb = new ToneBuzzer(); ;
        }
  
        static public void setState(bool s) {
            Debug.Print(imperialMarch_tones_lazy.Length + " / " + notes.Length);
            if (s == true) {
                if (!tb.sound)
                {
                    tb.sound = true;
                    //new Thread(() => tb.Play(mytone)).Start();
                    new Thread(() => tb.Play(imperialMarch_tones_lazy, imperialMarch_beats_lazy)).Start();
                }
            }
            else
            {
                tb.sound = false;
            }
            //output.Write(s);
            state= s;
        }
        
    }
}
