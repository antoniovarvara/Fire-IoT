using System;
using System.Net;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Time;

namespace NETMFBook
{
    class TimeSync
    {
        public static AutoResetEvent connectionEvent = new AutoResetEvent(false);
        private const int localTimeZone = +1 * 60;

        public static void update() {

            try
            {
                connectionEvent.WaitOne();
                var NTPTime = new TimeServiceSettings();
                NTPTime.AutoDayLightSavings = true;
                NTPTime.ForceSyncAtWakeUp = true;
                NTPTime.RefreshTime = 3600;
                NTPTime.PrimaryServer = IPAddress.Parse("193.204.114.232").GetAddressBytes();
                NTPTime.AlternateServer = IPAddress.Parse("193.204.114.233").GetAddressBytes();
                TimeService.Settings = NTPTime;
                TimeService.SetTimeZoneOffset(localTimeZone); // UTC+1 Time zone : GMTè1
                TimeService.SystemTimeChanged += TimeService_SystemTimeChanged;
                TimeService.TimeSyncFailed += TimeService_TimeSyncFailed;
                TimeService.Start();
                Debug.Print("Time Service started");
                Thread.Sleep(3000);
                TimeService.UpdateNow(0);
                Debug.Print("Time Service updating...");
                Thread.Sleep(1000);
                Debug.Print("It is : " + DateTime.Now.ToString());
                DateTime time = DateTime.Now;
                Utility.SetLocalTime(time);
                TimeService.Stop();
                Debug.Print("Time Service updated");
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
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
