using ePayment.API.Dtos;
using ePayment.API.Infrastructure.Data;
using ePayment.API.Models;
using ePayment.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ePayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(
        IPaymentGateway paymentGateway,
        PaymentDbContext dbContext,
        ILogger<PaymentsController> logger) : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway = paymentGateway;
        private readonly PaymentDbContext _dbContext = dbContext;
        private readonly ILogger<PaymentsController> _logger = logger;

        [HttpPost("initiate")]
        [HttpPost("InitiatePayment")]
        public async Task<ActionResult<PaymentInitiateResult>> InitiatePaymentLink(
            PaymentInfoDto paymentInfo,
            CancellationToken cancellationToken)
        {
            if (paymentInfo.BookingId == Guid.Empty)
            {
                return BadRequest("BookingId is required.");
            }

            if (paymentInfo.UserId == Guid.Empty)
            {
                return BadRequest("UserId is required.");
            }

            if (paymentInfo.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            var payment = new PaymentEntity
            {
                PaymentId = Guid.NewGuid(),
                BookingId = paymentInfo.BookingId,
                UserId = paymentInfo.UserId,
                ReceiptId = paymentInfo.Id,
                ReceiptNumber = paymentInfo.ReceiptNumber,
                Amount = paymentInfo.Amount,
                Currency = paymentInfo.Currency,
                TransactionId = $"ETK-{Guid.NewGuid():N}",
                Status = PaymentStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            try
            {
                var gatewayRequest = new PaymentInitiateRequest
                {
                    TotalAmount = paymentInfo.Amount,
                    Currency = paymentInfo.Currency,
                    ReceiptId = paymentInfo.Id.ToString(),
                    TransactionId = payment.TransactionId,
                    CustomerCity = paymentInfo.City,
                    CustomerCountry = paymentInfo.Country,
                    CustomerEmail = paymentInfo.Email,
                    CustomerPhone = paymentInfo.Phone,
                    CustomerName = paymentInfo.FullName,
                    CustomerAddress1 = paymentInfo.Address,
                    ProductCategory = paymentInfo.ProductType,
                    ProductProfile = paymentInfo.ProductProfile,
                };

                var result = await _paymentGateway.InitiateAsync(gatewayRequest, cancellationToken);

                payment.SessionKey = result.SessionKey;
                payment.GatewayPaymentUrl = result.GatewayPageURL;
                payment.RawGatewayResponse = JsonSerializer.Serialize(result);
                payment.UpdatedAtUtc = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);

                result.PaymentId = payment.PaymentId;
                result.BookingId = payment.BookingId;
                result.TransactionId = payment.TransactionId;
                result.PaymentUrl = result.GatewayPageURL;

                return Ok(result);
            }
            catch (Exception exception)
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = exception.Message;
                payment.UpdatedAtUtc = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(CancellationToken.None);

                _logger.LogError(
                    exception,
                    "Payment initiation failed for BookingId {BookingId}, PaymentId {PaymentId}",
                    payment.BookingId,
                    payment.PaymentId);

                return Problem(
                     title: "Payment initiation failed",
                     detail: "The payment gateway could not be reached or rejected the request.",
                     statusCode: StatusCodes.Status502BadGateway);
            }
        }

        [HttpPost("{paymentId:guid}/verify")]
        public async Task<ActionResult<PaymentVerifyResult>> VerifyPayment(
            Guid paymentId,
            CancellationToken cancellationToken)
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(entity => entity.PaymentId == paymentId, cancellationToken);

            if (payment is null)
            {
                return NotFound("Payment not found.");
            }

            var result = await VerifyAndUpdatePaymentAsync(payment, cancellationToken);
            return Ok(result);
        }

        [HttpPost("sslcommerz/success")]
        public async Task<IActionResult> Success(CancellationToken cancellationToken)
        {
            return await ProcessSslCommerzCallbackAsync(SslCommerzCallbackKind.SuccessRedirect, cancellationToken);
        }

        [HttpPost("sslcommerz/fail")]
        public async Task<IActionResult> Fail(CancellationToken cancellationToken)
        {
            return await ProcessSslCommerzCallbackAsync(SslCommerzCallbackKind.FailRedirect, cancellationToken);
        }

        [HttpPost("sslcommerz/cancel")]
        public async Task<IActionResult> Cancel(CancellationToken cancellationToken)
        {
            return await ProcessSslCommerzCallbackAsync(SslCommerzCallbackKind.CancelRedirect, cancellationToken);
        }

        [HttpPost("sslcommerz/ipn")]
        public async Task<IActionResult> Ipn(CancellationToken cancellationToken)
        {
            var result = await HandleSslCommerzCallbackAsync(
                fallbackStatus: null,
                callbackSource: "ipn",
                cancellationToken);
            return Ok(result);
        }

        private async Task<IActionResult> ProcessSslCommerzCallbackAsync(
            SslCommerzCallbackKind callbackKind,
            CancellationToken cancellationToken)
        {
            PaymentStatus? fallbackStatus = callbackKind switch
            {
                SslCommerzCallbackKind.FailRedirect => PaymentStatus.Failed,
                SslCommerzCallbackKind.CancelRedirect => PaymentStatus.Cancelled,
                _ => null
            };

            var result = await HandleSslCommerzCallbackAsync(
                fallbackStatus,
                callbackKind.ToString(),
                cancellationToken);

            return Ok(result);
        }

        private async Task<PaymentVerifyResult> HandleSslCommerzCallbackAsync(
            PaymentStatus? fallbackStatus,
            string callbackSource,
            CancellationToken cancellationToken)
        {
            var form = await Request.ReadFormAsync(cancellationToken);
            var transactionId = form["tran_id"].ToString();
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new InvalidOperationException("SSLCommerz callback does not contain tran_id.");
            }

            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(entity => entity.TransactionId == transactionId, cancellationToken)
                ?? throw new KeyNotFoundException($"Payment transaction {transactionId} was not found.");

            payment.RawGatewayResponse = JsonSerializer.Serialize(form.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToString()));
            payment.ValidationId = form["val_id"].ToString();
            payment.BankTransactionId = form["bank_tran_id"].ToString();
            _logger.LogInformation(
                "SSLCommerz {CallbackSource} callback received for TransactionId {TransactionId}",
                callbackSource,
                transactionId);

            var gatewayStatus = form["status"].ToString();
            if (string.Equals(gatewayStatus, "VALID", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(gatewayStatus, "VALIDATED", StringComparison.OrdinalIgnoreCase))
            {
                return await VerifyAndUpdatePaymentAsync(payment, cancellationToken);
            }

            payment.Status = fallbackStatus ?? MapGatewayStatus(gatewayStatus);
            payment.FailureReason = form["error"].ToString();
            payment.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ToVerifyResult(payment, payment.Status == PaymentStatus.Success);
        }

        private async Task<PaymentVerifyResult> VerifyAndUpdatePaymentAsync(
            PaymentEntity payment,
            CancellationToken cancellationToken)
        {
            var gatewayResult = await _paymentGateway.VerifyAsync(payment.TransactionId, cancellationToken);
            var amountMatches = gatewayResult.Amount == 0 || gatewayResult.Amount == payment.Amount;
            var currencyMatches = string.IsNullOrWhiteSpace(gatewayResult.Status) ||
                string.IsNullOrWhiteSpace(payment.Currency) ||
                gatewayResult.RawResponse.Contains(payment.Currency, StringComparison.OrdinalIgnoreCase);

            var isSuccess = gatewayResult.IsSuccess && amountMatches && currencyMatches;

            payment.Status = isSuccess ? PaymentStatus.Validated : PaymentStatus.Failed;
            payment.ValidationId = gatewayResult.ValidationId ?? payment.ValidationId;
            payment.RawGatewayResponse = gatewayResult.RawResponse;
            payment.FailureReason = isSuccess ? string.Empty : "SSLCommerz verification failed.";
            payment.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ToVerifyResult(payment, isSuccess);
        }

        private static PaymentVerifyResult ToVerifyResult(PaymentEntity payment, bool isSuccess)
        {
            return new PaymentVerifyResult
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                ReceiptId = payment.ReceiptId,
                IsSuccess = isSuccess,
                Status = payment.Status.ToString(),
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                ValidationId = string.IsNullOrWhiteSpace(payment.ValidationId) ? null : payment.ValidationId,
                RawResponse = payment.RawGatewayResponse
            };
        }

        private static PaymentStatus MapGatewayStatus(string gatewayStatus)
        {
            return gatewayStatus.ToUpperInvariant() switch
            {
                "FAILED" or "FAIL" => PaymentStatus.Failed,
                "CANCELLED" or "CANCELED" or "CANCEL" => PaymentStatus.Cancelled,
                "VALID" or "VALIDATED" => PaymentStatus.Validated,
                _ => PaymentStatus.Pending
            };
        }

        private enum SslCommerzCallbackKind
        {
            SuccessRedirect,
            FailRedirect,
            CancelRedirect
        }
    }
}
