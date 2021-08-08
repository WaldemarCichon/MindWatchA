using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public abstract class GenericAPIModel
    {
        public bool Deleted { get; set; }
        public abstract string Id { get; }
        public string Text { get; set; }
        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }

        public override string ToString()
        {
            return Text+" ("+Id+")";
        }
    }
}
