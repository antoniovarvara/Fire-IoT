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
            else if (value == 0)
            {
                return SensStatus.FAIL;
            }
            else {
                return SensStatus.OUTOFRANGE;
            }
        }
        public override double convert(double value) {
            return value * 100; //0.01V per grado;
        }
        public TemperatureSensor(Gadgeteer.SocketInterfaces.AnalogInput input, Mqtt mqtt):base(input, mqtt){
            this.name = this.GetType().FullName;
        }
    }
}
