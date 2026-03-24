namespace eMotoCare.Domain.ValueObjects.Commons
{
    public class PhoneNumber : IEquatable<PhoneNumber>
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty.", nameof(value));

            var normalized = Normalize(value);

            if (normalized.Length is < 8 or > 15)
                throw new ArgumentException("Phone number length is invalid.", nameof(value));

            Value = normalized;
        }

        public static PhoneNumber Create(string value) => new(value);

        public override string ToString() => Value;

        public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;

        public override bool Equals(object? obj) => obj is PhoneNumber other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PhoneNumber? left, PhoneNumber? right) => Equals(left, right);

        public static bool operator !=(PhoneNumber? left, PhoneNumber? right) => !Equals(left, right);

        private static string Normalize(string input)
        {
            var trimmed = input.Trim();
            var hasPlus = trimmed.StartsWith('+');

            var digits = new string(trimmed.Where(char.IsDigit).ToArray());

            return hasPlus ? $"+{digits}" : digits;
        }
    }
}
