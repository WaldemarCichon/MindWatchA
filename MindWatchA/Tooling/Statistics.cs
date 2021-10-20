using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using MindWidgetA.StateMachine;

namespace MindWidgetA.Tooling
{
    public class Statistics
    {
        public class GoodBadCounter
        {
            public int Good { get; set; }
            public int Neutral { get; set; }
            public int Bad { get; set; }
            public int Total { get
                {
                    return Good + Neutral + Bad;
                }
            }
        }

        public class YesNoCounter
        {
            public int Yes { get; set; }
            public int No { get; set; }
            public int Total
            {
                get
                {
                    return Yes + No;
                }
            }
        }

        public enum StatisticKind
        {
            Daily,
            Weekly,
            Monthly,
            Yearly,
            Global
        }

        public YesNoCounter TaskCounter { get; set; }
        public YesNoCounter QuestionCounter { get; set; }
        public GoodBadCounter GoodBad { get; set; }
        public DateTime StartDate { get; set; }
        public StatisticKind Kind { get; set; }
        public string InfoText
        {
            get
            {
                return "Statistik\n" +
                    "Fragen mit Ja beantwortet: " + QuestionCounter.Yes + "\n" +
                    "Fragen mit Nein beantwortet: " + QuestionCounter.No + "\n\n" +
                    "Aufgaben ausgeführt: " + TaskCounter.Yes + "\n" +
                    "Aufgaben nicht ausgeführt: " + TaskCounter.No + "\n" +
                    "Positive Einstellung: " + GoodBad.Good + '\n' +
                    "Neutrale Einstellung: " + GoodBad.Neutral + '\n' +
                    "Negative Einstellung: " + GoodBad.Bad;
            }
        }

        public static Statistics Daily = NewStatistics(StatisticKind.Daily);
        public static Statistics Monthly = NewStatistics(StatisticKind.Monthly);
        public static Statistics Weekly = NewStatistics(StatisticKind.Weekly);
        public static Statistics Yearly = NewStatistics(StatisticKind.Yearly);
        public static Statistics Global = NewStatistics(StatisticKind.Global);
        public static Statistics[] All = new Statistics[]  {
            Daily,
            Weekly,
            Monthly,
            Yearly,
            Global
        };

        private static Statistics NewStatistics(StatisticKind kind)
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), kind.ToString() + "Statistics.json");
            if (File.Exists(path))
            {
                using FileStream stream = File.OpenRead(path);
                var bytes = new byte[stream.Length];
                stream.Read(bytes);
                var content = Encoding.ASCII.GetString(bytes);
                var statistics = JsonSerializer.Deserialize<Statistics>(content);
                statistics.checkForDateSwitch();
                return statistics;
            }
            else
            {
                return new Statistics() { Kind = kind };
            }
        }

        public Statistics()
        {
            init();
        }

        private void init()
        {
            TaskCounter = new YesNoCounter();
            QuestionCounter = new YesNoCounter();
            GoodBad = new GoodBadCounter();
            StartDate = DateTime.Today;
        }

        private void persist()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Kind.ToString()+"Statistics.json");
            using FileStream fileStream = File.Create(path);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }

        private void checkForDateSwitch()
        {
            var mustInit = false;
            switch (this.Kind)
            {
                case StatisticKind.Daily: mustInit = StartDate.Day != DateTime.Now.Day; break;
                case StatisticKind.Weekly: mustInit = CultureInfo.GetCultureInfo("de-DE").Calendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) != CultureInfo.GetCultureInfo("de-DE").Calendar.GetWeekOfYear(StartDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday); break;
                case StatisticKind.Monthly: mustInit = StartDate.Month != DateTime.Now.Month; break;
                case StatisticKind.Yearly: mustInit = StartDate.Year != DateTime.Now.Year; break;
            }

            if (mustInit)
            {
                init();
            }

        }

        public static void Persist()
        {
            Daily.persist();
            Weekly.persist();
            Monthly.persist();
            Yearly.persist();
            Global.persist();
        }

        private void incrementTask(bool which)
        {
            checkForDateSwitch();
            if (which)
            {
                TaskCounter.Yes++;
            } else
            {
                TaskCounter.No++;
            }
        }

        private void incrementQuestion(bool which)
        {
            checkForDateSwitch();
            if (which)
            {
                QuestionCounter.Yes++;
            } else
            {
                QuestionCounter.No++;
            }
        }

        private void incrementMindState(Events _event)
        {
            checkForDateSwitch();
            switch (_event)
            {
                case Events.HappyButtonPressed: GoodBad.Good++; break;
                case Events.NeutralButtonPressed: GoodBad.Neutral++; break;
                case Events.SadButtonPressed: GoodBad.Bad++; break;
            }

        }

        public static void IncrementTask(bool which)
        {
            Daily.incrementTask(which);
            Weekly.incrementTask(which);
            Monthly.incrementTask(which);
            Yearly.incrementTask(which);
            Global.incrementTask(which);
            Persist();
        }

        public static void IncrementQuestion(bool which)
        {
            Daily.incrementQuestion(which);
            Weekly.incrementQuestion(which);
            Monthly.incrementQuestion(which);
            Yearly.incrementQuestion(which);
            Global.incrementQuestion(which);
            Persist();
        }

        public static void IncrementMindState(Events _event)
        {
            Daily.incrementMindState(_event);
            Weekly.incrementMindState(_event);
            Monthly.incrementMindState(_event);
            Yearly.incrementMindState(_event);
            Global.incrementMindState(_event);
            Persist();
        }


    }
}
