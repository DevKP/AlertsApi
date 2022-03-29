using System.Text.Json.Serialization;

namespace AlertsApi.TgAlertsFramework.Models
{
    public class TgAlert
    {
        public bool Active { get; set; }

        [JsonPropertyName("location_title")]
        public string? LocationTitle { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime FetchedAt { get; set; }
    }
}