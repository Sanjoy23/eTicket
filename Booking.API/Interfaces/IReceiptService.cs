namespace Booking.API.Interfaces
{
    public interface IReceiptService
    {
        Task<string> GenerateUniqueReceiptNumberAsync(string prefix = "TKT", CancellationToken cancellationToken = default);
    }
}
