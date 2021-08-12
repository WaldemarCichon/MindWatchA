using System.Text.Json.Serialization;

namespace Selftastic_WS_Test.Models.Single
{
    public class Task: GenericAPIModel
    {
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }
        public override string Id => TaskId;
    }
}
