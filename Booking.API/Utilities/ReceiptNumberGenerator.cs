using System.Security.Cryptography;

namespace Booking.API.Utilities
{
    public static class ReceiptNumberGenerator
    {
        private static readonly char[] _chars =
        "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();
        public static string Generate(string prefix = "TKT")
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = GenerateRandomSegment(8);
            return $"{prefix}-{datePart}-{randomPart}";
        }
        private static string GenerateRandomSegment(int length)
        {
            var result = new char[length];
            var randomBytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
                result[i] = _chars[randomBytes[i] % _chars.Length];

            return new string(result);
        }

        public static string GenerateWithSequence(string prefix = "TKT", int sequence = 0)
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = GenerateRandomSegment(8);
            var sequencePart = sequence.ToString().PadLeft(4, '0');
            return $"{prefix}-{datePart}-{randomPart}-{sequencePart}";
        }
    }
}
