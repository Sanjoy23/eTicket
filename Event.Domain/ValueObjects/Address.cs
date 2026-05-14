namespace Event.Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; private set; } = default!;
        public string City { get; private set; } = default!;
        public string Country { get; private set; } = default!;

        private Address() { }

        public Address(string street, string city, string country)
        {
            Street = street;
            City = city;
            Country = country;
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {Country}";
        }
    }
}
