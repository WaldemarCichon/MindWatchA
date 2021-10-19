
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
            var questionChart = new ChartUpdater(view.FindViewById<SfChart>(Resource.Id.sfChart1), ChartType.QuestionChart);
            var taskChart = new ChartUpdater(view.FindViewById<SfChart>(Resource.Id.sfChart2), ChartType.TaskChart);
            var stateOfMindChart = new ChartUpdater(view.FindViewById<SfChart>(Resource.Id.sfChart3), ChartType.StateOfMindChart);
            var intervalRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.interval_radio_group);
            questionChart.IntervalRadioGroup = intervalRadioGroup;
            taskChart.IntervalRadioGroup = intervalRadioGroup;
            stateOfMindChart.IntervalRadioGroup = intervalRadioGroup;
            (intervalRadioGroup.GetChildAt(0) as RadioButton).Checked = true;
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.statistics_fragment, container, false);
        }



    }
}
