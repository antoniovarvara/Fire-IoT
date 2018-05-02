using System;
using Microsoft.SPOT;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;
using NETMFBook.Database;
using NETMFBook.Sensors;

namespace NETMFBook
{
    public class Mqtt
    {
        private IPAddress EndPoint = IPAddress.Parse("192.168.3.235");
        private MqttClient client;
        private Boolean isconnecting = false;
        public Mqtt() {
            client = new MqttClient(EndPoint);
            client.ConnectionClosed += client_ConnectionClosed;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            DisplayLCD.addMqttInfo(false);
            connectInfinite();
        }
        public void connectInfinite() {
                if (isconnecting == true) {
                    return;
                }
                else
                {
                    isconnecting = true;
                }
            
            new Thread(() =>
            {
                lock (client)
                {
                    while (client.IsConnected == false)
                    {
                        try
                        {
                            Thread.Sleep(2000);
                            connect();
                            isconnecting = false;
                            break;
                        }
                        catch (Exception)
                        {
                            Debug.Print("MQTT Connection FAILED");

                        }
                    }
                 }
            }).Start();
                
        }
        private void connect() {
            client.Connect("Fire_sensor_board");
            this.Subscribe("incendio");
            StatusLed.led.SetLed(3, true);
            DisplayLCD.addMqttInfo(true);
        }

        void client_ConnectionClosed(object sender, EventArgs e)
        {
            DisplayLCD.addMqttInfo(false);
            StatusLed.led.SetLed(3, false);
            connectInfinite();
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic.Equals("incendio"))
            {
                String message = new String(Encoding.UTF8.GetChars(e.Message));
                AlarmMessage m = new AlarmMessage(message);
                Buzzer.setState(m.Alarm);
            }
        }
        public ushort Subscribe(String topic) {
           return client.Subscribe(new String[]{topic}, new byte[] {MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE});
       }

        public ushort Publish(String Topic, String Message)
        {
            try
            {
                if (client.IsConnected == false)
                {
                    this.connectInfinite();
                    Debug.Print("MQTT Publish store in sd");
                    MeasureDB.addMeasure(Topic, Message);
                    return 0;
                }
                while (client.IsConnected == true && MeasureDB.hasPendingMeasure(Topic)){
                    String pendingMessage = MeasureDB.firstPendingMeasure(Topic);
                    try
                    {
                        Debug.Print("MQTT pending message publishing...");
                        client.Publish(Topic, Encoding.UTF8.GetBytes(pendingMessage), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,false);
                        Debug.Print("MQTT pending message published");
                    }
                    catch (Exception)
                    {
                        Debug.Print("MQTT Publish pending FAILED");
                        MeasureDB.addMeasure(Topic, pendingMessage);
                        MeasureDB.addMeasure(Topic, Message);
                        return 0;
                    }
                }
                Debug.Print("MQTT Publish"+Message);
                return client.Publish(Topic, Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            }
            catch(Exception e) {
                //Debug.Print(e.StackTrace);
                Debug.Print("MQTT Publish FAILED");
                return 0;
            }
        }
    }
}
