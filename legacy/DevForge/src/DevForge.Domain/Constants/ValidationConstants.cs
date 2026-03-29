namespace DevForge.Domain.Constants
{
    public static class ValidationConstants
    {
        public static class User
        {
            public const int UsernameMinLength = 3;
            public const int UsernameMaxLength = 50;
            public const int EmailMaxLength = 254;
            public const int PasswordMinLength = 8;
            public const int PasswordMaxLength = 128;
            public const int PhoneNumberMinLength = 10;
            public const int PhoneNumberMaxLength = 15;
        }

        public static class Role
        {
            public const int NameMaxLength = 50;
            public const int DescriptionMaxLength = 500;
        }

        public static class Permission
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 500;
            public const int CategoryMaxLength = 50;
        }

        public static class Token
        {
            public const int RefreshTokenExpirationDays = 7;
            public const int EmailConfirmationTokenExpirationHours = 24;
            public const int PasswordResetTokenExpirationMinutes = 60;
        }

        public static class Security
        {
            public const int MaxLoginAttempts = 5;
            public const int LockoutDurationMinutes = 15;
            public const int PasswordHashMinLength = 20;
        }
    }
}
