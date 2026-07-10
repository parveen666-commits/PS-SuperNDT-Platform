using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportModel
{
    public string ReportNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Inspector { get; set; } = string.Empty;

    public DateTime ReportDate { get; set; } = DateTime.Now;

    public string Result { get; set; } = "ACCEPTED";

    public string Remarks { get; set; } = string.Empty;

    public string ReportFilePath { get; set; } = string.Empty;
}