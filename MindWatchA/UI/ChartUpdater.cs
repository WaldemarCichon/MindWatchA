using System;
using System.Collections.Generic;
using Android.Widget;
using Com.Syncfusion.Charts;
using MindWidgetA.Tooling;

// https://help.syncfusion.com/xamarin/charts/datamarker
namespace MindWatchA.UI
{
    public struct Record
    {

        public Record(string label, int value)
        {
            Label = label;
            Value = value;
        }
        
        public int Value { get; set; }
        public string Label { get; set; }
    }

    public enum ChartType
    {
        QuestionChart,
        TaskChart,
        StateOfMindChart
    }

    public class ChartUpdater
    {
        private ChartType chartType;
        private SfChart chart;
        private ChartSeries series;
        private RadioGroup intervalRadioGroup;

        public ChartUpdater(SfChart chart, ChartType chartType)
        {
            this.chart = chart;
            series = new PieSeries();
            this.chartType = chartType;
        }

        public RadioGroup IntervalRadioGroup
        {
            get => intervalRadioGroup;
            set
            {
                intervalRadioGroup = value;
                intervalRadioGroup.CheckedChange += (sender, args) => changeSelection(args.CheckedId);
                
            }
        }



        private void changeSelection(int intervalKind)
        {
            var currentStatistics = Statistics.All[intervalKind];
            switch (chartType)
            {
                case ChartType.QuestionChart: fillYesNoChart(currentStatistics.QuestionCounter); break;
                case ChartType.TaskChart: fillYesNoChart(currentStatistics.TaskCounter); break;
                case ChartType.StateOfMindChart: fillTreeStateChart(currentStatistics.GoodBad); break;
            }
        }

        private void fillYesNoChart(Statistics.YesNoCounter questionCounter)
        {
            var data = new Record[2];
            data[0] = new Record("Positiv", questionCounter.Yes);
            data[1] = new Record("Negativ", questionCounter.No);
            // delete old data
            Data = data;
        }

        private void fillTreeStateChart(Statistics.GoodBadCounter goodBad)
        {
            var data = new Record[3];
            data[0] = new Record("Gut", goodBad.Good);
            data[1] = new Record("Neutral", goodBad.Neutral);
            data[2] = new Record("Schlecht", goodBad.Bad);
            Data = data;
        }

        private Record[] _data = new Record[0];
        public Record[] Data {
            get => _data;
            set
            {
                _data = value;
                series.ItemsSource = value;
                series.XBindingPath = "Label";
                series.Label = "Label";
                if (series.GetType() == typeof(PieSeries))
                {
                    ((PieSeries)series).YBindingPath = "Value";
                    ((PieSeries)series).DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended;
                    series.DataMarker.LabelContent = LabelContent.Percentage;
                    ((PieSeries)series).CircularCoefficient = 0.75;
                    ((PieSeries)series).ExplodableOnTouch = true;
                    
                }
                if (chart.Series.Count > 0)
                {
                    chart.Series.RemoveAt(0);
                }
                chart.Series.Add(series);
                chart.Legend.Visibility = Visibility.Visible;
                series.DataMarker.ShowLabel = true;
                series.DataMarker.ShowMarker = true;
                series.DataMarker.MarkerType = DataMarkerType.VerticalLine;
            }
        }
    }
}
