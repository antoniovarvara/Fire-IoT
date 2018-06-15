using System;
using System.Net;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Time;
using Gadgeteer.Networking;
namespace NETMFBook
{
    class TimeSync
    {
        public static AutoResetEvent connectionEvent = new AutoResetEvent(false);
        public static AutoResetEvent timeSetted = new AutoResetEvent(false);
        private const int localTimeZone = +2 * 60; //CEST

        public static void update() {

            try
            {
                connectionEvent.WaitOne();
                /*var NTPTime = new TimeServiceSettings();
                NTPTime.AutoDayLightSavings = true;
                NTPTime.ForceSyncAtWakeUp = true;
                NTPTime.RefreshTime = 3600;
                NTPTime.PrimaryServer = IPAddress.Parse("192.168.3.235").GetAddressBytes();
                NTPTime.AlternateServer = IPAddress.Parse("193.204.114.233").GetAddressBytes();
                TimeService.Settings = NTPTime;
                TimeService.SetTimeZoneOffset(localTimeZone); // UTC+2 Time zone : CEST
                TimeService.SystemTimeChanged += TimeService_SystemTimeChanged;
                TimeService.TimeSyncFailed += TimeService_TimeSyncFailed;
                TimeService.Start();
                Debug.Print("Time Service started");
                //TimeService.UpdateNow(IPAddress.Parse("192.168.3.235").GetAddressBytes(), 2000);
                Debug.Print("Time Service updating...");
                Thread.Sleep(1000);
                while (DateTime.Now.Ticks < 1527248910)
                {
                    Debug.Print("Waiting for time update");
                    Thread.Sleep(1000);
                }
                Debug.Print("It is : " + DateTime.Now.ToString());
                DateTime time = DateTime.Now;
                Utility.SetLocalTime(time);
                TimeService.Stop();
                Debug.Print("Time Service updated");
                */
                Debug.Print("Time Service started");
                while (DateTime.Now.Ticks < 130717343174511258)
                {
                    Thread.Sleep(2000);
                    try
                    {

                        HttpRequest requestTime = HttpHelper.CreateHttpGetRequest("http://52.57.156.220/time");
                        requestTime.ResponseReceived += time_ResponseReceived;
                        requestTime.SendRequest();
                        timeSetted.WaitOne();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                Debug.Print("Time Service updated");
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        
        
        }


        static void time_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            try
            {
                DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                Object o = Json.NETMF.JsonSerializer.DeserializeString(response.Text);
                long timestamp = (long)((System.Collections.Hashtable)o)["timestamp"];
                Utility.SetLocalTime(new DateTime(timestamp * TimeSpan.TicksPerSecond + unixStart.Ticks));
                timeSetted.Set();
            }
            catch (Exception)
            {
                Debug.Print("Error in time service");
                timeSetted.Set();
                update();
            }
        }

        static void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {
            Debug.Print("Ntp sync OK!");
        }

        static void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {
            Debug.Print("Ntp sync Fail!");
        }
    }
}
