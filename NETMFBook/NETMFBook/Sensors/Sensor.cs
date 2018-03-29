using System;
using Microsoft.SPOT;
using GT = Gadgeteer;

namespace NETMFBook.Sensors
{
    public abstract class Sensor
    {
        private GT.SocketInterfaces.AnalogInput input;
        private double value;
        private Guid id=new Guid();
        private Mqtt mqtt;
        protected string name;
        public Sensor(GT.SocketInterfaces.AnalogInput input, Mqtt mqtt) {
            this.mqtt = mqtt;
            this.input = input;
        }
        public double read()
        {
            return this.value = this.convert(input.ReadVoltage());   
        }
        public abstract double convert(double value);
        public abstract SensStatus checkValidity(double value);
        public void publish()
        {
            mqtt.Publish(this.name,Measure.Json(new Measure(this.name,checkValidity(this.read()),this.value)));
        }
    }
}
