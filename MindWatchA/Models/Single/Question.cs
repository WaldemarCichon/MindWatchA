using System.Text.Json.Serialization;


namespace Selftastic_WS_Test.Models.Single
{
    public class Question: GenericAPIModel
    {
        [JsonPropertyName("question_id")]
        public string QuestionId { get; set; }
        public override string Id => QuestionId;
    }
}
