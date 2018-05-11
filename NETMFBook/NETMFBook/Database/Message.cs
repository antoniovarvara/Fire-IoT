using System;
using Microsoft.SPOT;
using Json.NETMF;
using NETMFBook.Sensors;
using System.Collections;

namespace NETMFBook.Database
{
    class Message
    {
        public String device_id { get; set; }
        public ArrayList measurements{get;set;}
        public Message(String id)
        {
            this.device_id = id;
            this.measurements = new ArrayList();
        }

        public void addMeasure(Measure measure)
        {
            measurements.Add(measure);
        }

        public static string Json(Message msg)
        {
            JsonSerializer serializer = new JsonSerializer(DateTimeFormat.ISO8601);
            return serializer.Serialize(msg);
        }

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
            Message other = (Message) obj;
            // TODO: write your implementation of Equals() here
            return this.device_id.Equals(other.device_id) && this.measurements.Equals(other.measurements);
        }
    
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return this.device_id.GetHashCode() & this.measurements.GetHashCode();
        }
    }
}
