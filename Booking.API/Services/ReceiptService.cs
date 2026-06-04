using Booking.API.Interfaces;
using Booking.API.Utilities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Services
{
    public class ReceiptService(BookingDbContext dbContext) : IReceiptService
    {
        private readonly BookingDbContext _dbContext = dbContext;
        public async Task<string> GenerateUniqueReceiptNumberAsync(string prefix = "TKT", CancellationToken cancellationToken = default)
        {
            string receiptNumber;
            int attempts = 0;
            const int maxAttempts = 5;

            do
            {
                if (attempts >= maxAttempts)
                    throw new InvalidOperationException(
                        $"Failed to generate unique receipt number after {maxAttempts} attempts.");

                receiptNumber = ReceiptNumberGenerator.Generate(prefix);
                attempts++;
            }
            while (await _dbContext.Receipts
                .AnyAsync(r => r.ReceiptNumber == receiptNumber, cancellationToken));

            return receiptNumber;
        }
    }
}
