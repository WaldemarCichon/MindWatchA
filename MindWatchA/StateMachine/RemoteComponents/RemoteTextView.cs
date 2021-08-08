using System;
using Android.Widget;

namespace MindWidgetA.StateMachine.RemoteComponents
{
    public class RemoteTextView: RemoteComponent
    {
        private String _text;
        private bool isTextView;
        private ArrayAdapter<String> arrayAdapter;


        public RemoteTextView(int componentId, bool isTextView = true): base(componentId)
        {

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
                if (isTextView)
                {
                    RemoteViews.SetTextViewText(ComponentId, value);
                } else
                {
                    
                }
                AppWidgetManager.UpdateAppWidget(Widget, RemoteViews);
            }
        }

    }
}
