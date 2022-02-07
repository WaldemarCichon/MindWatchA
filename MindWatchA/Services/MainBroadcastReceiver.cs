using System;
using Android.Content;
using MindWatchA.Tooling;

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
            Logger.Info("in Main Broadcast Receiver, received action: " + intent.GetIntExtra("action", -1));
            Console.WriteLine("Received in main broadcast receiver");
            var instance = ServiceSingleton.Instance(context);
            Console.WriteLine(instance);
            if (intent.GetIntExtra("action", -1) == (int)ActionKind.Logout)
            {
                instance.Logout();
                return;
            }

            if (intent.GetIntExtra("action", -1) == (int)ActionKind.Login)
            {
                instance.Login();
                return;
            }
            var buttonAction = intent.GetStringExtra("buttonAction");
            instance.ButtonClicked(buttonAction);
        }
    }
}
