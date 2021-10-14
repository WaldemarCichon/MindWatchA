using System;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using MindWatchA;
using MindWatchA.Services;
using MindWidgetA.StateMachine;
using MindWidgetA.StateMachine.RemoteComponents;

namespace MindWidgetA.UI
{
    [BroadcastReceiver(Label = "Mind Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE", })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class MainWidget : AppWidgetProvider 
    {

        // private const String Logid = "de.cisoft.MindWidgetA.MainWidget";
        // for border: https://stackoverflow.com/questions/3496269/how-do-i-put-a-border-around-an-android-textview

        private static AbstractUI ui;

        public MainWidget()
        {

        }

        public override void OnRestored(Context context, int[] oldWidgetIds, int[] newWidgetIds)
        {
            base.OnRestored(context, oldWidgetIds, newWidgetIds);
            Console.WriteLine("Widget - on restored");
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);
            Intent intent = new Intent(context.ApplicationContext, typeof(MainService));
            context.StopService(intent);
            Console.WriteLine("Widget - on deleted");
        }

        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
            Console.WriteLine("Widget - on options changed");
        }

        public override void OnEnabled(Context context)
        {
            base.OnEnabled(context);
            Console.WriteLine("Widget - on enabled");
        }

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            Console.WriteLine("Update started");
            Console.WriteLine("Context = " + context);
            var widget = new ComponentName(context, Java.Lang.Class.FromType(typeof(MainWidget)));
            var allIds = appWidgetManager.GetAppWidgetIds(widget);
            Console.WriteLine("Starting service - building intent");
            var service = new ComponentName(context.ApplicationContext, Java.Lang.Class.FromType(typeof(MainService)));
            Intent intent = new Intent(context.ApplicationContext, typeof(MainService));
            // intent.SetComponent(service);
            intent.PutExtra("action", (int)ActionKind.Startup);
            intent.PutExtra("widgetIds", allIds);
            intent.PutExtra("componentName", widget);
            Console.WriteLine("Starting service");
            context.StartService(intent);
            Console.WriteLine("Update finished");
        }

        public override void OnReceive(Context context, Intent intent) {
            base.OnReceive(context, intent);
            Console.WriteLine("In OnReceive");
            Console.WriteLine(intent.Action, intent.Component);
            if (intent.Action.EndsWith("Clicked"))
            {
                Console.WriteLine("Prepairing service call");
                var serviceIntent = new Intent(context.ApplicationContext, typeof(MainService));
                serviceIntent.PutExtra("action", (int)ActionKind.ButtonClicked);
                serviceIntent.PutExtra("buttonAction", intent.Action);
                context.StartService(serviceIntent);
                Console.WriteLine("Service called");
            }
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_main);
            Console.WriteLine(remoteViews);
            ComponentName widget = new ComponentName(context, Java.Lang.Class.FromType(typeof(MainWidget)));
            Console.WriteLine(widget);
        }


    }
}