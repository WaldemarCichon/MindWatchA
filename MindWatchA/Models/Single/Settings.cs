using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MindWatchA.Models.Single
{
    public class Settings
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednsday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public bool GetWeekDayValue(int index)
        {
            switch (index)
            {
                case 0: return Monday;
                case 1: return Tuesday;
                case 2: return Wednsday;
                case 3: return Thursday;
                case 4: return Friday;
                case 5: return Saturday;
                case 6: return Sunday;
                default: throw new Exception("Not a weekday index " + index);
            }
        }

        public void SetWeekDayValue (int index, bool value)
        {
            switch (index)
            {
                case 0: Monday = value; break;
                case 1: Tuesday = value; break; 
                case 2: Wednsday = value; break; 
                case 3: Thursday = value; break;
                case 4: Friday = value; break;
                case 5: Saturday = value; break;
                case 6: Sunday = value; break;
                default: throw new Exception("Not a weekday index " + index);
            }
        }

        public Settings()
        {
        }

        public static Settings Restore()
        {
            string path = typeof(Settings).Name.ToLower() + ".json";
            var prefix = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullPath = Path.Combine(prefix, path);
            if (File.Exists(fullPath))
            {
                try
                {
                    var content = File.ReadAllText(fullPath);
                    var settings = JsonSerializer.Deserialize<Settings>(content);
                    return settings;
                } catch (Exception)
                {

                }
            }
            return new Settings {
                StartTime = new DateTime(8L * 60 * 60 * 10000000L),
                EndTime = new DateTime(20L * 60 * 60 * 10000000L),
                Monday = true,
                Tuesday = true,
                Wednsday = true,
                Thursday = true,
                Friday = true,
                
            };
        } 

        public void Persist()
        {
            string path = this.GetType().Name.ToLower() + ".json";
            var prefix = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullPath = Path.Combine(prefix, path);
            using FileStream fileStream = File.Create(fullPath);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }
    }
}
