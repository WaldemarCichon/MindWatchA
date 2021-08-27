using System;
using System.Text.Json.Serialization;
namespace Selftastic_WS_Test.Models.Single
{
    public class Mood: GenericAPIModel
    {
        public static Mood Default = new Mood()
        {
            Id = "9d41b2e3-9dcc-4c55-8da0-e9c80e5d0641",
            Deleted = false,
            Text = "Wie geht es Dir heute?",
            UpdatedOn = DateTime.Now
        }; 
        public Mood()
        {    }
        [JsonPropertyName("mood_id")]
        public override string Id { get; set; }
    }
}
