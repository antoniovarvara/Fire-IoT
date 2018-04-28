using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using System.Text;
namespace NETMFBook.Sensors
{
    static class Buzzer
    {
        static private GT.SocketInterfaces.DigitalOutput output;
        static private bool state;
        static public void init(GT.SocketInterfaces.DigitalOutput o) {
            output = o;
        }
        static public void setState(bool s) {
            output.Write(s);
            state= s;
        }
        
    }
}
