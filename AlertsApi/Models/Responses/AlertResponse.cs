using System.Text.Json.Serialization;

namespace AlertsApi.Api.Models.Responses
{
    public class AlertResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("location_title")]
        public string? LocationTitle { get; set; }

        [JsonPropertyName("location_type")]
        public string? LocationType { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("finished_at")]
        public DateTime? FinishedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("calculated")]
        public object? Calculated { get; set; }
    }
}