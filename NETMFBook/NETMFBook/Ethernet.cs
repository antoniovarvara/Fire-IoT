using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using System;
using System.Text;
using System.Threading;

namespace NETMFBook
{
    class Ethernet
    {
        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        private EthernetJ11D ethernetJ11D;
        private Mqtt mqtt=null;
        public Mqtt MQTT { get{
            while (mqtt == null) {
                waitHandle.WaitOne();
            }
            lock (this)
            {
                return mqtt;
            }
        }}

        public Ethernet(EthernetJ11D ethernetJ11D) {
            this.ethernetJ11D = ethernetJ11D;
            try
            {

                //ethernetJ11D.NetworkSettings.PhysicalAddress=new byte[]{0x00,0x21,0x03,0x80,0x8B,0x6B};
                if (!ethernetJ11D.NetworkInterface.Opened)
                {
                    ethernetJ11D.NetworkInterface.Open();
                }
                PrintNetworkState();
                ethernetJ11D.UseThisNetworkInterface();
                ethernetJ11D.NetworkSettings.EnableDhcp();
                ethernetJ11D.NetworkSettings.EnableDynamicDns();
                //ethernetJ11D.UseDHCP();
                //ethernetJ11D.UseStaticIP("192.168.3.99", "255.255.255.0", "192.168.3.235");
                while (ethernetJ11D.NetworkInterface.IPAddress.Equals("0.0.0.0"))
                {
                    Debug.Print("Waiting for Network!");
                    Thread.Sleep(1000);
                }
                Debug.Print("Connected IP:" + ethernetJ11D.NetworkInterface.IPAddress);
                PrintNetworkState();
                lock (this)
                {
                    mqtt = new Mqtt();
                    waitHandle.Set();
                }
                Microsoft.SPOT.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += (s, e) => ethernetJ11D_CableChange(s, e);
                ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
                ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            }
            catch (Exception e) {
                Debug.Print(e.StackTrace);
            }
        }

        private void ethernetJ11D_CableChange(object s, Microsoft.SPOT.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                Debug.Print("Network cable connected.");
                StatusLed.led.SetLed(1, true);
                PrintNetworkState();
                //mqtt.connectInfinite();
            }
            else
            {
                Debug.Print("Network cable disconnected.");
                StatusLed.led.SetLed(1, false);
                PrintNetworkState();
                /*new Thread(() =>
                {
                    while (ethernetJ11D.NetworkInterface.IPAddress.Equals("0.0.0.0"))
                    {
                        Debug.Print("Waiting for Network!");
                        Thread.Sleep(2000);
                        //ethernetJ11D.UseDHCP();
                    }
                    PrintNetworkState();
                }).Start();*/
            }
        }

        void ethernetJ11D_NetworkUp(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        {
            StatusLed.led.SetLed(1, true);
            StatusLed.led.SetLed(2, true);
            Debug.Print("Network UP!");
            PrintNetworkState();
            mqtt.connectInfinite();
        }

        void ethernetJ11D_NetworkDown(Gadgeteer.Modules.Module.NetworkModule sender, Gadgeteer.Modules.Module.NetworkModule.NetworkState state)
        {
            StatusLed.led.SetLed(1, false);
            StatusLed.led.SetLed(2, false);
            Debug.Print("Network DOWN!");
            PrintNetworkState();
            /*new Thread(() =>
            {
                while (ethernetJ11D.NetworkInterface.IPAddress.Equals("0.0.0.0"))
                {
                    Debug.Print("Waiting for Network!");
                    Thread.Sleep(1000);
                    ethernetJ11D.UseDHCP();
                }
            }).Start();*/

        }
        private void PrintNetworkState()
        {
	        StringBuilder builder = new StringBuilder();
	        builder.Append("Up=");
	        builder.Append(ethernetJ11D.IsNetworkUp);
	        builder.Append("; ");
	        builder.Append("Connected=");
	        builder.Append(ethernetJ11D.IsNetworkConnected);
	        builder.Append("; ");
	        builder.Append("IP=");
	        builder.Append(ethernetJ11D.NetworkInterface.IPAddress);
	        builder.Append("; ");
	        builder.Append("Mask=");
	        builder.Append(ethernetJ11D.NetworkInterface.SubnetMask);
	        builder.Append("; ");
	        builder.Append("GW=");
	        builder.Append(ethernetJ11D.NetworkInterface.GatewayAddress);
	        Debug.Print(builder.ToString());
            DisplayLCD.addNetInfo(
                ethernetJ11D.IsNetworkConnected,
                ethernetJ11D.NetworkInterface.IPAddress,
                ethernetJ11D.NetworkInterface.SubnetMask,
                ethernetJ11D.NetworkInterface.GatewayAddress);
        }
    }

}
