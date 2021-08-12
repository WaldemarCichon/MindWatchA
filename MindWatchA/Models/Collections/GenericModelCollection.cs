using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Enums;
using Selftastic_WS_Test.Models.Single;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Collections
{
    public class GenericModelCollection <T> where T : GenericAPIModel
    {
        public DateTime LastUpdated { get; set; }
        public List<T> positions { get; set; }
        public List<T> available { get; set; }
        Random random;
        public GenericModelCollection()
        {
            this.positions = new List<T>();
            this.available = new List<T>();
            random = new Random();
        }

        public GenericModelCollection(IEnumerable<T> positions)
        {
            this.positions = positions.ToList();
            this.available = positions.ToList();
            random = new Random();
        }

        [JsonIgnore]
        public T NewRandom
        {
            get
            {
                var count = available.Count;
                if (count == 0)
                {
                    available.AddRange(positions);
                }
                var id = random.Next(available.Count);
                var position = available[id];
                available.RemoveAt(id);
                Last = position;
                return position;
            }
        }

        public T Last { get; set; }

        public static C CreateInstance<C>() where C: GenericModelCollection<T>
        {
            C newInstance;
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), typeof(C).Name.ToLower() + ".json");
            if (File.Exists(path))
            {
                Console.WriteLine(typeof(C).Name + " loaded from file system");
                using FileStream stream = File.OpenRead(path);
                var bytes = new byte[stream.Length];
                stream.Read(bytes);
                var content = Encoding.ASCII.GetString(bytes);
                newInstance = JsonSerializer.Deserialize<C>(content);
                return newInstance;
            }
            Console.WriteLine(typeof(C).Name + " loaded from web service");
            var properties = typeof(C).GetProperties();
            var property = typeof(C).GetProperty("InstanceFromWebservice");
            newInstance = (C)property.GetValue(null);
            newInstance.Persist();
            return newInstance;
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

        public void SendAnswer(AnswerKind answer)
        {
            Thread t = new Thread(
                async () => await ApiCall.Instance.SendAnswer(Last, answer));
            t.Start();
        }
    }
}
