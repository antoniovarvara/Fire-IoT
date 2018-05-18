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
        public String[] sensors { get; set; }
        public String description { get; set; }
        public String location { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool Internal { get; set; }

        public Configuration() { 
            version = 1;
            id = "FEZ_24";
            name = "";
            group = "FEZ 24";
            type = "fire";
            sensors = new String[4];
            sensors[0] = "heating";
            sensors[1] = "smoke";
            sensors[2] = "other";
            sensors[3] = "temperature";
            description = "";
            location = "Torino";
            latitude =  45.058120;
            longitude = 7.691776;
            Internal = true;
        }

        public static string Json(Configuration msg)
        {
            JsonSerializer serializer = new JsonSerializer(DateTimeFormat.ISO8601);
            return serializer.Serialize(msg);
        }
    }
}
