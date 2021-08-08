using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Util;
using MindWatchA;

namespace MindWidgetA.StateMachine.RemoteComponents.RemoteViewsListView {

    [BroadcastReceiver(Label = "@string/widget_name")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    //[MetaData("android.appwidget.provider", Resource = "@xml/widget_word")]
    public class WidgetProvider : AppWidgetProvider
    {
        //readonly int N;
#nullable enable
        public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
        {
#nullable disable
            int N = appWidgetIds.Length;

            for (int i = 0; i < N; ++i)
            {
                
                RemoteViews remoteViews = updateWidgetListView(context, appWidgetIds[i]);

                appWidgetManager.UpdateAppWidget(appWidgetIds[i], remoteViews);
            }
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

        }

        [System.Obsolete]
        private RemoteViews updateWidgetListView(Context context, int appWidgetId)
        {

            //which layout to show on widget
            // TODO was
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_main);

            Intent svcIntent = new Intent(context, typeof(RemoteListViewService));
            //passing app widget id to that RemoteViews Service
            svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);

            //setting a unique Uri to the intent
            //don't know its purpose to me right now
            //svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToURI(Intent.UriIntentScheme)));
            svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(IntentUriType.Scheme)));

            Log.Debug("XXX", "WidgetProvider before SetRemoteAdapter");

            //setting adapter to listview of the widget
            remoteViews.SetRemoteAdapter(appWidgetId, Resource.Id.listText, svcIntent);
            //setting an empty view in case of no data
            remoteViews.SetEmptyView(Resource.Id.listText, Resource.Id.listText);

            Log.Debug("YYY", "WidgetProvider before return");


            return remoteViews;
        }

    } }