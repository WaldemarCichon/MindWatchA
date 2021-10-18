using System;
using System.Collections.Generic;
using Com.Syncfusion.Charts;

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

    public class ChartUpdater
    {
        private SfChart chart;
        private ChartSeries series;
        public ChartUpdater(SfChart chart)
        {
            this.chart = chart;
            series = new PieSeries();
        }

        private List<Record> _data = new List<Record>();
        public List<Record> Data {
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
