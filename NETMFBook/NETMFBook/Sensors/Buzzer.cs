using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using System.Text;
namespace NETMFBook.Sensors
{
    class Buzzer
    {
        private Mqtt mqtt;
        private GT.SocketInterfaces.DigitalOutput output;
        private bool state;
        private string name;
        public Buzzer(GT.SocketInterfaces.DigitalOutput output, Mqtt mqtt,String name=null) {
            this.mqtt = mqtt;
            this.output = output;
            this.name = name == null ? this.GetType().Name : name;
        }
        public void subscribe()
        {
            mqtt.Subscribe(this.name);
            mqtt.PublishEvent += mqtt_PublishEvent;
        }

        void mqtt_PublishEvent(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            if (e.Topic.Equals(this.name)) {
                AlarmMessage m=(AlarmMessage) Json.NETMF.JsonSerializer.DeserializeString(new String(Encoding.UTF8.GetChars(e.Message)));
                this.output.Write(m.Alarm);
                state = m.Alarm;
            }
        }
    }
}
