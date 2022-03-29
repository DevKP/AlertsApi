using System.Text.Json.Serialization;

namespace AlertsApi.Api.Models.Responses
{
    public class MetaResponse
    {
        [JsonPropertyName("last_updated_at")]
        public DateTime LastUpdatedAt { get; set; }

        [JsonPropertyName("generated_at")]
        public string? GeneratedAt { get; set; }
    }
}