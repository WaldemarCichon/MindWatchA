
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MindWatchA.Tooling;

namespace MindWatchA.UI
{
    [Activity(Label = "ShowLogsActivity")]
    public class ShowLogsActivity : Activity
    {
        TextView logsTextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.show_logs_activity);
            logsTextView = FindViewById<TextView>(Resource.Id.logs_textview);
            StringBuilder stringBuilder = new StringBuilder();
            var text = File.ReadAllLines(Logger.Path);
            stringBuilder = new StringBuilder();
            for (int i = text.Length-1; i>0; i--)
            {
                stringBuilder.Append(text[i]).Append("\n");
            }
            logsTextView.Text = stringBuilder.ToString();
        }
    }
}
