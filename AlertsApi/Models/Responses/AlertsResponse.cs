using System.Text.Json.Serialization;

namespace AlertsApi.Api.Models.Responses
{
    public class AlertsResponse
    {
        [JsonPropertyName("alerts")]
        public List<AlertResponse>? Alerts { get; set; }
    }
}