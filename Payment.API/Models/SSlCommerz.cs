using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ePayment.API.Models
{
    public class SSlCommerz
    {
    }
    public class SslCommerzInitiateResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("GatewayPageURL")]
        public string GatewayPageUrl { get; set; } = string.Empty;
    }

    public class SslCommerzCallbackRequest
    {
        [FromForm(Name = "tran_id")]
        public string TranId { get; set; } = string.Empty;

        [FromForm(Name = "val_id")]
        public string ValId { get; set; } = string.Empty;

        [FromForm(Name = "status")]
        public string Status { get; set; } = string.Empty;

        [FromForm(Name = "amount")]
        public decimal Amount { get; set; }
    }
}
