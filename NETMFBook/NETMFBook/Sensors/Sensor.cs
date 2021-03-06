using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using NETMFBook.Database;
using System.Threading;

namespace NETMFBook.Sensors
{
    public abstract class Sensor
    {
        private GT.SocketInterfaces.AnalogInput input;
        protected double value, oldValue = -1;
        public string name { get; protected set; }
        public Sensor(GT.SocketInterfaces.AnalogInput input) {
            this.input = input;
        }
        public double read()
        {
            this.oldValue = this.value;
            this.value = this.convert(input.ReadVoltage());
            DisplayLCD.addMeasure(this, this.value);
            Debug.Print("Published name: " + this.name + " value: " + this.value);
            return this.value;
        }
        public abstract double convert(double value);
        public abstract SensStatus checkValidity(double value);
        public abstract bool changedSignificantly();
       /* 
        public void publish()
        {
            double reading = 0;
            if (lastValue == -1 || lastValue != value || repetition > 1)
            {
                repetition = 0;
                new Thread(() =>
                {
                    reading = this.read();
                    DisplayLCD.addMeasure(this, reading);
                    //mqtt.Publish(this.name, Measure.Json(new Measure("fez24", this.name, checkValidity(reading), reading)));
                    Debug.Print("Published name: "+ this.name+ " status: " + checkValidity(reading) + " value: " + reading);
                }).Start();
                lastValue = this.value;
            }
            else
            {
                //Debug.Print("Not published "+ this.name + ", last value: "+lastValue+" read: "+reading+" repetition: "+repetition);
                repetition++;
            }
        }*/
    }
}
