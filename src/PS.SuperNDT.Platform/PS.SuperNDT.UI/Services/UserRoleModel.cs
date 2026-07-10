namespace PS.SuperNDT.UI.Models;

public sealed class UserRoleModel
{
    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = "Viewer";

    public bool IsActive { get; set; } = true;
}