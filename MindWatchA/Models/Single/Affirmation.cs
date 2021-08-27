using System.Text.Json.Serialization;

namespace Selftastic_WS_Test.Models.Single
{
    public class Affirmation: GenericAPIModel
    {
        [JsonPropertyName("affirmation_id")]
        public override string Id { get; set; }
    }
}
