using ePayment.API.Dtos;
using ePayment.API.Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace ePayment.API.Services
{
    public class SslCommerzPaymentGateway(HttpClient httpClient, IOptions<SslCommerzOptions> options) : IPaymentGateway
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly SslCommerzOptions _options = options.Value;

        public async Task<PaymentInitiateResult> InitiateAsync(PaymentInitiateRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TransactionId))
            {
                throw new ArgumentException("TransactionId is required.", nameof(request));
            }

            var baseUrl = GetBaseUrl();
            var formData = new Dictionary<string, string>
            {
                ["store_id"] = _options.StoreId,
                ["store_passwd"] = _options.StorePassword,
                ["total_amount"] = request.TotalAmount.ToString("F2", CultureInfo.InvariantCulture),
                ["currency"] = request.Currency,
                ["tran_id"] = request.TransactionId,

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
                ["product_name"] = request.ProductName ?? "E-Ticket",
                ["product_category"] = request.ProductCategory ?? "Ticket",
                ["product_profile"] = request.ProductProfile ?? "general"
            };

            var response = await _httpClient
                .PostAsync($"{baseUrl}/gwprocess/v4/api.php", new FormUrlEncodedContent(formData), cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<PaymentInitiateResult>(cancellationToken);

            if (json is null || !string.Equals(json.Status, "SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(json?.FailedReason ?? "SSLCommerz payment initiation failed.");
            }

            return new PaymentInitiateResult
            {
                Status = json.Status,
                FailedReason = json.FailedReason,
                SessionKey = json.SessionKey,
                GatewayPageURL = json.GatewayPageURL,
                PaymentUrl = json.GatewayPageURL,
                TransactionId = request.TransactionId,
                StoreBanner = json.StoreBanner,
                StoreLogo = json.StoreLogo
            };
        }

        public async Task<PaymentVerifyResult> VerifyAsync(string transactionId, CancellationToken cancellationToken)
        {
            var baseUrl = GetBaseUrl();
            var url =
                $"{baseUrl}/validator/api/merchantTransIDvalidationAPI.php" +
                $"?tran_id={WebUtility.UrlEncode(transactionId)}" +
                $"&store_id={WebUtility.UrlEncode(_options.StoreId)}" +
                $"&store_passwd={WebUtility.UrlEncode(_options.StorePassword)}" +
                "&format=json";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var status = TryReadString(rawResponse, "status");

            return new PaymentVerifyResult
            {
                IsSuccess = string.Equals(status, "VALID", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(status, "VALIDATED", StringComparison.OrdinalIgnoreCase) ||
                    rawResponse.Contains("\"VALID\"", StringComparison.OrdinalIgnoreCase),
                TransactionId = transactionId,
                RawResponse = rawResponse,
                Status = status,
                ValidationId = TryReadString(rawResponse, "val_id"),
                Amount = TryReadDecimal(rawResponse, "amount")
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

        private static string TryReadString(string json, string propertyName)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                return FindProperty(document.RootElement, propertyName)?.GetString() ?? string.Empty;
            }
            catch (JsonException)
            {
                return string.Empty;
            }
        }

        private static decimal TryReadDecimal(string json, string propertyName)
        {
            try
            {
                using var document = JsonDocument.Parse(json);
                var property = FindProperty(document.RootElement, propertyName);
                if (property is null)
                {
                    return 0;
                }

                if (property.Value.ValueKind == JsonValueKind.Number &&
                    property.Value.TryGetDecimal(out var number))
                {
                    return number;
                }

                if (property.Value.ValueKind == JsonValueKind.String &&
                    decimal.TryParse(property.Value.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var textNumber))
                {
                    return textNumber;
                }
            }
            catch (JsonException)
            {
                return 0;
            }

            return 0;
        }

        private static JsonElement? FindProperty(JsonElement element, string propertyName) =>
        element.ValueKind switch
        {
            JsonValueKind.Object => FindPropertyInObject(element, propertyName),
            JsonValueKind.Array => FindPropertyInArray(element, propertyName),
            _ => null
        };

        private static JsonElement? FindPropertyInObject(JsonElement element, string propertyName)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value;
                }

                var nested = FindProperty(property.Value, propertyName);
                if (nested is not null)
                {
                    return nested;
                }
            }

            return null;
        }

        private static JsonElement? FindPropertyInArray(JsonElement element, string propertyName)
        {
            foreach (var item in element.EnumerateArray())
            {
                var nested = FindProperty(item, propertyName);
                if (nested is not null)
                {
                    return nested;
                }
            }

            return null;
        }
    }
}
