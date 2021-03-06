using System;
using Microsoft.SPOT;

namespace NETMFBook.Sensors
{
    class TemperatureSensor:Sensor
    {
        public override SensStatus checkValidity(double value){
            if (value < 150 && value > 2) {
                return SensStatus.OK;
            }
            else if (value <= 2)
            {
                return SensStatus.FAIL;
            }
            else {
                return SensStatus.OUTOFRANGE;
            }
        }
        public override double convert(double value) {
            double valore = value * 100; //0.01V per grado;
            return System.Math.Truncate((valore)*10)/10; //trocamento a 1 cifra decimale 
        }
        public TemperatureSensor(Gadgeteer.SocketInterfaces.AnalogInput input, String name=null):base(input)
        {
            this.name = name==null?this.GetType().Name:name;
        }
        public override bool changedSignificantly()
        {
            return System.Math.Abs(this.value - this.oldValue) > 0.5;
        }
    }
}
