using System.Text.Json.Serialization;

namespace Selftastic_WS_Test.Models.Single
{
    public class Task: GenericAPIModel
    {
        [JsonPropertyName("task_id")]
        public override string Id { get; set; }
    }
}
