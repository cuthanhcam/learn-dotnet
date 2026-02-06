using DevForge.Application.Common.Models;

namespace DevForge.Application.Common.Errors
{
    public static class AuthErrors
    {
        public static Error InvalidCredentials => Error.Unauthorized(
            "Auth.InvalidCredentials",
            "Invalid username/email or password");

        public static Error UserNotFound => Error.NotFound(
            "Auth.UserNotFound",
            "User not found");

        public static Error UserInactive => Error.Forbidden(
            "Auth.UserInactive",
            "User account is inactive or locked");

        public static Error EmailNotConfirmed => Error.Forbidden(
            "Auth.EmailNotConfirmed",
            "Email address has not been confirmed");

        public static Error UsernameAlreadyExists => Error.Conflict(
            "Auth.UsernameExists",
            "Username is already taken");

        public static Error EmailAlreadyExists => Error.Conflict(
            "Auth.EmailExists",
            "Email address is already registered");

        public static Error InvalidRefreshToken => Error.Unauthorized(
            "Auth.InvalidRefreshToken",
            "Refresh token is invalid or expired");

        public static Error InvalidEmailConfirmationToken => Error.Validation(
            "Auth.InvalidEmailConfirmationToken",
            "Email confirmation token is invalid or expired");

        public static Error InvalidPasswordResetToken => Error.Validation(
            "Auth.InvalidPasswordResetToken",
            "Password reset token is invalid or expired");

        public static Error WeakPassword => Error.Validation(
            "Auth.WeakPassword",
            "Password does not meet security requirements");
    }
}
