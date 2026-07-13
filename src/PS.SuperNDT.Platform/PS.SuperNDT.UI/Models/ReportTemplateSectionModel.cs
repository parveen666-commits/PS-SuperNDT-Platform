using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportTemplateSectionModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TemplateId { get; set; }

    public string SectionName { get; set; } = string.Empty;

    public string DisplayTitle { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool IsMandatory { get; set; }

    public string Description { get; set; } = string.Empty;
}