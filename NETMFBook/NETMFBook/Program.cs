using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using System.Text;
using NETMFBook.Sensors;

namespace NETMFBook
{
    public partial class Program
    {
        private bool status;
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/
            GT.Timer timer = new GT.Timer(2000);
            timer.Tick += timer_Tick;
            timer.Start();
            StatusLed.led = ledStrip;
            StatusLed.led.SetLed(0, true);
            Ethernet eth = new Ethernet(ethernetJ11D);
            Mqtt mqtt = eth.MQTT;
            TimeSync.update();
            mqtt.Publish("status", "ciao");
            mqtt.Subscribe("led");
            mqtt.PublishEvent += mqtt_PublishEvent;
            SmokeSensor smoke = new SmokeSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Four), mqtt,"smoke");
            COSensor co = new COSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Five), mqtt, "co");
            FlameSensor flame = new FlameSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Three), mqtt, "flame");
            //TemperatureSensor temperature=new TemperatureSensor(breakout.CreateAnalogInput(GT.Socket.Pin.Three),mqtt,"temperature");
            breakout2.CreateDigitalOutput(GT.Socket.Pin.Four, true);
            pubTimer(smoke,3000);
            Thread.Sleep(500);
            pubTimer(co,3000);
            Thread.Sleep(500);
            pubTimer(flame,3000);
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

        }
        private void pubTimer(Sensor sens,int time=20000) {
            GT.Timer timer = new GT.Timer(time);
            timer.Tick += (s) => sens.publish();
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
