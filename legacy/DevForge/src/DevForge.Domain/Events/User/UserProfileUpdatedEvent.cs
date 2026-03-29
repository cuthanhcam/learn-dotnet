namespace DevForge.Domain.Events.User
{
    public class UserProfileUpdatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string NewUsername { get; }
        public string NewEmail { get; }

        public UserProfileUpdatedEvent(Guid userId, string newUsername, string newEmail)
        {
            UserId = userId;
            NewUsername = newUsername;
            NewEmail = newEmail;
        }
    }
}
