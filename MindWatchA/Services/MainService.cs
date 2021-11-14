using System;
using System.Collections.Generic;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Core.App;
using MindWidgetA.StateMachine;
using MindWidgetA.UI;

// SEE https://docs.microsoft.com/en-us/answers/questions/301019/xamarinandroid-widget-not-working-when-app-is-clos.html
// SEE https://stackoverflow.com/questions/60028587/background-service-with-isolated-process-in-xamarin-forms/60032046
// SEE https://www.titanwolf.org/Network/q/e5784f5b-c4f0-430b-910d-ef65905437df/y

// SEE https://developer.android.com/about/versions/oreo/background.html
namespace MindWatchA.Services
{
    [Service(Enabled = true, Exported = false)]
    public class MainService: JobIntentService
    {
        public const String HAPPY_BTN_CLICKED = "HappyButtonClicked";
        public const String NEUTRAL_BTN_CLICKED = "NeutralButtonClicked";
        public const String SAD_BTN_CLICKED = "SadButtonClicked";
        public const String INFO_BTN_CLICKED = "InfoBtnClicked";
        public const String BACK_BTN_CLICKED = "BackButtonClicked";
        public const String OK_BTN_CLICKED = "OkButtonClicked";
        public const String NO_BTN_CLICKED = "NoButtonClicked";

        private List<int> widgetIds;
        private ComponentName widget;
        private AbstractUI ui;
        private Handler handler;
        private Action runnable;
        private bool isStarted = true;
        private const String CHANNEL_ID = "MainServiceChannel";

        public MainService()
        {
            widgetIds = new List<int>();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Console.WriteLine("Service - on create");
            // createNotificationChannel();
            StartForeground(1, new Notification(18, "Text blah"));
            Console.WriteLine("Service created");
            /*
            handler = new Handler();
            //here is what you want to do always, i just want to push a notification every 5 seconds here
            runnable = new Action(() =>
            {
                if (isStarted)
                {
                    // DispatchNotification();
                    handler.PostDelayed(runnable, 5000);
                }
            });
            */

        }


        private void createNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                NotificationChannel serviceChannel = new NotificationChannel(
                        CHANNEL_ID,
                        "Foreground Service Channel",
                        Android.App.NotificationImportance.Default
                );
                NotificationManager manager = GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager))) as NotificationManager;
                manager.CreateNotificationChannel(serviceChannel);
            }
        }

        [Obsolete]
        public override void OnStart(Intent intent,  int startId)
        //public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // base.OnStartCommand(intent, flags, startId);
            Console.WriteLine("Service - checking action");
            var action = intent.GetIntExtra("action",(int) ActionKind.None);
            if (action == (int)ActionKind.Startup)
            {
                Console.WriteLine("Service - checking action");
                var ids = intent.GetIntArrayExtra("widgetIds");
                widgetIds.AddRange(ids);
                widget = intent.GetParcelableExtra("componentName") as ComponentName;
                var context = Application.Context;
                var appWidgetManager = AppWidgetManager.GetInstance(context);
                Init(context, appWidgetManager);
                Console.WriteLine("Service - on startup finished");
            } else
            {
                if (action == (int)ActionKind.ButtonClicked)
                {
                    var buttonAction = intent.GetStringExtra("buttonAction");
                    buttonClicked(buttonAction);
                }
            }
            StopSelf(startId);
            //return StartCommandResult.Sticky;
        }

        private void buttonClicked(string buttonAction)
        {
            Console.WriteLine("Button clicked");
            Console.WriteLine(buttonAction);

            buttonAction = "";
            switch (buttonAction)
            {
                case INFO_BTN_CLICKED: ui.InfoButton.Clicked(this, null); break;
                case HAPPY_BTN_CLICKED: ui.HappyButton.Clicked(this, null); break;
                case NEUTRAL_BTN_CLICKED: ui.NeutralButton.Clicked(this, null); break;
                case SAD_BTN_CLICKED: ui.SadButton.Clicked(this, null); break;
                case BACK_BTN_CLICKED: ui.BackButton.Clicked(this, null); break;
                case OK_BTN_CLICKED: ui.OkButton.Clicked(this, null); break;
                case NO_BTN_CLICKED: ui.NoButton.Clicked(this, null); break;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Console.WriteLine("Destroying service");
        }

        private void Init(Context context, AppWidgetManager appWidgetManager)
        {
            Console.WriteLine("Update started");
            RemoteViews remoteViews;
            ComponentName widget;

            remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_main);
            widget = new ComponentName(context, Java.Lang.Class.FromType(typeof(MainWidget))); //  new ComponentName(context, typeof(Widget));

            Console.WriteLine("Creating Abstract UI");

            ui = AbstractUI.Instance;
            ui.SetBaseData(remoteViews, widget, appWidgetManager);

            Console.WriteLine("Base data set");

            remoteViews.SetOnClickPendingIntent(Resource.Id.info_widget, GetPendingSelfIntent(context, INFO_BTN_CLICKED));
            remoteViews.SetOnClickPendingIntent(Resource.Id.happy_widget, GetPendingSelfIntent(context, HAPPY_BTN_CLICKED));
            remoteViews.SetOnClickPendingIntent(Resource.Id.neutral_widget, GetPendingSelfIntent(context, NEUTRAL_BTN_CLICKED));
            remoteViews.SetOnClickPendingIntent(Resource.Id.sad_widget, GetPendingSelfIntent(context, SAD_BTN_CLICKED));
            remoteViews.SetOnClickPendingIntent(Resource.Id.back_widget, GetPendingSelfIntent(context, BACK_BTN_CLICKED));
            remoteViews.SetOnClickPendingIntent(Resource.Id.ok_widget, GetPendingSelfIntent(context, OK_BTN_CLICKED));
            remoteViews.SetOnClickPendingIntent(Resource.Id.no_widget, GetPendingSelfIntent(context, NO_BTN_CLICKED));

            appWidgetManager.UpdateAppWidget(widget, remoteViews);

        }

        protected PendingIntent GetPendingSelfIntent(Context context, String action)
        {
            Intent intent = new Intent(context, typeof(MainWidget));
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(context, 0, intent, 0);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        protected override void OnHandleWork(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}
