using System;
using Microsoft.SPOT;
using Json.NETMF;

namespace NETMFBook.Sensors
{
    class Measure
    {
        public double value { get; set; }
        public string status { get { 
            switch (intstatus)
	        {
                case SensStatus.OK:{
                    return "OK";
                }
                case SensStatus.FAIL:{
                    return "FAIL";
                }
                case SensStatus.OUTOFRANGE:{
                    return "OUTOFRANGE";
                }
	        }
            return "";
        }}
        public SensStatus intstatus;
        public int sensor_id { get { return Int32.Parse(sensor_name); } }
        public string sensor_name;
        public String iso_timestamp { 
            get {
                return timestamp.ToString("yyyy-MM-ddTHH:mm:ss+02:00");
            }
        }
        private DateTime timestamp;
        public Measure(String name, SensStatus status, double value) {
            this.sensor_name = name;
            this.intstatus = status;
            this.value = value;
            this.timestamp = DateTime.Now;
        }
        public Measure() {}
        
        // override object.Equals
        public override bool Equals (object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType()) 
            {
                return false;
            }
            Measure other = (Measure) obj;
            return this.sensor_id.Equals(other.sensor_id) && this.value.Equals(other.value);
        }
    
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return this.sensor_id.GetHashCode() & this.value.GetHashCode();
        }
        
    }
    public enum SensStatus
	{
	    OK=0,
        FAIL=1,
        OUTOFRANGE=2,
	}
}
