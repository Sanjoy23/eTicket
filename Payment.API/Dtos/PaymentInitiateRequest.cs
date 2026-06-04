using System.Text.Json.Serialization;

namespace ePayment.API.Dtos
{
    public class PaymentInitiateRequest
    {
        [JsonPropertyName("store_id")]
        public string? StoreId { get; set; }

        [JsonPropertyName("store_passwd")]
        public string? StorePassword { get; set; }

        // Transaction
        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "BDT";

        [JsonPropertyName("receipt_id")]
        public string? ReceiptId { get; set; }

        [JsonPropertyName("tran_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("product_category")]
        public string? ProductCategory { get; set; }

        // Redirect URLs
        [JsonPropertyName("success_url")]
        public string? SuccessUrl { get; set; }

        [JsonPropertyName("fail_url")]
        public string? FailUrl { get; set; }

        [JsonPropertyName("cancel_url")]
        public string? CancelUrl { get; set; }

        [JsonPropertyName("ipn_url")]
        public string? IpnUrl { get; set; }

        // Card Options
        [JsonPropertyName("multi_card_name")]
        public string? MultiCardName { get; set; }

        [JsonPropertyName("allowed_bin")]
        public string? AllowedBin { get; set; }

        // EMI Options
        [JsonPropertyName("emi_option")]
        public int EmiOption { get; set; }

        [JsonPropertyName("emi_max_inst_option")]
        public int? EmiMaxInstOption { get; set; }

        [JsonPropertyName("emi_selected_inst")]
        public int? EmiSelectedInst { get; set; }

        [JsonPropertyName("emi_allow_only")]
        public int? EmiAllowOnly { get; set; }

        // Customer Info
        [JsonPropertyName("cus_name")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("cus_email")]
        public string? CustomerEmail { get; set; }

        [JsonPropertyName("cus_add1")]
        public string? CustomerAddress1 { get; set; }

        [JsonPropertyName("cus_add2")]
        public string? CustomerAddress2 { get; set; }

        [JsonPropertyName("cus_city")]
        public string? CustomerCity { get; set; }

        [JsonPropertyName("cus_state")]
        public string? CustomerState { get; set; }

        [JsonPropertyName("cus_postcode")]
        public string? CustomerPostcode { get; set; }

        [JsonPropertyName("cus_country")]
        public string? CustomerCountry { get; set; }

        [JsonPropertyName("cus_phone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("cus_fax")]
        public string? CustomerFax { get; set; }

        // Shipping Info
        [JsonPropertyName("shipping_method")]
        public string? ShippingMethod { get; set; }

        [JsonPropertyName("num_of_item")]
        public int NumberOfItems { get; set; }

        [JsonPropertyName("ship_name")]
        public string? ShipName { get; set; }

        [JsonPropertyName("ship_add1")]
        public string? ShipAddress1 { get; set; }

        [JsonPropertyName("ship_add2")]
        public string? ShipAddress2 { get; set; }

        [JsonPropertyName("ship_city")]
        public string? ShipCity { get; set; }

        [JsonPropertyName("ship_state")]
        public string? ShipState { get; set; }

        [JsonPropertyName("ship_postcode")]
        public string? ShipPostcode { get; set; }

        [JsonPropertyName("ship_country")]
        public string? ShipCountry { get; set; }

        // Product Info
        [JsonPropertyName("product_name")]
        public string? ProductName { get; set; }

        [JsonPropertyName("product_profile")]
        public string? ProductProfile { get; set; }

        // Travel / Flight
        [JsonPropertyName("hours_till_departure")]
        public string? HoursTillDeparture { get; set; }

        [JsonPropertyName("flight_type")]
        public string? FlightType { get; set; }

        [JsonPropertyName("pnr")]
        public string? Pnr { get; set; }

        [JsonPropertyName("journey_from_to")]
        public string? JourneyFromTo { get; set; }

        [JsonPropertyName("third_party_booking")]
        public string? ThirdPartyBooking { get; set; }

        // Hotel
        [JsonPropertyName("hotel_name")]
        public string? HotelName { get; set; }

        [JsonPropertyName("length_of_stay")]
        public string? LengthOfStay { get; set; }

        [JsonPropertyName("check_in_time")]
        public string? CheckInTime { get; set; }

        [JsonPropertyName("hotel_city")]
        public string? HotelCity { get; set; }

        // Top-up
        [JsonPropertyName("product_type")]
        public string? ProductType { get; set; }

        [JsonPropertyName("topup_number")]
        public string? TopupNumber { get; set; }

        [JsonPropertyName("country_topup")]
        public string? CountryTopup { get; set; }

        // Pricing Breakdown
        [JsonPropertyName("cart")]
        public string? Cart { get; set; }

        [JsonPropertyName("product_amount")]
        public decimal? ProductAmount { get; set; }

        [JsonPropertyName("vat")]
        public decimal? Vat { get; set; }

        [JsonPropertyName("discount_amount")]
        public decimal? DiscountAmount { get; set; }

        [JsonPropertyName("convenience_fee")]
        public decimal? ConvenienceFee { get; set; }
    }
}
