using System;
using Microsoft.SPOT;
using System.Collections;
using Json.NETMF;
namespace NETMFBook.GeoScan
{
    class GeoMessage:ArrayList
    {

        public GeoMessage(GHI.Networking.WiFiRS9110.NetworkParameters[] scanResults)
        {
            foreach(GHI.Networking.WiFiRS9110.NetworkParameters info in scanResults){
                this.Add(new GeoScan.WiFi_Info(info));
            }
        }

        public void addNetwork(GHI.Networking.WiFiRS9110.NetworkParameters info)
        {
            this.Add(new GeoScan.WiFi_Info(info));
        }

        public static string Json(GeoMessage msg)
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Serialize(msg);
        }

    }
}
