using System;
using Microsoft.SPOT;

namespace NETMFBook.Sensors
{
    class FlameSensor:Sensor
    {
        public override SensStatus checkValidity(double value){
            if(value <= 2 || value>1022)
            {
                return SensStatus.FAIL;
            }
            return SensStatus.OK;
        }
        public override double convert(double value) {
            return 1024- System.Math.Round(value / 3.3 * 1024); //map to 10 bit adc and not it (logic)
        }
        public FlameSensor(Gadgeteer.SocketInterfaces.AnalogInput input, String name = null)
            : base(input)
        {
            this.name = name==null?this.GetType().Name:name;
        }
        public override bool changedSignificantly()
        {
            return System.Math.Abs(this.value - this.oldValue) > 50;
        }
    }
}
