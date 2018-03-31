using System;
using Microsoft.SPOT;

namespace NETMFBook.Sensors
{
    class FlameSensor:Sensor
    {
        public override SensStatus checkValidity(double value){
            if(value == 0 && value>1022)
            {
                return SensStatus.FAIL;
            }
            return SensStatus.OK;
        }
        public override double convert(double value) {
            return 1024- System.Math.Round(value / 3.3 * 1024); //map to 10 bit adc and not it (logic)
        }
        public FlameSensor(Gadgeteer.SocketInterfaces.AnalogInput input, Mqtt mqtt, String name = null)
            : base(input, mqtt)
        {
            this.name = name==null?this.GetType().Name:name;
        }
    }
}
