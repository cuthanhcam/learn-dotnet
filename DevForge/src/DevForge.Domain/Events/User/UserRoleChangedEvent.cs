namespace DevForge.Domain.Events.User
{
    public class UserRoleChangedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public int OldRole { get; }
        public int NewRole { get; }

        public UserRoleChangedEvent(Guid userId, int oldRole, int newRole)
        {
            UserId = userId;
            OldRole = oldRole;
            NewRole = newRole;
        }
    }
}
