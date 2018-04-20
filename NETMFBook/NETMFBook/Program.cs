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

namespace NETMFBook
{
    public partial class Program
    {
        private bool status;
        private static AutoResetEvent mountEvent = new AutoResetEvent(false);
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            GT.Timer timer = new GT.Timer(2000);
            timer.Tick += timer_Tick;
            timer.Start();
            DisplayLCD.addSDInfo(false, 0);
            new Thread(() => init()).Start();
            Debug.Print("Program Started");

        }
        
        private void init()
        {
            StatusLed.led = ledStrip;
            StatusLed.led.SetLed(0, true);
            DisplayLCD.lcd = displayTE35;
            DisplayTimer();
            RemovableMedia.Insert += (sender, e) => { mountEvent.Set(); Debug.Print("SD Mounted"); };
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
            mountEvent.WaitOne();
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
            Mqtt mqtt = eth.MQTT;
            MeasureDB.sd = sdCard;
            TimeSync.update();
            //mqtt.Publish("status", "ciao");
            mqtt.Subscribe("led");
            mqtt.PublishEvent += mqtt_PublishEvent;
            SmokeSensor smoke = new SmokeSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Four), mqtt, "smoke");
            COSensor co = new COSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Five), mqtt, "co");
            FlameSensor flame = new FlameSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Three), mqtt, "flame");
            //TemperatureSensor temperature=new TemperatureSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Three),mqtt,"temperature");
            Buzzer b = new Buzzer(breakout2.CreateDigitalOutput(GT.Socket.Pin.Four, true), mqtt, "incendio");
            b.subscribe();
            pubTimer(smoke, 3000);
            Thread.Sleep(500);
            pubTimer(co, 3000);
            Thread.Sleep(500);
            pubTimer(flame, 3000);
        }

        private void pubTimer(Sensor sens,int time=20000) {
            GT.Timer timer = new GT.Timer(time);
            timer.Tick += (s) => sens.publish();
            MeasureDB.mapTimers.Add(sens.name, timer);
            timer.Start();
        }

        private void DisplayTimer(int time = 1000)
        {
            GT.Timer timer = new GT.Timer(time);
            timer.Tick += (s) => DisplayLCD.Refresh();
            timer.Start();
        }

        void mqtt_PublishEvent(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            if (e.Topic == "led")
            {
                /*
                Application.Current.Dispatcher.Invoke(TimeSpan.Zero, (displayTE35obj) =>
                {
                    ((DisplayTE35)displayTE35obj).SimpleGraphics.DisplayText(new String(Encoding.UTF8.GetChars(e.Message)), Resources.GetFont(Resources.FontResources.NinaB), GT.Color.Blue, 10, 10);
                    return 0;
                }, displayTE35);
            
                 */
           }
        }

        void timer_Tick(GT.Timer timer)
        {
            status = !status;
            Mainboard.SetDebugLED(status);
        }
    }
}
