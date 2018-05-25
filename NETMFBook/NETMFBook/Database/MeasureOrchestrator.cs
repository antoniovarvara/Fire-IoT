using System;
using Microsoft.SPOT;
using System.Collections;
using NETMFBook.Sensors;
namespace NETMFBook.Database
{
    static class MeasureOrchestrator
    {
        public static String id = "FEZ_24";
        private static Message lastMessage = null;
        private static ArrayList sensors = new ArrayList();
        private static Mqtt mqtt;
        private static int repetition = 0;

        public static void setMqtt(Mqtt m)
        {
            mqtt = m;
        }

        public static void register(Sensor sens)
        {
            sensors.Add(sens);
        }

        public static void publish()
        {
            Message msg = new Message(id);
            bool publishNeed = false;

            foreach (Sensor s in sensors)
            {
                double reading = s.read();
                publishNeed = publishNeed | s.changedSignificantly();
                msg.addMeasure(new Measure(s.name, s.checkValidity(reading), reading));
            }
            if (lastMessage == null || publishNeed || repetition > 10 ){
                    repetition = 0;
                    mqtt.Publish(id, Message.Json(msg));
                    lastMessage = msg;
               }
            else
            {
                repetition++;
            }
        }


    }

    

}
