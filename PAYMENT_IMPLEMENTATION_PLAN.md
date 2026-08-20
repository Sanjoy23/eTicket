# SSLCommerz Payment Implementation Plan

## Current State

The booking flow currently locks seats, creates a pending booking and receipt, then calls the payment service. The payment service can initiate an SSLCommerz gateway session, but the full payment lifecycle is incomplete.

Main gaps:

- Booking marks a booking as paid immediately after payment initiation.
- Booking and Payment API route contracts do not match.
- Booking expects `PaymentId` and `PaymentUrl`, but Payment API returns the raw SSLCommerz initiation result.
- Payment verification endpoint is not implemented.
- SSLCommerz success, fail, cancel, and IPN callback endpoints are not implemented.
- Payment records are not persisted even though payment entities and `PaymentDbContext` exist.
- Seats are not confirmed after verified payment in the current automatic flow.
- Receipt payment status is not updated after successful payment.
- Client-provided total amount is trusted instead of recalculating server-side.

## Target Payment Flow

1. User requests booking with session and seat IDs.
2. Booking API validates seats and calculates total amount server-side.
3. Booking API locks seats in Event API.
4. Booking API creates booking with `Pending` status.
5. Booking API creates receipt with `IsPaid = false`.
6. Booking API calls Payment API to create a payment session.
7. Payment API creates a local payment record with status `Pending`.
8. Payment API initiates SSLCommerz and stores:
   - `PaymentId`
   - `BookingId`
   - `UserId`
   - `ReceiptId`
   - `TransactionId`
   - `SessionKey`
   - `GatewayPageURL`
   - amount and currency
9. Booking API returns `BookingId`, `PaymentId`, and gateway redirect URL to the client.
10. SSLCommerz sends success/fail/cancel redirect and IPN callback.
11. Payment API validates the transaction with SSLCommerz validator API.
12. Payment API updates local payment status.
13. Booking API is notified or queried to finalize the booking.
14. Booking API confirms seats only after verified payment.
15. Booking API marks booking as `Paid` and receipt as paid.
16. Failed, cancelled, or expired payments cancel/expire the booking and release seats.

## Phase 1: Fix Service Contracts

### Tasks

- Rename or add Payment API initiation endpoint to match Booking API:
  - Preferred: `POST /api/payments/initiate`
- Add request DTO shared contract fields:
  - `BookingId`
  - `UserId`
  - `ReceiptId`
  - `ReceiptNumber`
  - `Amount`
  - `Currency`
  - customer name, email, phone, address
  - product information
- Add initiation response DTO:
  - `PaymentId`
  - `BookingId`
  - `TransactionId`
  - `SessionKey`
  - `PaymentUrl`
  - `Status`
- Update Booking API `PaymentService` to call the correct route.
- Map SSLCommerz `GatewayPageURL` to Booking API `PaymentUrl`.

### Acceptance Checks

- Booking API can call Payment API initiation endpoint without 404.
- Response contains non-empty `PaymentId`, `TransactionId`, and `PaymentUrl`.
- Booking stays `Pending` after payment initiation.

## Phase 2: Persist Payment Records

### Tasks

- Update `PaymentEntity` to include all required fields:
  - `PaymentId`
  - `BookingId`
  - `UserId`
  - `ReceiptId`
  - `ReceiptNumber`
  - `TransactionId`
  - `SessionKey`
  - `GatewayPaymentUrl`
  - `Amount`
  - `Currency`
  - `Status`
  - `FailureReason`
  - `ValidationId`
  - `BankTransactionId`
  - `RawGatewayResponse`
  - `CreatedAtUtc`
  - `UpdatedAtUtc`
- Use a real `PaymentStatus` enum instead of `int`.
- Add indexes:
  - unique `TransactionId`
  - index `BookingId`
  - index `ReceiptId`
  - index `Status`
- Save payment record before returning initiation response.
- Add migration for payment table changes.

### Acceptance Checks

