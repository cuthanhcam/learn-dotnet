using DevForge.Domain.Common;
using DevForge.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DevForge.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; private set; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            email = email.Trim().ToLowerInvariant();

            if (!IsValidEmailFormat(email))
                throw new DomainException($"'{email}' is not a valid email address");

            if (email.Length > 254)
                throw new DomainException("Email address is too long (max 254 characters)");

            return new Email(email);
        }

        private static bool IsValidEmailFormat(string email)
        {
            // Simple but effective email regex pattern
            // More complex patterns exist but this covers 99% of cases
            var emailRegex = new Regex(
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return emailRegex.IsMatch(email);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
    }
}
