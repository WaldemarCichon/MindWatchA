using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MindWatchA.Enums;

namespace Selftastic_WS_Test.Models.Single
{
    public class User
    {
        private static User instance;

        public string user_id { get; set; }
        public string first_name { get; set; } = "New User";
        public string last_name { get; set; } = "New created";
        public string email { get; set; } = "empty";
        public string password { get; set; } = "abcd";
        public bool newsletter { get; set; } = false;
        // public Gender gender { get; set; } =
        public string gender { get; set; }
        public DateTime registered_on { get; set; }
        public DateTime updated_on { get; set; }
        public Boolean test_mode { get; set; } = true;
        public DateTime birthdate { get; set; } = DateTime.Now;
        public DateTime accepted_gdpr { get; set; } = DateTime.Now;
        public DateTime accepted_tac { get; set; } = DateTime.Now;
        public bool fresh { get; set; }

        public User()
        {

        }

        public static User Instance
        {
            get
            {
                if (instance == null)
                {
                    createUser();
                }
                return instance;
            }
        }

        private static void createUser()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "user.json");
            if (File.Exists(path))
            {
                using FileStream stream = File.OpenRead(path);
                var bytes = new byte[stream.Length];
                stream.Read(bytes);
                var content = Encoding.ASCII.GetString(bytes);
                instance = JsonSerializer.Deserialize<User>(content);
            }
            else
            {
                instance = new User();
                instance.fresh = true;
            }
        }

        public void Persist()
        {
            fresh = false;
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "user.json");
            using FileStream fileStream = File.Create(path);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }

        public static void Remove()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "user.json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public override string ToString()
        {
            return $"{user_id}: {email} => {first_name} {last_name}";
        }

        internal User Reload()
        {
            createUser();
            return instance;
        }
    }


}
