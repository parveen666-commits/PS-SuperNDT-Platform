using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.Services;

public sealed class AuthorizationService
{
    private readonly AccessControlService _accessControlService;

    public AuthorizationService(
        AccessControlService accessControlService)
    {
        _accessControlService = accessControlService;
    }

    public void RefreshRoleFromSession()
    {
        _accessControlService.SetRole(
            UserSessionService.Instance.Role);
    }

    public bool CanCreateJob()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanCreateJob();
    }

    public bool CanOpenJob()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanOpenJob();
    }

    public bool CanReview()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanReview();
    }

    public bool CanGenerateReports()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanGenerateReports();
    }

    public bool CanManageUsers()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanManageUsers();
    }

    public bool CanViewAuditLog()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanViewAuditLog();
    }

    public bool CanOpenSettings()
    {
        RefreshRoleFromSession();
        return _accessControlService.CanOpenSettings();
    }
}