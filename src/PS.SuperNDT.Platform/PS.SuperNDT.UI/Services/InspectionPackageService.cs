using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class InspectionPackageService
{
    public ObservableCollection<InspectionPackageModel> Packages { get; } = new();

    public InspectionPackageModel CreatePackage(
        string jobNumber,
        string inspectionNumber)
    {
        var package = new InspectionPackageModel
        {
            Id = Guid.NewGuid(),
            PackageNumber = $"PKG-{DateTime.Now:yyyyMMddHHmmss}",
            JobNumber = jobNumber,
            InspectionNumber = inspectionNumber,
            CreatedOn = DateTime.Now,
            TransferStatus = TransferStatus.Pending
        };

        Packages.Add(package);

        return package;
    }

    public InspectionPackageModel? GetPackage(Guid id)
    {
        return Packages.FirstOrDefault(x => x.Id == id);
    }

    public void AddImage(
        Guid packageId,
        string imageFile,
        long fileSizeBytes)
    {
        var package = GetPackage(packageId);

        if (package is null)
            return;

        package.ImageFiles.Add(imageFile);
        package.TotalSizeBytes += fileSizeBytes;
    }

    public void ApproveForTransfer(Guid packageId)
    {
        var package = GetPackage(packageId);

        if (package is null)
            return;

        package.ApprovedForTransfer = true;
    }

    public void SetTransferStatus(
        Guid packageId,
        TransferStatus status)
    {
        var package = GetPackage(packageId);

        if (package is null)
            return;

        package.TransferStatus = status;
    }

    public void Remove(Guid packageId)
    {
        var package = GetPackage(packageId);

        if (package is null)
            return;

        Packages.Remove(package);
    }
}