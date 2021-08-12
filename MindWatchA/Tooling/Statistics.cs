using System;
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

        public YesNoCounter TaskCounter { get; set; }
        public YesNoCounter QuestionCounter { get; set; }
        public GoodBadCounter GoodBad { get; set; }
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

        private static Statistics instance;

        public static Statistics Instance
        {
            get
            {
                if (instance == null)
                {
                    string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "statistics.json");
                    if (File.Exists(path))
                    {
                        using FileStream stream = File.OpenRead(path);
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes);
                        var content = Encoding.ASCII.GetString(bytes);
                        instance = JsonSerializer.Deserialize<Statistics>(content);
                    }
                    else
                    {
                        instance = new Statistics();
                    }
                }
                return instance;
            }
        }

        public Statistics()
        {
            TaskCounter = new YesNoCounter();
            QuestionCounter = new YesNoCounter();
            GoodBad = new GoodBadCounter();
        }

        public void Persist()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "statistics.json");
            using FileStream fileStream = File.Create(path);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }

        public void IncrementTask(bool which)
        {
            if (which)
            {
                TaskCounter.Yes++;
            } else
            {
                TaskCounter.No++;
            }
        }

        public void IncrementQuestion(bool which)
        {
            if (which)
            {
                QuestionCounter.Yes++;
            } else
            {
                QuestionCounter.No++;
            }
        }

        public void IncrementMindState(Events _event)
        {
            switch (_event) {
                case Events.HappyButtonPressed: GoodBad.Good++;break;
                case Events.NeutralButtonPressed: GoodBad.Neutral++; break;
                case Events.SadButtonPressed: GoodBad.Good++; break;
            }
        }
    }
}
