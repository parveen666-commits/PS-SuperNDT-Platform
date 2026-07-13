using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportTemplateModel
{
    public Guid Id { get; set; }

    public string TemplateName { get; set; } = string.Empty;

    public string TemplateCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string HeaderText { get; set; } = string.Empty;

    public string FooterText { get; set; } = string.Empty;

    public string LogoPath { get; set; } = string.Empty;

    public bool IncludeImages { get; set; }

    public bool IncludeAnnotations { get; set; }

    public bool IncludeSignature { get; set; }

    public bool IsDefault { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string Remarks { get; set; } = string.Empty;
}