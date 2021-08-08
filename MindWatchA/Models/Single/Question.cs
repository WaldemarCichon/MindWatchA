using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class Question: GenericAPIModel
    {
        [JsonProperty("question_id")]
        public string QuestionId { get; set; }
        public override string Id => QuestionId;
    }
}
