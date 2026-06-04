using System.Text.Json.Serialization;

namespace ePayment.API.Dtos
{
    public class PaymentInitiateResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("failedreason")]
        public string FailedReason { get; set; } = string.Empty;

        [JsonPropertyName("sessionkey")]
        public string SessionKey { get; set; } = string.Empty;

        [JsonPropertyName("GatewayPageURL")]
        public string GatewayPageURL { get; set; } = string.Empty;

        [JsonPropertyName("storeBanner")]
        public string StoreBanner { get; set; } = string.Empty;

        [JsonPropertyName("StoreLogo")]
        public string StoreLogo { get; set; } = string.Empty;
    }
}
