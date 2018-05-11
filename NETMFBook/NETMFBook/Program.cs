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
        Message message;
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
            Buzzer.init(breakout2.CreateDigitalOutput(GT.Socket.Pin.Four, false));
            Ethernet eth = new Ethernet(ethernetJ11D);
            Debug.Print("Ethernet created");
            Mqtt mqtt = eth.MQTT;
            Debug.Print("Mqtt created");
            MeasureOrchestrator.setMqtt(mqtt);
            MeasureDB.sd = sdCard;
            TimeSync.update();
            Debug.Print("Time updated");
            while (!mqtt.isConnected())
            {
                Thread.Sleep(1000);
            }
            mqtt.Publish(MeasureOrchestrator.id, Configuration.Json(new Configuration()));
            SmokeSensor smoke = new SmokeSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Four), mqtt, "smoke");
            COSensor co = new COSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Five), mqtt, "co");
            FlameSensor flame = new FlameSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Three), mqtt, "flame");
            TemperatureSensor temperature=new TemperatureSensor(breakout3.CreateAnalogInput(GT.Socket.Pin.Three),mqtt,"temperature");
            registerSensor(temperature);
            registerSensor(smoke);
            registerSensor(co);
            registerSensor(flame);
            pubTimer(10000);
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
