using System;
using System.Text.Json.Serialization;

namespace Selftastic_WS_Test.Models.Single
{
    public abstract class GenericAPIModel
    {
        public bool Deleted { get; set; }
        public abstract string Id { get; set; }
        public virtual string Text { get; set; }
        [JsonIgnore]
        public string Title { get => Text.Split("§")[0]; }
        [JsonIgnore]
        public string Output { get => Title + "\n\n"; }
        [JsonIgnore]
        public string Description { get => Text.Contains("§") ? Text.Split("§")[1] : ""; }
        [JsonPropertyName("is_difficult")]
        public Boolean IsDifficult { get; set; }
        [JsonPropertyName("updated_on")]
        public DateTime UpdatedOn { get; set; }

        public override string ToString()
        {
            return Text+" ("+Id+")";
        }
    }
}
