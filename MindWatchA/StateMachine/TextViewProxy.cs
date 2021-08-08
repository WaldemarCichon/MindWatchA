using System;
using System.Collections.Generic;
using Android.Content;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using MindWatchA;
using MindWidgetA.StateMachine.RemoteComponents;

namespace MindWidgetA.StateMachine
{
    public class TextViewProxy
    {
        private TextView textView;
        private ListView listView;
        private string _text;
        private ViewStates _visibility;
        private RemoteTextView remoteTextView;
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
                }
                if (remoteTextView != null)
                {
                    remoteTextView.Text = value;
                }
                if (listView != null)
                {
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
