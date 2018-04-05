using System;
using Microsoft.SPOT;
using GT = Gadgeteer;

using System.Threading;

namespace NETMFBook.Sensors
{
    public abstract class Sensor
    {
        private GT.SocketInterfaces.AnalogInput input;
        private int repetition;
        private double value, lastValue = -1;
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
            if (lastValue == -1 || lastValue != value || repetition > 14)
            {
                repetition = 0;
                new Thread(() =>
                {
                    mqtt.Publish(this.name, Measure.Json(new Measure(this.name, checkValidity(this.read()), this.value)));
                });
                lastValue = this.value;
            }
            else
            {
                repetition++;
            }
        }
    }
}
