using System;

namespace PS.SuperNDT.UI.Services;

public sealed class AccessControlService
{
    private readonly UserRoleService _userRoleService;

    public AccessControlService(
        UserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public string CurrentRole { get; private set; } = "Viewer";

    public event EventHandler? RoleChanged;

    public void SetRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return;
        }

        CurrentRole = role;

        RoleChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public bool CanAccess(
        string permission)
    {
        return _userRoleService.HasPermission(
            CurrentRole,
            permission);
    }

    public bool CanOpenJob()
    {
        return CanAccess("Acquisition");
    }

    public bool CanCreateJob()
    {
        return CanAccess("Acquisition");
    }

    public bool CanReview()
    {
        return CanAccess("Review");
    }

    public bool CanGenerateReports()
    {
        return CanAccess("Reporting");
    }

    public bool CanManageUsers()
    {
        return CanAccess("UserManagement");
    }

    public bool CanOpenSettings()
    {
        return CanAccess("Settings");
    }

    public bool CanViewAuditLog()
    {
        return CanAccess("Audit");
    }
}