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
        public static string ip = null, netmask = null, gateway = null;
        public static int indexIp = -1, indexNetmask = -1, indexGateway = -1, indexMqtt = -1;
        public static bool mqttConnected = false;
        private static void drawBuffer()
        {
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
                    buffer.DrawText(log[c], Resources.GetFont(Resources.FontResources.small), Color.White, startX, startY + (c * 10));
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

        public static void addNetInfo(String ipE, String netmaskE, String gatewayE)
        {
            lock (log)
            {
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
        
    }
}