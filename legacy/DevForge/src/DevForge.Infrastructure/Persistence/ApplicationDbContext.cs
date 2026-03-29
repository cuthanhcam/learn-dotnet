using DevForge.Application.Common.Events;
using DevForge.Domain.Common;
using DevForge.Domain.Entities;
using DevForge.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DevForge.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IMediator? _mediator;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) 
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events before saving
            await DispatchDomainEventsAsync(cancellationToken);

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            if (_mediator == null) return;

            var entities = ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear events before publishing to avoid infinite loops
            entities.ForEach(e => e.ClearDomainEvents());

            // Convert domain events to MediatR notifications
            foreach (var domainEvent in domainEvents)
            {
                await PublishDomainEventAsync(domainEvent, cancellationToken);
            }
        }

        private async Task PublishDomainEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (_mediator == null) return;

            // Create a wrapper notification that MediatR can handle
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification != null)
            {
                await _mediator.Publish(notification, cancellationToken);
            }
        }
    }
}
