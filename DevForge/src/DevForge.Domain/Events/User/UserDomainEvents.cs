using DevForge.Domain.Events;

namespace DevForge.Domain.Events.User
{
    /// <summary>
    /// Event raised when user's profile is updated
    /// </summary>
    public sealed class UserProfileUpdatedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string? PhoneNumber { get; }
        public bool PhoneNumberChanged { get; }

        public UserProfileUpdatedDomainEvent(Guid userId, string? phoneNumber, bool phoneNumberChanged)
        {
            UserId = userId;
            PhoneNumber = phoneNumber;
            PhoneNumberChanged = phoneNumberChanged;
        }
    }

    /// <summary>
    /// Event raised when user's email is confirmed
    /// </summary>
    public sealed class UserEmailConfirmedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }

        public UserEmailConfirmedDomainEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }

    /// <summary>
    /// Event raised when user's phone number is confirmed
    /// </summary>
    public sealed class UserPhoneNumberConfirmedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string PhoneNumber { get; }

        public UserPhoneNumberConfirmedDomainEvent(Guid userId, string phoneNumber)
        {
            UserId = userId;
            PhoneNumber = phoneNumber;
        }
    }

    /// <summary>
    /// Event raised when two-factor authentication is enabled
    /// </summary>
    public sealed class TwoFactorEnabledDomainEvent : DomainEvent
    {
        public Guid UserId { get; }

        public TwoFactorEnabledDomainEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Event raised when two-factor authentication is disabled
    /// </summary>
    public sealed class TwoFactorDisabledDomainEvent : DomainEvent
    {
        public Guid UserId { get; }

        public TwoFactorDisabledDomainEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Event raised when user account is activated
    /// </summary>
    public sealed class UserActivatedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserActivatedDomainEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Event raised when user account is deactivated
    /// </summary>
    public sealed class UserDeactivatedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string? Reason { get; }

        public UserDeactivatedDomainEvent(Guid userId, string? reason = null)
        {
            UserId = userId;
            Reason = reason;
        }
    }

    /// <summary>
    /// Event raised when password reset is requested
    /// </summary>
    public sealed class PasswordResetRequestedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }

        public PasswordResetRequestedDomainEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }

    /// <summary>
    /// Event raised when password is successfully reset
    /// </summary>
    public sealed class PasswordResetCompletedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }

        public PasswordResetCompletedDomainEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Event raised when user role assignment changes
    /// </summary>
    public sealed class UserRolesChangedDomainEvent : DomainEvent
    {
        public Guid UserId { get; }
        public List<Guid> AddedRoleIds { get; }
        public List<Guid> RemovedRoleIds { get; }

        public UserRolesChangedDomainEvent(Guid userId, List<Guid> addedRoleIds, List<Guid> removedRoleIds)
        {
            UserId = userId;
            AddedRoleIds = addedRoleIds;
            RemovedRoleIds = removedRoleIds;
        }
    }
}
