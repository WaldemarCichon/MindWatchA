using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Collections
{
    public class GenericModelCollection <T> where T : GenericAPIModel
    {
        DateTime LastUpdated { get; set; }
        List<T> positions;
        List<T> available;
        Random random;
        public GenericModelCollection(IEnumerable<T> positions)
        {
            this.positions = positions.ToList();
            this.available = positions.ToList();
            random = new Random();
        }

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
                return position;
            }
        }

        public static C CreateInstance<C>()
        {
            C newInstance;
            string path = Path.Combine(System.Environment.SpecialFolder.MyDocuments.ToString(), MethodBase.GetCurrentMethod().DeclaringType.Name, ".json");
            if (File.Exists(path))
            {
                using FileStream stream = File.OpenRead(path);
                newInstance = JsonSerializer.DeserializeAsync<C>(stream).Result;
                return newInstance;
            }
            newInstance = (C)typeof(C).GetProperty("InstanceFromWebservice").GetValue(null);
            return newInstance;
        } 

        public void Persist()
        {
            string path = this.GetType().Name.ToLower() + ".json";
            using FileStream fileStream = File.Create(path);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }
    }
}
