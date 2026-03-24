using System.Text.RegularExpressions;

namespace eMotoCare.Domain.ValueObjects.Commons
{
    public class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty.", nameof(value));

            var normalized = value.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(normalized))
                throw new ArgumentException("Email format is invalid.", nameof(value));

            Value = normalized;
        }

        public static Email Create(string value) => new(value);

        public override string ToString() => Value;

        public bool Equals(Email? other) => other is not null && Value == other.Value;

        public override bool Equals(object? obj) => obj is Email other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Email? left, Email? right) => Equals(left, right);

        public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
    }
}
