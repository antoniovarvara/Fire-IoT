using System;
using Microsoft.SPOT;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;
using NETMFBook.Database;
using NETMFBook.Sensors;
using System.Collections;
namespace NETMFBook
{
    public class Mqtt
    {
        private IPAddress EndPoint = IPAddress.Parse("192.168.3.235");
        private MqttClient client;
        private Boolean isconnecting = false;
        private Hashtable pendingSend = new Hashtable();
        public Mqtt() {
            client = new MqttClient(EndPoint);
            //client.Settings.InflightQueueSize = 5;
            client.ConnectionClosed += client_ConnectionClosed;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.MqttMsgPublished += client_MqttMsgPublished;
            DisplayLCD.addMqttInfo(false);
            connectInfinite();
        }

        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            if (!e.IsPublished)
            {
                String message = (String)pendingSend[e.MessageId];
                if (message != null) {
                    MeasureDB.addMeasure(message);
                    pendingSend.Remove(e.MessageId);
                }
            }
            Debug.Print("MQTT Publish status:"+ e.IsPublished+ " message "+ e.MessageId);
        }
        public bool isConnected()
        {
            return client.IsConnected;
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
            client.Connect("Fire_sensor_board",null,null,true,3);
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
                StatusLed.led.SetLed(5, m.Alarm);
                StatusLed.led.SetLed(6, m.Alarm);
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
                    Debug.Print("MQTT Publish store in sd: "+ Message);
                    MeasureDB.addMeasure(Message);
                    return 0;
                }
                //PublishOld(Topic, Message);
                Debug.Print("MQTT Publish"+Message);
                try
                {
                    ushort retval=client.Publish(Topic, Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                    pendingSend.Add(retval, Message);
                    return retval;
                }
                catch (Exception)
                {
                    Debug.Print("MQTT Publish pending FAILED");
                    MeasureDB.addMeasure(Message);
                    //try { client.Disconnect();}catch (Exception) { };
                    return 0;
                }
            }
            catch(Exception) {
                //Debug.Print(e.StackTrace);
                Debug.Print("MQTT Publish FAILED");
                return 0;
            }
        }
        public ushort PublishOld(String Topic, String Message=null) {
            if (client.IsConnected == true && MeasureDB.hasPendingMeasure())
            {
                if (Topic.Equals("cfg"))
                {
                    return 0;
                }
                String pendingMessage = MeasureDB.firstPendingMeasure();
                try
                {
                    Debug.Print("MQTT pending message publishing...");
                    pendingSend.Add(client.Publish(Topic, Encoding.UTF8.GetBytes(pendingMessage), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false), pendingMessage);
                    Debug.Print("MQTT pending message published");
                }
                catch (Exception)
                {
                    Debug.Print("MQTT Publish pending FAILED");
                    MeasureDB.addMeasure(pendingMessage);
                    if (Message != null)
                    {
                        MeasureDB.addMeasure(Message);
                    }
                    return 0;
                }
            }
            return 0;
        }

    }
}
