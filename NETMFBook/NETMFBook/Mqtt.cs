using System;
using Microsoft.SPOT;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;
using NETMFBook.Database;

namespace NETMFBook
{
    public class Mqtt
    {
        private IPAddress EndPoint = IPAddress.Parse("52.57.156.220");
        private MqttClient client;
        public event uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishEventHandler PublishEvent;
        public Mqtt() {
            client = new MqttClient(EndPoint);
            client.ConnectionClosed += client_ConnectionClosed;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            DisplayLCD.addMqttInfo(false);
            connectInfinite();
        }
        public void connectInfinite() {
            new Thread(() =>
            {
                lock (client)
                {
                    while (client.IsConnected == false)
                    {
                        try
                        {
                            connect();
                            break;
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(5000);
                            Debug.Print("MQTT Connection FAILED");

                        }
                    }
                 }
            }).Start();
                
        }
        private void connect() {

            client.Connect("fez", "utente", "fezspiderII");
            StatusLed.led.SetLed(3, true);
            DisplayLCD.addMqttInfo(true);
        }

        void client_ConnectionClosed(object sender, EventArgs e)
        {
            DisplayLCD.addMqttInfo(false);
            StatusLed.led.SetLed(3, false);
            //connectInfinite();
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (PublishEvent != null) {
 
                PublishEvent.Invoke(sender,e);
            }
        }
        public ushort Subscribe(String topic) {
            if (client.IsConnected == false)
            {
                //this.connectInfinite();
            }
            return client.Subscribe(new String[]{topic}, new byte[] {MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE});
        }

        public ushort Publish(String Topic, String Message)
        {
            try
            {
                if (client.IsConnected == false)
                {
                    //this.connectInfinite();
                    Debug.Print("MQTT Publish store in sd");
                    MeasureDB.addMeasure(Topic, Message);
                    return 0;
                }
                while (client.IsConnected == true && MeasureDB.hasPendingMeasure(Topic)){
                    String pendingMessage = MeasureDB.firstPendingMeasure(Topic);
                    try
                    {
                        Debug.Print("MQTT pending message publishing...");
                        client.Publish(Topic, Encoding.UTF8.GetBytes(pendingMessage), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
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
                return client.Publish(Topic, Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
            }
            catch(Exception e) {
                //Debug.Print(e.StackTrace);
                Debug.Print("MQTT Publish FAILED");
                return 0;
            }
        }
    }
}
