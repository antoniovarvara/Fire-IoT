using System;
using Microsoft.SPOT;

namespace NETMFBook.Sensors
{
    class AlarmMessage
    {
        public bool Alarm { get; set; }
        public AlarmMessage() { }
        public AlarmMessage(bool alarm) {
            this.Alarm = alarm;
        }
    }
}