- Every payment initiation creates one local payment row.
- Duplicate `TransactionId` cannot be inserted.
- Payment record can be found by `PaymentId`, `BookingId`, and `TransactionId`.

## Phase 3: Add SSLCommerz Callback and IPN Handling

### Tasks

- Add endpoints in Payment API:
  - `POST /api/payments/sslcommerz/success`
  - `POST /api/payments/sslcommerz/fail`
  - `POST /api/payments/sslcommerz/cancel`
  - `POST /api/payments/sslcommerz/ipn`
- Accept SSLCommerz callback form fields, especially:
  - `tran_id`
  - `val_id`
  - `amount`
  - `currency`
  - `status`
  - `bank_tran_id`
  - `card_type`
  - `risk_level`
  - `risk_title`
- Always validate successful payment using SSLCommerz validation API.
- Validate that gateway amount and currency match local payment record.
- Treat redirect success as user-facing only; use IPN or explicit validation as the source of truth.
- Store raw callback data for audit/debugging.

### Acceptance Checks

- Success callback validates transaction before marking payment successful.
- Fail and cancel callbacks update payment status correctly.
- IPN endpoint is idempotent and can safely process repeated callbacks.
- Amount/currency mismatch prevents payment success.

## Phase 4: Implement Payment Verification Endpoint

### Tasks

- Add endpoint:
  - `POST /api/payments/{paymentId:guid}/verify`
- Load local payment by `PaymentId`.
- Revalidate by `TransactionId` using SSLCommerz validator API.
- Return:
  - `PaymentId`
  - `BookingId`
  - `IsSuccess`
  - `Status`
  - `TransactionId`
  - `Amount`
  - `Currency`
- Update Booking API `VerifyPaymentAsync` to match this response.

### Acceptance Checks

- Booking API confirm-payment no longer hits a missing endpoint.
- Verification fails when `BookingId`, amount, or currency does not match.
- Verification succeeds only for validated SSLCommerz payments.

## Phase 5: Finalize Booking After Verified Payment

### Tasks

- Remove the immediate `booking.Status = BookingStatus.Paid` after initiation.
- Add a booking finalization flow:
  - verify payment
  - confirm seats in Event API
  - mark booking `Paid`
  - mark receipt `IsPaid = true`
  - save transaction information on receipt
- Ensure `ConfirmPayment` does not return early before confirming seats if seats are not sold yet.
- Make finalization idempotent:
  - repeated success/IPN should not double-confirm seats
  - already-paid booking should return success only if seats are confirmed
- Decide whether Payment API calls Booking API on IPN, or Booking API polls/verifies.
  - Recommended for this codebase: Payment API stores payment truth; Booking API finalizes when frontend calls confirm-payment after redirect.
  - Later improvement: use an outbox/event bus for automatic finalization.

### Acceptance Checks

- Booking is `Pending` while user is on SSLCommerz page.
- Booking becomes `Paid` only after verified payment.
- Seats become `Sold` only after verified payment.
- Receipt becomes paid only after verified payment.
- Repeated confirm-payment request is safe.

## Phase 6: Failure, Cancellation, and Expiry Handling

### Tasks

- On payment fail/cancel:
  - mark payment failed/cancelled
  - mark booking cancelled
  - release locked seats
- Add expiry handling for bookings whose seat lock expires before payment completes.
- Add background job or scheduled command to:
  - find pending bookings older than lock duration
  - verify no successful payment exists
  - mark booking expired
  - release seats
- Ensure late successful IPN is handled carefully:
  - if seats are still available, finalize
  - if seats already sold/released to another booking, flag for manual review/refund

### Acceptance Checks

- Failed payment releases seats.
- Cancelled payment releases seats.
- Expired payment releases seats.
- Late payment after seat expiry does not silently create an invalid paid booking.

## Phase 7: Amount and Seat Validation

### Tasks

- Stop trusting `TotalAmount` from client request.
- Fetch seat/session pricing from Event API or booking read model.
- Calculate total amount on server.
- Validate all selected seats:
  - belong to session
  - are available or lockable
  - match expected event/session
  - are not duplicated
