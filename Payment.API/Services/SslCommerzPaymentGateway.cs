using ePayment.API.Dtos;
using ePayment.API.Models;
using Microsoft.Extensions.Options;

namespace ePayment.API.Services
{
    public class SslCommerzPaymentGateway(HttpClient httpClient, IOptions<SslCommerzOptions> options) : IPaymentGateway
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly SslCommerzOptions _options = options.Value;

        public async Task<PaymentInitiateResult> InitiateAsync(PaymentInitiateRequest request, CancellationToken cancellationToken)
        {
            var transactionId = $"ETK-{Guid.NewGuid():N}";
            var baseUrl = GetBaseUrl();
            var formData = new Dictionary<string, string>
            {
                ["store_id"] = _options.StoreId,
                ["store_passwd"] = _options.StorePassword,
                ["total_amount"] = request.TotalAmount.ToString("F2"),
                ["currency"] = request.Currency,
                ["tran_id"] = transactionId,

                ["success_url"] = _options.SuccessUrl,
                ["fail_url"] = _options.FailUrl,
                ["cancel_url"] = _options.CancelUrl,
                ["ipn_url"] = _options.IpnUrl,

                ["cus_name"] = request.CustomerName ?? "Test User",
                ["cus_email"] = request.CustomerEmail ?? "test@example.com",
                ["cus_add1"] = request.CustomerAddress1 ?? "Dhaka",
                ["cus_city"] = request.CustomerCity ?? "Dhaka",
                ["cus_country"] = request.CustomerCountry ?? "Bangladesh",
                ["cus_phone"] = request.CustomerPhone ?? "01700000000",

                ["shipping_method"] = "NO",
                ["product_name"] = "E-Ticket",
                ["product_category"] = "Ticket",
                ["product_profile"] = "general"
            };
            var response = await _httpClient
                .PostAsync($"{baseUrl}/gwprocess/v4/api.php",
                                            new FormUrlEncodedContent(formData),
                                            cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<PaymentInitiateResult>(
                cancellationToken);

            if (json is null || json.Status != "SUCCESS")
                throw new Exception("SSLCommerz payment initiation failed.");

            return new PaymentInitiateResult
            {
                Status = json.Status,
                FailedReason = json.FailedReason,
                SessionKey = json.SessionKey,
                GatewayPageURL = json.GatewayPageURL,
                StoreBanner = json.StoreBanner,
                StoreLogo = json.StoreLogo
            };
        }

        public async Task<PaymentVerifyResult> VerifyAsync(string transactionId, CancellationToken cancellationToken)
        {
            var baseUrl = GetBaseUrl();
            var url =
            $"{baseUrl}/validator/api/merchantTransIDvalidationAPI.php" +
            $"?tran_id={transactionId}" +
            $"&store_id={_options.StoreId}" +
            $"&store_passwd={_options.StorePassword}" +
            $"&format=json";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            return new PaymentVerifyResult
            {
                IsSuccess = rawResponse.Contains("VALID", StringComparison.OrdinalIgnoreCase),
                TransactionId = transactionId,
                RawResponse = rawResponse
            };
        }

        private string GetBaseUrl()
        {
            if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                return _options.BaseUrl.TrimEnd('/');
            }

            return _options.IsLive
                ? "https://securepay.sslcommerz.com"
                : "https://sandbox.sslcommerz.com";
        }
    }
}
