using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportEquipmentService
{
    private readonly ObservableCollection<ReportEquipmentModel> _equipment;

    public ReportEquipmentService()
    {
        _equipment = new ObservableCollection<ReportEquipmentModel>();
    }

    public ReadOnlyObservableCollection<ReportEquipmentModel> Equipment =>
        new(_equipment);

    public void AddEquipment(
        Guid reportId,
        string equipmentName,
        string equipmentId,
        string manufacturer,
        string modelNumber,
        string calibrationNumber,
        DateTime calibrationDate,
        DateTime calibrationDueDate)
    {
        _equipment.Add(
            new ReportEquipmentModel
            {
                ReportId = reportId,
                EquipmentName = equipmentName,
                EquipmentId = equipmentId,
                Manufacturer = manufacturer,
                ModelNumber = modelNumber,
                CalibrationNumber = calibrationNumber,
                CalibrationDate = calibrationDate,
                CalibrationDueDate = calibrationDueDate,
                Status = calibrationDueDate >= DateTime.Now
                    ? "Valid"
                    : "Expired"
            });
    }

    public bool IsCalibrationValid(
        ReportEquipmentModel equipment)
    {
        return equipment != null &&
               equipment.CalibrationDueDate >= DateTime.Now;
    }
}