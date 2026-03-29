using DevForge.Domain.Common;
using DevForge.Domain.Exceptions;

namespace DevForge.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; private set; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number cannot be empty");

            phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

            if (phoneNumber.Length < 10 || phoneNumber.Length > 15)
                throw new DomainException("Phone number must be between 10 and 15 digits");

            if (!phoneNumber.All(char.IsDigit) && !phoneNumber.StartsWith("+"))
                throw new DomainException("Phone number must contain only digits and optionally start with '+'");

            return new PhoneNumber(phoneNumber);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    }
}
