using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selftastic_WS_Test.Models.Single
{
    public class Affirmation: GenericAPIModel
    {
        [JsonProperty("affirmation_id")]
        public string AffirmationId { get; set; }
        public override string Id => AffirmationId;
    }
}
