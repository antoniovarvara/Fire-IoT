using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using System;
using System.IO;
using System.Text;
using System.Collections;
using GT = Gadgeteer;
using NETMFBook.Sensors;
using Microsoft.SPOT.IO;
namespace NETMFBook.Database
{
    public static class MeasureDB
    {
        public static Hashtable mapTimers = new Hashtable();
        public static Hashtable mapMeasure = new Hashtable();
        private static Hashtable mapId = new Hashtable();
        public static SDCard sd { get; set; }
        private const long MAX_N_FILE = 30;
        private static int standardTime = 3000;

        public static void addMeasure(String sensor, String m)
        {
            byte[] data = Encoding.UTF8.GetBytes(m);
            lock (sd)
            {
                if (!mapMeasure.Contains(sensor))
                {
                    mapMeasure.Add(sensor, new ArrayList());
                    mapId.Add(sensor, "0");
                    Debug.Print("Added new sensor in the maps: " + sensor);
                }
                long id = Convert.ToInt64((String)mapId[sensor]);
                String filename = sensor + "_measure_" + id;
                ArrayList l = (ArrayList)mapMeasure[sensor];
                l.Add(filename);
                Debug.Print(sensor + " created new file " + id + ": " + m);
                sd.StorageDevice.WriteFile(filename, data);
                mapId[sensor] = ((id + 1) % MAX_N_FILE).ToString();
                /*if (id < 3 / 4 * MAX_N_FILE)
                {
                    //imposta timer con intervallo standard
                    GT.Timer timer = (GT.Timer)mapTimers[sensor];
                    System.TimeSpan interval = new TimeSpan(standardTime);
                    timer.Interval = interval;
                }
                else
                {
                    //imposta timer con intervallo doppio
                    GT.Timer timer = (GT.Timer)mapTimers[sensor];
                    System.TimeSpan interval = new TimeSpan(standardTime * 2);
                    timer.Interval = interval;
                }*/
                Debug.Print(sensor + " added new pending measure " + m);
                DisplayLCD.addSDInfo(true, sd.StorageDevice.ListFiles("").Length);
            }
            
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
            int n = 0;
            ArrayList l;
            lock (sd)
            {
                if (VolumeInfo.GetVolumes()[0].IsFormatted)
                {
                    string rootDirectory =
                        VolumeInfo.GetVolumes()[0].RootDirectory;
                    string[] files = Directory.GetFiles(rootDirectory);

                    Debug.Print("Files available on " + rootDirectory + ":");
                    for (int i = 0; i < files.Length; i++)
                        Debug.Print(files[i]);
                    if (mapMeasure.Count == 0) return false;
                    l = (ArrayList)mapMeasure[s];
                    Debug.Print(s + " has " + l.Count + " pending measures");
                    n = l.Count;
                    //n = sd.StorageDevice.ListFiles("").Length;
                    DisplayLCD.addSDInfo(true, sd.StorageDevice.ListFiles("").Length);
                    Debug.Print("File in sd " + n);
                }
                else
                {
                    Debug.Print("Storage is not formatted. " +
                        "Format on PC with FAT32/FAT16 first!");
                }
                
            }
            return n != 0;
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
            String message = "";
            ArrayList l;
            Debug.Print("Sending first Pending measure...");
            lock (sd)
            {
                if (VolumeInfo.GetVolumes()[0].IsFormatted)
                {
                    string rootDirectory =
                        VolumeInfo.GetVolumes()[0].RootDirectory;
                    string[] files = Directory.GetFiles(rootDirectory);

                    Debug.Print("Files available on " + rootDirectory + ":");
                    for (int i = 0; i < files.Length; i++)
                        Debug.Print(files[i]);
                    l = (ArrayList)mapMeasure[s];
                    String filename = "";
                    if (l.Count > 0)
                    {
                        filename = (String)l[0];
                        l.RemoveAt(0);
                        Debug.Print(s + " removed " + filename);
                        FileStream f = sd.StorageDevice.OpenRead(filename);
                        StreamReader reader = new StreamReader(f);
                        message = reader.ReadToEnd();
                        f.Close();
                        sd.StorageDevice.Delete(filename);
                    }
                }
                else
                {
                    Debug.Print("Storage is not formatted. " +
                        "Format on PC with FAT32/FAT16 first!");
                }
                DisplayLCD.addSDInfo(true, sd.StorageDevice.ListFiles("").Length);
                
            }
            return message;
        }

    }
}
