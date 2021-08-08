using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.API
{
    public class Authentification
    {
        public Authentification(string user, string password)
        {
            this.User = user;
            this.Password = password;
        }

        public string User { get; }
        public string Password { get; }
        private byte[] authBytes => Encoding.ASCII.GetBytes(User + ":" + Password);
        public string Base64String => Convert.ToBase64String(authBytes);
        public string AuthPropertyString => "Basic " + Base64String;

    }
}
