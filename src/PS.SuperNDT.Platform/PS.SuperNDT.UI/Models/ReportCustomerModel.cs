using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportCustomerModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string ProjectName { get; set; } = string.Empty;

    public string SiteLocation { get; set; } = string.Empty;

    public string ContactPerson { get; set; } = string.Empty;

    public string ContactNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PurchaseOrderNumber { get; set; } = string.Empty;
}