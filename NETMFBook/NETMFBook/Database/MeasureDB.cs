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
        public static SDCard sd { get; set; }
        private static long id = 0;
        private const long MAX_N_FILE = 30;
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
            lock (sd)
            {
                sd.StorageDevice.WriteFile("measure" + id, data); 
                Debug.Print(sensor + " created new file " + id + ": " + m);
                id = (id + 1) % MAX_N_FILE;
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
            int n;

            lock (sd)
            {
            if (VolumeInfo.GetVolumes()[0].IsFormatted)
            {
                string rootDirectory =
                    VolumeInfo.GetVolumes()[0].RootDirectory;
                string[] files = Directory.GetFiles(rootDirectory);
                string[] folders = Directory.GetDirectories(rootDirectory);

                Debug.Print("Files available on " + rootDirectory + ":");
                for (int i = 0; i < files.Length; i++)
                {
                    Debug.Print(files[i]);
                }
                Debug.Print("Folders available on " + rootDirectory + ":");
            }
                n = sd.StorageDevice.ListFiles("").Length;
                DisplayLCD.addSDInfo(true, n);
                Debug.Print("File in sd " + n);
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
                    Debug.Print("Removed " + files[0]);
                    FileStream f = sd.StorageDevice.OpenRead(files[0]);
                    StreamReader reader = new StreamReader(f);
                    message = reader.ReadToEnd();
                    f.Close();
                    sd.StorageDevice.Delete(files[0]);
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
