using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class User
    {
        string user_id { get; set; }
        string name { get; set; }
        string email { get; set; }
        string password { get; set; }
        DateTime registered_on { get; set; }
        DateTime updated_on { get; set; } 
    }
}
