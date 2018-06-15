using System;
using System.Collections;
using System.Threading;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Networking;
using System.Text;
using NETMFBook.Sensors;
using NETMFBook.Database;
using NETMFBook.GeoScan;

namespace NETMFBook
{
    public partial class Program
    {
        private bool status;
        private static AutoResetEvent mountEvent = new AutoResetEvent(false);
        private Mqtt mqtt;
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            GT.Timer timer = new GT.Timer(2000);
            timer.Tick += timer_Tick;
            timer.Start();
            DisplayLCD.addSDInfo(false, 0);
            new Thread(() => {
                try
                {
                    init();
                }
                catch (Exception)
                {
                    PowerState.RebootDevice(true);
                }
                
            }).Start();
            Debug.Print("Program Started");
            
        }
        
        private void init()
        {
            Buzzer.init(breakout2.CreatePwmOutput(GT.Socket.Pin.Nine));
            StatusLed.led = ledStrip;
            StatusLed.led.SetLed(0, true);
            DisplayLCD.lcd = displayTE35;
            DisplayTimer();
            while (!wifi.NetworkInterface.Opened)
            {
                try
                {
                    Debug.Print("Opening Wifi interface");
                    wifi.NetworkInterface.Open();
                    Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    continue;
                }
            }
            GeoMessage geomessage = new GeoMessage(wifi.NetworkInterface.Scan());
            wifi.NetworkInterface.Close();
            RemovableMedia.Insert += (sender, e) => {
                mountEvent.Set(); Debug.Print("SD Mounted"); };
            while(!sdCard.IsCardInserted)
            {
                DisplayLCD.addSDInfo(false, 0);
                Thread.Sleep(1000);
                Debug.Print("Waiting for sd card");
            }
           
            while (!sdCard.IsCardMounted)
            {
                DisplayLCD.addSDInfo(false, 0);
                Thread.Sleep(1000);
                if(!sdCard.IsCardMounted)
                    sdCard.Mount();
            }
            //mountEvent.WaitOne();
            //byte[] data = Encoding.UTF8.GetBytes("Hello World!");
            //sdCard.StorageDevice.WriteFile("measure" + 0, data);
            //sdCard.StorageDevice.CreateDirectory(@"test");
            if (VolumeInfo.GetVolumes()[0].IsFormatted)
            {
                string rootDirectory =
                    VolumeInfo.GetVolumes()[0].RootDirectory;
                string[] files = Directory.GetFiles(rootDirectory);
                string[] folders = Directory.GetDirectories(rootDirectory);

                Debug.Print("Files available on " + rootDirectory + ":");
                for (int i = 0; i < files.Length; i++)
                {
                    Debug.Print("Deleted " + files[i]);
                    sdCard.StorageDevice.Delete(files[i]);
                }
                Debug.Print("Folders available on " + rootDirectory + ":" + folders.Length);
            }
            else
            {
                Debug.Print("Storage is not formatted. " +
                    "Format on PC with FAT32/FAT16 first!");
            }
            DisplayLCD.addSDInfo(true,0);
            Ethernet eth = new Ethernet(ethernetJ11D);
            Debug.Print("Ethernet created");
            mqtt = eth.MQTT;
            Debug.Print("Mqtt created");
            MeasureOrchestrator.setMqtt(mqtt);
            MeasureDB.sd = sdCard;
            Debug.Print("Time updated");
            TimeSync.update();
            while (!mqtt.isConnected())
            {
                Thread.Sleep(1000);
            }
            POSTContent pc = POSTContent.CreateTextBasedContent(GeoMessage.Json(geomessage));
            try
            {
                HttpRequest wc = HttpHelper.CreateHttpPostRequest("http://52.57.156.220/geolocation", pc, "application/json");
                wc.ResponseReceived += wc_ResponseReceived;
                wc.SendRequest();
            }
            catch (Exception) {
                mqtt.Publish("cfg", Configuration.Json(new Configuration(45.0631, 7.66004)));
            }

            //send a request with GeoMessage.Json(message) and set the configuration
            FlameSensor flame = new FlameSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Three), "0");
            SmokeSensor smoke = new SmokeSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Four), "1");
            COSensor co = new COSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Five), "2");
            TemperatureSensor temperature=new TemperatureSensor(breakout3.CreateAnalogInput(GT.Socket.Pin.Three),"3");
            registerSensor(temperature);
            registerSensor(smoke);
            registerSensor(co);
            registerSensor(flame);
            pubTimer(3000);
            pubOldTimer(2000);
        }

        void wc_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            try
            {
                Object o = Json.NETMF.JsonSerializer.DeserializeString(response.Text);
                Object location = (Object)((System.Collections.Hashtable)o)["location"];
                Double lat = (Double)((System.Collections.Hashtable)location)["lat"];
                Double lng = (Double)((System.Collections.Hashtable)location)["lng"];
                mqtt.Publish("cfg", Configuration.Json(new Configuration(lat, lng)));
                StatusLed.led.SetLed(4, true);
            }
            catch (Exception)
            {
                mqtt.Publish("cfg", Configuration.Json(new Configuration(45.0631, 7.66004)));
            }
        }

        private void registerSensor(Sensor sens)
        {
            MeasureOrchestrator.register(sens);
        }

        private void pubTimer(int time=20000) {
            GT.Timer timer = new GT.Timer(time);
            timer.Tick += (s) => MeasureOrchestrator.publish();
            MeasureDB.Timer = timer;
            timer.Start();
        }
        private void pubOldTimer(int time = 20000)
        {
            GT.Timer timer = new GT.Timer(time);
            timer.Tick += (s) => { s.Stop(); mqtt.PublishOld(MeasureOrchestrator.id); s.Start(); };
            MeasureDB.Timer = timer;
            timer.Start();
        }

        private void DisplayTimer(int time = 1000)
        {
            GT.Timer timer = new GT.Timer(time);
            timer.Tick += (s) => DisplayLCD.Refresh();
            timer.Start();
        }


        void timer_Tick(GT.Timer timer)
        {
            status = !status;
            Mainboard.SetDebugLED(status);
        }
    }
}
