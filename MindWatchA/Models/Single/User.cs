﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class User
    {
        private static User instance;

        public string user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime registered_on { get; set; }
        public DateTime updated_on { get; set; }

        private User()
        {

        }

        public static User Instance
        {
            get
            {
                if (instance == null)
                {
                    string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "user.json");
                    if (File.Exists(path))
                    {
                        using FileStream stream = File.OpenRead(path);
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes);
                        var content = Encoding.ASCII.GetString(bytes);
                        instance = JsonSerializer.Deserialize<User>(content);
                    } else
                    {
                        instance = new User();
                    }
                }
                return instance;
            }
        }

        public void Persist()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "user.json");
            using FileStream fileStream = File.Create(path);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }
    }


}
