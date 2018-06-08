using System;
using Microsoft.SPOT;
using Json.NETMF;

namespace NETMFBook
{
    class Configuration
    {
        public int version { get; set; }
        public String id { get; set; }
        public String name { get; set; }
        public String group { get; set; }
        public String type { get; set; }
        public DescrSensor[] sensors { get; set; }
        public String description { get; set; }
        public String location { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool  @internal { get; set; }
        public Configuration()
        {

        }
        public Configuration(double lat,double lng) { 
            version = 2;
            id = "FEZ_24";
            name = "Iot Fire monitoring";
            group = "FEZ 24";
            type = "fire";
            sensors = new DescrSensor[4];
            sensors[0] = new DescrSensor(0, "flame 1", "flame");
            sensors[1] = new DescrSensor(1, "smoke 1", "smoke");
            sensors[2] = new DescrSensor(2, "co2 1", "co2");
            sensors[3] = new DescrSensor(3, "temperature 1", "temperature");
            description = "Our platform monitor fire";
            location = "Torino";
            latitude = lat;
            longitude = lng;
            @internal = true;
        }

        public static string Json(Configuration msg)
        {
            JsonSerializer serializer = new JsonSerializer(DateTimeFormat.ISO8601);
            return serializer.Serialize(msg);
        }
    }
    class DescrSensor {
       public int id{get; set;}
       public string name { get; set; }
       public string type { get; set; }
       public DescrSensor() {}
       public DescrSensor(int id, string name, string type) {
           this.id = id;
           this.name = name;
           this.type = type;
       }
    }
}
