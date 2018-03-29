using System;
using Microsoft.SPOT;
using Json.NETMF;

namespace NETMFBook.Sensors
{
    class Measure
    {
        private double value;
        private SensStatus status;
        private string name;
        private DateTime timestamp = DateTime.Now;
        public Measure(String name, SensStatus status, double value) {
            this.name = name;
            this.status = status;
            this.value = value;
        }
        public Measure() {}
        public static string Json(Measure measure)
        {
            JsonSerializer serializer = new JsonSerializer(DateTimeFormat.Default);
            return serializer.Serialize(measure);
        }
    }
    public enum SensStatus
	{
	    OK,
        FAIL,
        OUTOFRANGE,
	}
}
