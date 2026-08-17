using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Services;

public sealed class UserRoleService
{
    private readonly Dictionary<string, HashSet<string>> _permissions =
        new(StringComparer.OrdinalIgnoreCase);

    public UserRoleService()
    {
        RegisterRole(
            "Admin",
            new[]
            {
                "Dashboard",
                "Acquisition",
                "Review",
                "Reporting",
                "Settings",
                "UserManagement",
                "License",
                "Audit",
                "Detector",
                "PLC"
            });

        RegisterRole(
            "LevelII",
            new[]
            {
                "Dashboard",
                "Acquisition",
                "Review",
                "Reporting"
            });

        RegisterRole(
            "LevelIII",
            new[]
            {
                "Dashboard",
                "Acquisition",
                "Review",
                "Reporting",
                "Settings",
                "Audit"
            });

        RegisterRole(
            "Operator",
            new[]
            {
                "Dashboard",
                "Acquisition"
            });

        RegisterRole(
            "Viewer",
            new[]
            {
                "Dashboard",
                "Review"
            });
    }

    public void RegisterRole(
        string role,
        IEnumerable<string> permissions)
    {
        _permissions[role] =
            new HashSet<string>(
                permissions,
                StringComparer.OrdinalIgnoreCase);
    }

    public bool HasPermission(
        string role,
        string permission)
    {
        if (!_permissions.TryGetValue(
                role,
                out var rolePermissions))
        {
            return false;
        }

        return rolePermissions.Contains(permission);
    }
}