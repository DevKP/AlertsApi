using System.Text.Json.Serialization;

namespace AlertsApi.Api.Models.Responses
{
    public class AlertResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("location_title")]
        public string? LocationTitle { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("ended_at")]
        public DateTime? EndedAt { get; set; }

        [JsonPropertyName("duration")]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}