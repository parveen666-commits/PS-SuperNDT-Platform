using System;

namespace PS.SuperNDT.UI.Models;

public sealed class JobModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string JobNumber { get; set; } = "";

    public string Customer { get; set; } = "";

    public string Project { get; set; } = "";

    public string Component { get; set; } = "";

    public string WeldNumber { get; set; } = "";

    public string Operator { get; set; } = "";

    public string Procedure { get; set; } = "";

    public string Material { get; set; } = "";

    public string Remark { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public bool IsClosed { get; set; }
}