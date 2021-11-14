using System;
using System.Collections.Generic;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using MindWatchA;
using MindWidgetA.StateMachine.RemoteComponents;
using MindWidgetA.UI;

namespace MindWidgetA.StateMachine
{
    public class TextViewProxy
    {
        private TextView textView;
        private ListView listView;
        private string _text;
        private ViewStates _visibility;
        public  RemoteTextView remoteTextView;
        private List<String> listViewText;

        public TextViewProxy()
        {
            
        }

        public void Register (TextView textView)
        {
            this.textView = textView;
        }

        public void Register (RemoteTextView remoteTextView)
        {
            this.remoteTextView = remoteTextView;
        }

        public void Register (ListView listView, Context context)
        {
            this.listView = listView;
            listViewText = new List<String>(1);
            listViewText.Add("Wie fühlst Du Dich heute?");
            listView.Adapter = new ArrayAdapter<String>(context, Resource.Layout.list_view_single_element, Resource.Id.listText, listViewText);
        }

        public String Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                if (textView != null)
                {
                    textView.Text = value;
                    Console.WriteLine("Setting textView text to " +value);
                }
                if (remoteTextView != null)
                {
                    Console.WriteLine("Old Value" + remoteTextView.Text);
                    Console.WriteLine("Setting remote text to " + value);
                   
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => remoteTextView.Text = value);
                    Console.WriteLine("CurrentValue" + remoteTextView.Text);
                    var remoteViews = remoteTextView.RemoteViews;
                    var context = Application.Context;
                    // var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_main);
                    remoteViews.SetTextViewText(Resource.Id.mainText, value);
                    var appWidgetManager = AppWidgetManager.GetInstance(context);
                    ComponentName widget = new ComponentName(context, Java.Lang.Class.FromType(typeof(MainWidget)));
                    appWidgetManager.UpdateAppWidget(widget, remoteViews);
                    Console.WriteLine("Text set or not");

                }
                if (listView != null)
                {
                    Console.WriteLine("Setting listview text to " + value);
                    listViewText[0] = value;

                    if (listView.Adapter != null) {
                        ((ArrayAdapter)listView.Adapter).Clear();
                        ((ArrayAdapter)listView.Adapter).Add(value);
                        //listView.NotifyDataSetChanged();
                    }
                    listView.Invalidate();
                }
            }
        }

        public ViewStates Visibility
        {
            get
            {
                return _visibility;
            }

            set
            {
                _visibility = value;
                if (textView != null)
                {
                    textView.Visibility = _visibility;
                }
                if (remoteTextView != null)
                {
                    remoteTextView.Visiblity = _visibility;  
                }
                if (listView != null)
                {
                    listView.Visibility = _visibility;
                }
            }
        }
    }
}
