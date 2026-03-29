namespace DevForge.Domain.Events.User
{
    public class UserCreatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        public int Role { get; }

        public UserCreatedEvent(Guid userId, string username, string email, int role)
        {
            UserId = userId;
            Username = username;
            Email = email;
            Role = role;
        }
    }
}
