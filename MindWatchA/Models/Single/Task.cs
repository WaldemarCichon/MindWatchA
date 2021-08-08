using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class Task: GenericAPIModel
    {
        [JsonProperty("task_id")]
        public string TaskId { get; set; }
        public override string Id => TaskId;
    }
}
