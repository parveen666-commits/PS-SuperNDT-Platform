using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportTemplateModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string TemplateName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Standard { get; set; } = string.Empty;

    public string Revision { get; set; } = "1.0";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public string CreatedBy { get; set; } = string.Empty;
}