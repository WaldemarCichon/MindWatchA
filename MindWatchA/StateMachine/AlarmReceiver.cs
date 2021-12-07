using System;
using Android.App;
using Android.Content;
using Android.OS;

//** SEE https://www.journaldev.com/27681/android-alarmmanager-broadcast-receiver-and-service#:~:text=There%20are%20different%20types%20of%20Alarm%20Manager%20that,%E2%80%93%20This%20method%20came%20up%20with%20Android%20M.
//** SEE https://developer.android.com/training/scheduling/alarms#java
//** SEE above - start an alaem when the ddevice restarts
//** SEE https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/broadcast-receivers#:~:text=Xamarin.Android%20provides%20a%20C%23%20attribute%2C%20IntentFilterAttribute%2C%20that%20will,application%20to%20statically%20register%20for%20an%20implicit%20broadcast.
namespace MindWatchA.StateMachine
{
    [BroadcastReceiver(Label = "AlarmReceiver")]
    // [IntentFilter(new string[] { "", })]
    public class AlarmReceiver: BroadcastReceiver
    {
        public AlarmReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("****** Inside of Alarm Receiver ************");
            var currentTime = DateTime.Now;

            if (currentTime.Hour >= 8 && currentTime.Hour<=13)
            {
                checkAlarm(currentTime, startTime); // COMPARE with saved start time, fire event when day break inside
            } else if (currentTime.Hour < 20 Uhr)
            {
                checkAlarm(currentTime, midTime);
            }
        }
    }
}
