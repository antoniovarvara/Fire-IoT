using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using System.Text;
namespace NETMFBook.Sensors
{
    static class Buzzer
    {
        static private GT.SocketInterfaces.DigitalOutput output;
        static private ToneBuzzer tb;
        static private bool state;
        static private ToneBuzzer.Tone[] mytone ={
                    new ToneBuzzer.Tone(ToneBuzzer.Note.C,5,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.G,5,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.E,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.D,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.C,6,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.G,5,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.E,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.D,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.C,6,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.G,5,ToneBuzzer.Duration.Whole),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.E,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.F,5,ToneBuzzer.Duration.Quarter),
                    new ToneBuzzer.Tone(ToneBuzzer.Note.D,5,ToneBuzzer.Duration.Whole),
        };
        static public void init(GT.SocketInterfaces.PwmOutput o) {
            tb = new ToneBuzzer(o); ;
        }
  
        static public void setState(bool s) {
            if (s == true) {
                tb.Play(mytone);
            }
            //output.Write(s);
            state= s;
        }
        
    }
}
