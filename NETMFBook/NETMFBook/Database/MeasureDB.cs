using Microsoft.SPOT;
using System;
using System.Collections;
using NETMFBook.Sensors;
namespace NETMFBook.Database
{
    public static class MeasureDB
    {
        private static Hashtable map = new Hashtable();


        public static void addMeasure(String sensor, String m)
        {
            lock (map)
            {
                if (!map.Contains(sensor))
                {
                    map.Add(sensor, new ArrayList());
                }
                ArrayList l = (ArrayList)map[sensor];
                l.Add(m);
            }
            Debug.Print(sensor + " added new pending measure " + m);
        }

        public static bool hasPendingMeasure(String s)
        {
            ArrayList l;
            lock (map)
            {
                if (map.Count == 0) return false;
                l = (ArrayList)map[s];
            }
            Debug.Print(s + " has " + l.Count + " pending measures");
            return l.Count > 0;
        }

        public static String firstPendingMeasure(String s)
        {
            ArrayList l;
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
            return res;
        }

    }
}
