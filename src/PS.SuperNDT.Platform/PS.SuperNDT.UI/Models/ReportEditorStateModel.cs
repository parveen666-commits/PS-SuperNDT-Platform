using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportEditorStateModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public bool IsDraft { get; set; } = true;

    public bool IsValidated { get; set; }

    public bool IsPreviewGenerated { get; set; }

    public bool IsPdfExported { get; set; }

    public string CurrentStatus { get; set; } = "Draft";

    public DateTime LastUpdatedOn { get; set; } = DateTime.Now;
}