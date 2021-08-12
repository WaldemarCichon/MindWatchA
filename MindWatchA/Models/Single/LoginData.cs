using System;
namespace MindWatchA.Models.Single
{
    public class LoginData
    {
        public LoginData(String email, String password)
        {
            this.email = email;
            this.password = password;
        }

        public String email { get; set; }
        public String password { get; set; }
    }
}
