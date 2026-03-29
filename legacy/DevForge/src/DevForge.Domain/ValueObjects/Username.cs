using DevForge.Domain.Common;
using DevForge.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace DevForge.Domain.ValueObjects
{
    public class Username : ValueObject
    {
        public string Value { get; private set; }

        private Username(string value)
        {
            Value = value;
        }

        public static Username Create(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("Username cannot be empty");

            username = username.Trim();

            if (username.Length < 3)
                throw new DomainException("Username must be at least 3 characters");

            if (username.Length > 50)
                throw new DomainException("Username must not exceed 50 characters");

            if (!IsValidUsernameFormat(username))
                throw new DomainException("Username can only contain letters, numbers, underscores, and hyphens");

            return new Username(username);
        }

        private static bool IsValidUsernameFormat(string username)
        {
            var usernameRegex = new Regex(
                @"^[a-zA-Z0-9_-]+$",
                RegexOptions.Compiled);

            return usernameRegex.IsMatch(username);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Username username) => username.Value;
    }
}
