using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using System;
using System.IO;
using System.Text;
using System.Collections;
using GT = Gadgeteer;
using NETMFBook.Sensors;
namespace NETMFBook.Database
{
    public static class MeasureDB
    {
        public static Hashtable mapTimers = new Hashtable();
        public static SDCard sd { get; set; }
        private static long id = 0;
        private static const long MAX_N_FILE = 30;
        private static int standardTime = 3000;

        public static void addMeasure(String sensor, String m)
        {
            /*lock (map)
            {
                if (!map.Contains(sensor))
                {
                    map.Add(sensor, new ArrayList());
                }
                ArrayList l = (ArrayList)map[sensor];
                l.Add(m);
            }*/
            byte[] data = Encoding.UTF8.GetBytes(m);
            sd.StorageDevice.WriteFile("measure" + id, data);
            id = (id + 1) % MAX_N_FILE;
            if (id < 3/4*MAX_N_FILE)
            {
                //imposta timer con intervallo standard
                GT.Timer timer = (GT.Timer) mapTimers[sensor];
                System.TimeSpan interval = new TimeSpan(standardTime);
                timer.Interval = interval;
            }
            else
            {
                //imposta timer con intervallo doppio
                GT.Timer timer = (GT.Timer)mapTimers[sensor];
                System.TimeSpan interval = new TimeSpan(standardTime*2);
                timer.Interval = interval;
            }
            Debug.Print(sensor + " added new pending measure " + m);
        }

        public static bool hasPendingMeasure(String s)
        {
            /*ArrayList l;
            lock (map)
            {
                if (sd.StorageDevice.ListFiles("").Length == 0) return false;

                if (map.Count == 0) return false;
                l = (ArrayList)map[s];
            }
            Debug.Print(s + " has " + l.Count + " pending measures");
            return l.Count > 0;*/
            return sd.StorageDevice.ListFiles("").Length == 0;
        }

        public static String firstPendingMeasure(String s)
        {
            /*ArrayList l;
            String res = null; 
            lock (map)
            {
                l = (ArrayList)map[s];
                if (l.Count > 0)
                {
                    res = (String)l[0];
                    l.RemoveAt(0);
                    Debug.Print(s + " removed " + res);
                }
            }
            return res;*/
            String[] files = sd.StorageDevice.ListFiles("");
            FileStream f = sd.StorageDevice.OpenRead(files[0]);
            StreamReader reader = new StreamReader(f);
            String message = reader.ReadToEnd();
            sd.StorageDevice.Delete(files[0]);
            return message;
        }

    }
}
