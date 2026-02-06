using DevForge.Domain.Common;
using DevForge.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace DevForge.Domain.ValueObjects
{
    public class PasswordHash : ValueObject
    {
        public string Value { get; private set; }

        private PasswordHash(string value)
        {
            Value = value;
        }

        public static PasswordHash Create(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new DomainException("Password hash cannot be empty");

            if (hashedPassword.Length < 20)
                throw new DomainException("Invalid password hash format");

            return new PasswordHash(hashedPassword);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;
    }

    public class Password : ValueObject
    {
        public string Value { get; private set; }

        private Password(string value)
        {
            Value = value;
        }

        public static Password Create(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException("Password cannot be empty");

            if (password.Length < 8)
                throw new DomainException("Password must be at least 8 characters long");

            if (password.Length > 128)
                throw new DomainException("Password must not exceed 128 characters");

            if (!HasUpperCase(password))
                throw new DomainException("Password must contain at least one uppercase letter");

            if (!HasLowerCase(password))
                throw new DomainException("Password must contain at least one lowercase letter");

            if (!HasDigit(password))
                throw new DomainException("Password must contain at least one digit");

            if (!HasSpecialCharacter(password))
                throw new DomainException("Password must contain at least one special character");

            return new Password(password);
        }

        private static bool HasUpperCase(string password) => password.Any(char.IsUpper);
        private static bool HasLowerCase(string password) => password.Any(char.IsLower);
        private static bool HasDigit(string password) => password.Any(char.IsDigit);
        private static bool HasSpecialCharacter(string password) => 
            Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => new string('*', Value.Length);

        public static implicit operator string(Password password) => password.Value;
    }
}
