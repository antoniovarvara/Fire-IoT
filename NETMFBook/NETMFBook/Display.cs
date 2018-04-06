using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace NETMFBook
{
    static class DisplayLCD
    {
        public static DisplayTE35 lcd;
        private static Bitmap buffer = new Bitmap((int)SystemMetrics.ScreenWidth, (int)SystemMetrics.ScreenHeight);

        private static string[] log = new string[22];
        private static int counter = 1;

        private static int startX = 15;
        private static int startY = 10;
        private static string ip = null, netmask = null, gateway = null;
        private static int indexState = -1, indexIp = -1, indexNetmask = -1, indexGateway = -1, indexMqtt = -1;
        private static int indexFlame = -1, indexCO = -1, indexSmoke = -1;
        private static int flameValue = 0, coValue = 0, smokeValue = 0;
        private static bool stateNet = false, mqttConnected = false;
        private static void drawBuffer()
        {
            buffer.Flush();
            lcd.SimpleGraphics.DisplayImage(buffer, 0, 0);
        }

        private static void scrollLog()
        {
            for (int c = 4; c < counter - 1; c++)
            {
                log[c] = log[c + 1];
            }

            counter--;
        }

        public static void clear()
        {
            lcd.SimpleGraphics.Clear();
        }

        public static void Refresh()
        {
            lock (log)
            {
                buffer.Clear();

                // draw frame
                buffer.DrawRectangle(Color.White, 1, 5, 5, (int)(lcd.Width - 10), (int)(lcd.Height - 10), 5, 5, Color.White, 0, 0, Color.White, 0, 0, 125);
                log[0] = DateTime.Now.ToString();
                // to buffer
                for (int c = 0; c < counter; c++)
                {
                    if (c != 0 && c == indexFlame+1)
                    {
                        buffer.DrawRectangle(Color.White, 1, startX-2, startY + (c * 10) - 2, ((int)(flameValue * 300) / 1024), 14, 0, 0, GT.Color.Green, startX-2, startY + (c * 10) - 2, GT.Color.Red, startX + 300, startY + 14, 255);
                    }
                    if (c != 0 && c == indexCO + 1)
                    {
                        buffer.DrawRectangle(Color.White, 1, startX - 2, startY + (c * 10) - 2, ((int)(coValue * 300) / 1024), 14, 0, 0, GT.Color.Green, startX - 2, startY + (c * 10) - 2, GT.Color.Red, startX + 300, startY + 14, 255);
                    }
                    if (c != 0 && c == indexSmoke + 1)
                    {
                        buffer.DrawRectangle(Color.White, 1, startX - 2, startY + (c * 10) - 2, ((int)(smokeValue * 300) / 1024), 14, 0, 0, GT.Color.Green, startX - 2, startY + (c * 10) - 2, GT.Color.Red, startX + 300, startY + 14, 255);
                    }
                    buffer.DrawText(log[c], Resources.GetFont(Resources.FontResources.small), GT.Color.Cyan, startX, startY + (c * 10));
                   
                }

                drawBuffer();
            }
            
        }


        public static void Print(string msg)
        {
            lock (log)
            {
                log[counter++] = msg;
                if (counter > 21)
                    scrollLog();
            }
        }
        public static void addMqttInfo(Boolean connected)
        {
            lock (log)
            {
                if(indexMqtt == -1) indexMqtt = counter++;
                log[indexMqtt] = "MQTT connected: " + connected;
            }
        }

        private static void clearInfo()
        {
            
            /*smokeValue = 0;
            if (indexSmoke != -1)
            {
                log[indexSmoke+1] = "Smoke: 0";
                //indexSmoke = -1;
                //counter -= 2;
            } 
            coValue = 0;
            if (indexCO != -1)
            {
                log[indexCO+1] = "CO: 0";
                //indexCO = -1;
                //counter -= 2;
            }
            flameValue = 0;
            if (indexFlame != -1)
            {
                log[indexFlame+1] = "Flame: 0";
                //indexFlame = -1;
                //counter-=2;
            }*/
            mqttConnected = false;
            if (indexMqtt != -1)
            {
                log[indexMqtt] = "MQTT connected: false";
                //indexMqtt = -1;
                //counter--;
            }
            gateway = null;
            if (indexGateway != -1)
            {
                log[indexGateway] = "Gateway:  0.0.0.0"; 
                //indexGateway = -1; 
                //counter--;
            }
            netmask = null;
            if (indexNetmask != -1)
            {
                log[indexNetmask] = "Netmask:  0.0.0.0"; 
                //indexNetmask = -1; 
                //counter--;
            }
            ip = null;
            if (indexIp != -1) 
            { 
                log[indexIp] = "IP: 0.0.0.0"; 
                //indexIp = -1; 
                //counter--; 
            }
            //counter = 2;
            
        }

        public static void addNetInfo(bool stateE, String ipE, String netmaskE, String gatewayE)
        {
            lock (log)
            {
                stateNet = stateE;
                if (indexState == -1) indexState = counter++;
                log[indexState] = "Ethernet Cable Connected: " + stateE;
                if (!stateE) { clearInfo(); return; }
                ip = ipE;
                if(indexIp == -1) indexIp = counter++;
                log[indexIp] = "IP: " + ip;
                netmask = netmaskE;
                if (indexNetmask == -1) indexNetmask = counter++;
                log[indexNetmask] = "Netmask: " + netmask;
                gateway = gatewayE;
                if (indexGateway == -1) indexGateway = counter++;
                log[indexGateway] = "Gateway: " + gateway;
            }
        }

        public static void addMeasure(Sensors.Sensor s, double value)
        {
            lock (log)
            {
                if (s.GetType().Name.Equals("FlameSensor"))
                {
                    flameValue = (int) System.Math.Round(value);
                    if (indexFlame == -1)
                    {
                        indexFlame = counter++;
                        counter++;
                    }
                    log[indexFlame] = "";
                    log[indexFlame+1] = "Flame: " + flameValue;
                }
                if (s.GetType().Name.Equals("COSensor"))
                {
                    coValue = (int)System.Math.Round(value);
                    if (indexCO == -1)
                    {
                        indexCO = counter++;
                        counter++;
                    }
                    log[indexCO] = "";
                    log[indexCO + 1] = "CO: " + coValue;
                }
                if (s.GetType().Name.Equals("SmokeSensor"))
                {
                    smokeValue = (int)System.Math.Round(value);
                    if (indexSmoke == -1)
                    {
                        indexSmoke = counter++;
                        counter++;
                    }
                    log[indexSmoke] = "";
                    log[indexSmoke + 1] = "Smoke: " + smokeValue;
                }
            }
        }
        
    }
}