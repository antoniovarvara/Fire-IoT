using System;
using Microsoft.SPOT;

namespace NETMFBook.Sensors
{
    class AlarmMessage
    {
        private object p;

        public bool Alarm { get; set; }
        public AlarmMessage() { }
        public AlarmMessage(bool alarm) {
            this.Alarm = alarm;
        }
        public AlarmMessage(String json) {
            Object o = Json.NETMF.JsonSerializer.DeserializeString(json);
            this.Alarm = (Boolean)((System.Collections.Hashtable)o)["Alarm"];
        }
    }
}
