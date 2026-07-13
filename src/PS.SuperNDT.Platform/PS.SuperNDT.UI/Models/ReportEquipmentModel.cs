using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportEquipmentModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string EquipmentName { get; set; } = string.Empty;

    public string EquipmentId { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string ModelNumber { get; set; } = string.Empty;

    public string CalibrationNumber { get; set; } = string.Empty;

    public DateTime CalibrationDate { get; set; } = DateTime.Now;

    public DateTime CalibrationDueDate { get; set; } = DateTime.Now.AddYears(1);

    public string Status { get; set; } = "Valid";
}