using System.Text.Json.Serialization;

namespace Selftastic_WS_Test.Models.Single
{
    public class Affirmation: GenericAPIModel
    {
        [JsonPropertyName("affirmation_id")]
        public string AffirmationId { get; set; }
        public override string Id => AffirmationId;
    }
}
