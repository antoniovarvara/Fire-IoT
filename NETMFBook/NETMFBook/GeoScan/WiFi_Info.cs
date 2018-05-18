using System;
using Microsoft.SPOT;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace NETMFBook.GeoScan
{
    class WiFi_Info
    {
        public String ssid { get; set; }
        public int channel { get; set; }
        public int rssi { get; set; }
        public int authMode { get; set; }
        public String bssid { get; set; }

        public WiFi_Info(GHI.Networking.WiFiRS9110.NetworkParameters info)
        {
             ssid = info.Ssid;
             channel = info.Channel;
             rssi = -info.Rssi;
             authMode = (int) info.SecurityMode;
             bssid = GetMACAddress(info.PhysicalAddress);
        }

         // borrowed from GHI's documentation
        string GetMACAddress(byte[] PhysicalAddress)
        {
            return ByteToHex(PhysicalAddress[0]) + ":"
                                + ByteToHex(PhysicalAddress[1]) + ":"
                                + ByteToHex(PhysicalAddress[2]) + ":"
                                + ByteToHex(PhysicalAddress[3]) + ":"
                                + ByteToHex(PhysicalAddress[4]) + ":"
                                + ByteToHex(PhysicalAddress[5]);
        }

        string ByteToHex(byte number)
        {
            string hex = "0123456789abcdef";
            return new string(new char[] { hex[(number & 0xF0) >> 4], hex[number & 0x0F] });
        }
    }
}
