using System;
using Android.Content;

namespace MindWatchA.Services
{
    [BroadcastReceiver(Label = "MainBroadcastReceiver")]
    public class MainBroadcastReceiver: BroadcastReceiver
    {
        public MainBroadcastReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("Received in main broadcast receiver");
            var instance = ServiceSingleton.Instance(context);
            Console.WriteLine(instance);
            var buttonAction = intent.GetStringExtra("buttonAction");
            instance.ButtonClicked(buttonAction);
        }
    }
}
