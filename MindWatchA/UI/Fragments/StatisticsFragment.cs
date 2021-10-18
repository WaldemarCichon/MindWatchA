
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AndroidX.Fragment.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.Charts;

namespace MindWatchA.UI.Fragments
{
    public class StatisticFragment : Fragment
    {
        public static Fragment Instance { get; } = new StatisticFragment();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var chart = new ChartUpdater(view.FindViewById<SfChart>(Resource.Id.sfChart1));
            var chart1 = new ChartUpdater(view.FindViewById<SfChart>(Resource.Id.sfChart2));
            var chart2 = new ChartUpdater(view.FindViewById<SfChart>(Resource.Id.sfChart3));
            var data = new List<Record>();
            data.Add(new Record("Hallo", 10));
            data.Add(new Record("Schmallow", 12));
            data.Add(new Record("Blah", 20));
            chart.Data = data;
            data.Add(new Record("Tada", 20));
            chart1.Data = data;
            chart2.Data = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.statistics_fragment, container, false);
        }



    }
}