- Pass server-calculated amount to Payment API.

### Acceptance Checks

- Client cannot reduce payable amount by modifying request body.
- Payment verification compares gateway paid amount with server-calculated amount.
- Duplicate seat IDs are handled consistently.

## Phase 8: Configuration and Modern .NET Improvements

### Tasks

- Move service URLs to configuration:
  - `Services:EventApi:BaseUrl`
  - `Services:PaymentApi:BaseUrl`
- Use typed HttpClients:
  - `AddHttpClient<ISeatLockService, SeatLockService>()`
  - `AddHttpClient<IPaymentService, PaymentService>()`
- Add timeout and resilience policies with `Microsoft.Extensions.Http.Resilience`.
- Use `IOptions<SslCommerzOptions>` with validation.
- Store SSLCommerz credentials in user-secrets, environment variables, or deployment secrets.
- Replace generic `Exception` throws with domain-specific exceptions or `ProblemDetails`.
- Replace `Console.WriteLine` with `ILogger`.
- Use `DateTimeOffset.UtcNow` or `TimeProvider` for testable timestamps.
- Consider `AddOpenApi` if moving to newer .NET conventions.

### Acceptance Checks

- No service base URL is hardcoded in code.
- Payment and Event clients have timeouts.
- Logs include payment/booking correlation IDs.
- Secrets are not committed in appsettings files.

## Phase 9: Observability and Audit Trail

### Tasks

- Add structured logs for:
  - booking created
  - seats locked
  - payment initiated
  - SSLCommerz callback received
  - payment validated
  - seats confirmed
  - booking finalized
  - payment failed/cancelled/expired
- Store raw gateway callback payloads.
- Add correlation fields:
  - `BookingId`
  - `PaymentId`
  - `TransactionId`
  - `ReceiptNumber`
- Add admin/debug endpoint or query for payment status history if needed.

### Acceptance Checks

- A single booking/payment can be traced across Booking API, Payment API, and Event API logs.
- Gateway callback data is available for support/debugging.

## Phase 10: Tests

### Unit Tests

- Booking initiation keeps booking pending.
- Booking initiation releases seats when payment initiation fails.
- Payment initiation persists payment record.
- Payment validation fails on amount mismatch.
- Payment validation fails on currency mismatch.
- Finalization confirms seats and marks booking paid.
- Finalization is idempotent.

### Integration Tests

- Booking API to Payment API initiation contract.
- Payment API SSLCommerz callback handling with mocked gateway response.
- Booking confirm-payment endpoint with successful payment.
- Fail/cancel payment releases seats.
- Expired lock prevents invalid finalization.

### Acceptance Checks

- Tests cover the full happy path:
  - lock seats
  - initiate payment
  - validate payment
  - confirm seats
  - mark booking and receipt paid
- Tests cover failure paths:
  - payment initiation failure
  - gateway failure
  - cancel callback
  - expired seat lock
  - duplicate callback/IPN

## Recommended Implementation Order

1. Fix initiation route and DTO mismatch.
2. Stop marking booking paid after initiation.
3. Persist payment records with transaction ID and gateway URL.
4. Add payment verification endpoint.
5. Add SSLCommerz success/fail/cancel/IPN endpoints.
6. Finalize booking only after verified payment.
7. Add fail/cancel/expiry cleanup.
8. Move hardcoded config into options and typed clients.
9. Add tests around the critical payment lifecycle.
10. Add observability and audit improvements.

## Definition of Done

- User can select seats and start payment.
- Seats are locked while payment is pending.
- Gateway redirect URL is returned to the client.
- Booking remains pending until SSLCommerz payment is verified.
- Successful verified payment confirms seats and marks booking paid.
- Receipt is marked paid only after verified payment.
- Failed, cancelled, or expired payment releases seats.
- Payment, booking, receipt, and seat inventory states remain consistent.
- Repeated callbacks or confirm requests are idempotent.
- Payment amount and currency are validated against server-side data.
- Critical flows are covered by tests.
