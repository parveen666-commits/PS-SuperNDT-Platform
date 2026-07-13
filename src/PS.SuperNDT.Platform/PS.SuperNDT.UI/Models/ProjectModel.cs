using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ProjectModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ProjectCode { get; set; } = string.Empty;

    public string ProjectName { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;
}