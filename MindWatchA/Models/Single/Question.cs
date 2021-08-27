using System.Text.Json.Serialization;


namespace Selftastic_WS_Test.Models.Single
{
    public class Question: GenericAPIModel
    {
        [JsonPropertyName("question_id")]
        public override string Id { get; set; }
    }
}
