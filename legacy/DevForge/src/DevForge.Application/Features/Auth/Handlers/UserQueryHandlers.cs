using DevForge.Application.Common.Extensions;
using DevForge.Application.Common.Models;
using DevForge.Application.Features.Auth.DTOs;
using DevForge.Application.Features.Auth.Queries;
using DevForge.Domain.Common;
using DevForge.Domain.Entities;
using DevForge.Domain.Repositories;
using DevForge.Domain.Specifications;
using DevForge.Domain.Specifications.Users;
using DevForge.Domain.ValueObjects;
using MediatR;

namespace DevForge.Application.Features.Auth.Handlers
{
    /// <summary>
    /// Handler for getting user by ID
    /// </summary>
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                return Result<UserDto>.Failure(Error.NotFound("Role.NotFound", "User not found"));

            var userDto = MapToUserDto(user);
            return Result<UserDto>.Success(userDto);
        }

        private UserDto MapToUserDto(User user)
        {
            var roles = user.UserRoles.Select(ur => ur.RoleId.ToString()).ToList();
            var permissions = new List<string>(); // Will be populated from role permissions

            return new UserDto(
                user.Id,
                user.Username.Value,
                user.Email.Value,
                user.PhoneNumber?.Value,
                user.IsActive,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.CreatedAt,
                user.LastLoginAt,
                roles,
                permissions
            );
        }
    }

    /// <summary>
    /// Handler for getting current user
    /// </summary>
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentUserQueryHandler(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                return Result<UserDto>.Failure(Error.NotFound("Role.NotFound", "User not found"));

            var roles = new List<string>();
            var permissions = new List<string>();

            foreach (var userRole in user.UserRoles)
            {
                var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(userRole.RoleId, cancellationToken);
                if (role != null)
                {
                    roles.Add(role.Name);
                    var rolePermissions = await _permissionRepository.GetPermissionsByRoleIdAsync(role.Id, cancellationToken);
                    permissions.AddRange(rolePermissions.Select(p => p.Name));
                }
            }

            permissions = permissions.Distinct().ToList();

            // Use mapping extension
            var userDto = user.ToDto(roles, permissions);

            return Result<UserDto>.Success(userDto);
        }
    }

    /// <summary>
    /// Handler for getting user profile with detailed information
    /// </summary>
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetUserProfileQueryHandler(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                return Result<UserProfileDto>.Failure(Error.NotFound("Role.NotFound", "User not found"));

            var roles = new List<RoleDto>();
            var permissions = new List<PermissionDto>();

            foreach (var userRole in user.UserRoles)
            {
                var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(userRole.RoleId, cancellationToken);
                if (role != null)
                {
                    roles.Add(new RoleDto(role.Id, role.Name, role.Description));

                    var rolePermissions = await _permissionRepository.GetPermissionsByRoleIdAsync(role.Id, cancellationToken);
                    permissions.AddRange(rolePermissions.Select(p => 
                        new PermissionDto(p.Id, p.Name, p.Description, p.Category)));
                }
            }

            permissions = permissions.DistinctBy(p => p.Id).ToList();

            var securityInfo = new UserSecurityInfoDto(
                user.TwoFactorEnabled,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.AccessFailedCount,
                user.IsLockedOut(),
                user.LockoutEnd,
                user.UpdatedAt // Approximate, would need separate tracking
            );

            var profileDto = new UserProfileDto(
                user.Id,
                user.Username.Value,
                user.Email.Value,
                user.PhoneNumber?.Value,
                user.IsActive,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.CreatedAt,
                user.UpdatedAt,
                user.LastLoginAt,
                user.AccessFailedCount,
                user.LockoutEnd,
                user.LockoutEnabled,
                roles,
                permissions,
                securityInfo
            );

            return Result<UserProfileDto>.Success(profileDto);
        }
    }

    /// <summary>
    /// Handler for getting paginated list of users
    /// </summary>
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            // Build specification based on filters
            var specifications = new List<Domain.Specifications.Specification<User>>();

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    specifications.Add(new ActiveUserSpecification());
            }

            if (request.EmailConfirmed.HasValue)
            {
                if (request.EmailConfirmed.Value)
                    specifications.Add(new EmailConfirmedSpecification());
            }

            // Combine specifications
            IEnumerable<User> users;
            if (specifications.Any())
            {
                var combinedSpec = specifications.First();
                foreach (var spec in specifications.Skip(1))
                {
                    combinedSpec = combinedSpec.And(spec);
                }
                users = await _userRepository.FindAsync(combinedSpec, cancellationToken);
            }
            else
            {
                users = await _userRepository.GetAllAsync(cancellationToken);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                users = users.Where(u => 
                    u.Username.Value.ToLower().Contains(searchLower) ||
                    u.Email.Value.ToLower().Contains(searchLower));
            }

            var totalCount = users.Count();

            // Apply pagination
            var paginatedUsers = users
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var userDtos = paginatedUsers.Select(u => new UserDto(
                u.Id,
                u.Username.Value,
                u.Email.Value,
                u.PhoneNumber?.Value,
                u.IsActive,
                u.EmailConfirmed,
                u.PhoneNumberConfirmed,
                u.TwoFactorEnabled,
                u.CreatedAt,
                u.LastLoginAt,
                new List<string>(),
                new List<string>()
            )).ToList();

            var result = new PaginatedList<UserDto>(userDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PaginatedList<UserDto>>.Success(result);
        }
    }

    /// <summary>
    /// Handler for getting users by role
    /// </summary>
    public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, Result<PaginatedList<UserDto>>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersByRoleQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsersByRoleAsync(request.RoleId, cancellationToken);

            var totalCount = users.Count();

            var paginatedUsers = users
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var userDtos = paginatedUsers.Select(u => new UserDto(
                u.Id,
                u.Username.Value,
                u.Email.Value,
                u.PhoneNumber?.Value,
                u.IsActive,
                u.EmailConfirmed,
                u.PhoneNumberConfirmed,
                u.TwoFactorEnabled,
                u.CreatedAt,
                u.LastLoginAt,
                new List<string>(),
                new List<string>()
            )).ToList();

            var result = new PaginatedList<UserDto>(userDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PaginatedList<UserDto>>.Success(result);
        }
    }

    /// <summary>
    /// Handler for checking if username exists
    /// </summary>
    public class CheckUsernameExistsQueryHandler : IRequestHandler<CheckUsernameExistsQuery, Result<bool>>
    {
        private readonly IUserRepository _userRepository;

        public CheckUsernameExistsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
        {
            var username = Username.Create(request.Username);
            var exists = await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
            return Result<bool>.Success(exists);
        }
    }

    /// <summary>
    /// Handler for checking if email exists
    /// </summary>
    public class CheckEmailExistsQueryHandler : IRequestHandler<CheckEmailExistsQuery, Result<bool>>
    {
        private readonly IUserRepository _userRepository;

        public CheckEmailExistsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var exists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
            return Result<bool>.Success(exists);
        }
    }

    /// <summary>
    /// Handler for getting user statistics
    /// </summary>
    public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, Result<UserStatisticsDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserStatisticsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserStatisticsDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
        {
            var allUsers = await _userRepository.GetAllAsync(cancellationToken);
            var usersList = allUsers.ToList();

            var today = DateTime.UtcNow.Date;
            var weekAgo = today.AddDays(-7);
            var monthAgo = today.AddMonths(-1);

            var stats = new UserStatisticsDto(
                TotalUsers: usersList.Count,
                ActiveUsers: usersList.Count(u => u.IsActive),
                InactiveUsers: usersList.Count(u => !u.IsActive),
                LockedUsers: usersList.Count(u => u.IsLockedOut()),
                UsersWithTwoFactor: usersList.Count(u => u.TwoFactorEnabled),
                UsersWithEmailConfirmed: usersList.Count(u => u.EmailConfirmed),
                UsersCreatedToday: usersList.Count(u => u.CreatedAt.Date == today),
                UsersCreatedThisWeek: usersList.Count(u => u.CreatedAt >= weekAgo),
                UsersCreatedThisMonth: usersList.Count(u => u.CreatedAt >= monthAgo)
            );

            return Result<UserStatisticsDto>.Success(stats);
        }
    }
}
