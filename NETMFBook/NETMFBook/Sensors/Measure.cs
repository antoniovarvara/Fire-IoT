using System;
using Microsoft.SPOT;
using Json.NETMF;

namespace NETMFBook.Sensors
{
    class Measure
    {
        public double Value { get; set; }
        public SensStatus Status { get; set; }
        public String Id { get; set; }
        public String Name { get; set; }
        public DateTime Timestamp { get; set; }
        public Measure(String id, String name, SensStatus status, double value) {
            this.Id = id;
            this.Name = name;
            this.Status = status;
            this.Value = value;
            this.Timestamp = DateTime.Now;
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
	    OK=0,
        FAIL=1,
        OUTOFRANGE=2,
	}
}
