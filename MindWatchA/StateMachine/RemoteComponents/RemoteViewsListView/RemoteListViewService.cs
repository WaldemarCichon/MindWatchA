using System;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Util;
using Android.Widget;
using static Android.Widget.RemoteViewsService;

namespace MindWidgetA.StateMachine.RemoteComponents.RemoteViewsListView
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    public class RemoteListViewService: RemoteViewsService
    {
        public RemoteListViewService()
        {
        }

        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            //Log.Debug(ListWidgetProvider.TAG, "WidgetService -> OnGetViewFactory");

            int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId,
            AppWidgetManager.InvalidAppwidgetId);

            return (null); // new ListWidgetProvider(this.ApplicationContext, intent));
        }
    }
}
