namespace DevForge.Domain.Constants
{
    public static class Permissions
    {
        public const string UsersRead = "users.read";
        public const string UsersCreate = "users.create";
        public const string UsersUpdate = "users.update";
        public const string UsersDelete = "users.delete";
        public const string UsersManageRoles = "users.manage_roles";
        
        public const string RolesRead = "roles.read";
        public const string RolesCreate = "roles.create";
        public const string RolesUpdate = "roles.update";
        public const string RolesDelete = "roles.delete";
        public const string RolesManagePermissions = "roles.manage_permissions";
        
        public const string PermissionsRead = "permissions.read";
        public const string PermissionsCreate = "permissions.create";
        public const string PermissionsUpdate = "permissions.update";
        public const string PermissionsDelete = "permissions.delete";
        
        public const string SystemAdmin = "system.admin";
        public const string SystemAudit = "system.audit";
        public const string SystemSettings = "system.settings";

        public static class Categories
        {
            public const string Users = "Users";
            public const string Roles = "Roles";
            public const string Permissions = "Permissions";
            public const string System = "System";
        }
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
        public const string Moderator = "Moderator";
    }
}
