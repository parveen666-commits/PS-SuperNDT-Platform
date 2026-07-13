using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportProcedureModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ProcedureNumber { get; set; } = string.Empty;

    public string ProcedureTitle { get; set; } = string.Empty;

    public string Revision { get; set; } = string.Empty;

    public string StandardReference { get; set; } = string.Empty;

    public string PreparedBy { get; set; } = string.Empty;

    public DateTime ApprovedDate { get; set; } = DateTime.Now;

    public string Remarks { get; set; } = string.Empty;
}