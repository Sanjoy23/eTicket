using Booking.API.Application.Commands;
using Booking.API.Application.Queries;
using Booking.API.Dtos;
using Booking.API.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        ISeatLockService seatLockService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ISeatLockService _seatLockService = seatLockService;

        [HttpPost("book")]
        public async Task<IActionResult> Booking([FromBody] BookSeatsCommand command)
        {
            var result = await _mediator.Send(command);
            return result.BookingId == Guid.Empty ? BadRequest("Booking Failed") : Ok(result);
        }

        [HttpPost("{bookingId:guid}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(Guid bookingId, [FromBody] ConfirmPaymentRequest request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Bookings.GetByIdWithSeatsAsync(bookingId, cancellationToken);
            if (booking is null)
            {
                return NotFound("Booking not found.");
            }

            if (booking.Status == BookingStatus.Paid)
            {
                return Ok(new { booking.BookingId, booking.Status });
            }

            var payment = await _paymentService.VerifyPaymentAsync(request.PaymentId, cancellationToken);
            if (!payment.IsSuccess || payment.BookingId != bookingId)
            {
                return BadRequest("Payment was not verified for this booking.");
            }

            if (payment.Amount != 0 && payment.Amount != booking.TotalAmount)
            {
                return BadRequest("Verified payment amount does not match booking amount.");
            }

            var seatIds = booking.BookingSeats.Select(seat => seat.EventSeatId).ToList();
            await _seatLockService.ConfirmSeatsAsync(request.SessionId, booking.BookingId, booking.UserId, seatIds, cancellationToken);

            booking.Status = BookingStatus.Paid;
            var receipt = await _unitOfWork.Receipts.GetByIdAsync(payment.ReceiptId, cancellationToken);
            if (receipt is not null)
            {
                receipt.IsPaid = true;
                receipt.PaymentDate = DateTime.UtcNow;
                receipt.TransactionResultText = payment.Status;
                receipt.PaymentInfo = payment.TransactionId;
                receipt.ModifiedOn = DateTime.UtcNow;
                receipt.ModifiedBy = booking.UserId;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(new { booking.BookingId, booking.Status });
        }

        [HttpPost("cancel")]
        public async Task<Results<NoContent, NotFound<string>>> Cancel([FromBody] CancelSeatBookingCommand command)
        {
            await _mediator.Send(command);
            return TypedResults.NoContent();
        }

        [HttpGet("{bookingId}")]
        public async Task<Results<Ok<BookingDto>, NotFound>> Get(Guid bookingId)
        {
            var result = await _mediator.Send(new BookingByIdQuery { BookingId = bookingId });
            return result is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(result);
        }
    }
}
