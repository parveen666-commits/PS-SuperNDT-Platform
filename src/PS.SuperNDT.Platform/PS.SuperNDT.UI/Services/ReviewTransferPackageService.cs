using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReviewTransferPackageService
{
    public ObservableCollection<ReviewTransferPackageModel> Packages { get; } = new();

    public ReviewTransferPackageModel CreatePackage(
        InspectionPackageModel sourcePackage,
        string destination)
    {
        var package = new ReviewTransferPackageModel
        {
            Id = Guid.NewGuid(),
            PackageNumber = sourcePackage.PackageNumber,
            JobNumber = sourcePackage.JobNumber,
            InspectionNumber = sourcePackage.InspectionNumber,
            Customer = sourcePackage.Customer,
            Project = sourcePackage.Project,
            Component = sourcePackage.Component,
            WeldNumber = sourcePackage.WeldNumber,
            Technique = sourcePackage.Technique,
            Operator = sourcePackage.Operator,
            CreatedOn = sourcePackage.CreatedOn,
            TransferDate = DateTime.Now,
            TransferDestination = destination,
            ImageCount = sourcePackage.ImageFiles.Count,
            PackageSizeBytes = sourcePackage.TotalSizeBytes,
            ImageFiles = sourcePackage.ImageFiles.ToList(),
            Status = TransferStatus.Pending,
            ReadyForReview = false,
            Notes = sourcePackage.Notes
        };

        Packages.Add(package);

        return package;
    }

    public ReviewTransferPackageModel? Get(Guid id)
    {
        return Packages.FirstOrDefault(x => x.Id == id);
    }

    public void MarkTransferred(Guid id)
    {
        var package = Get(id);

        if (package is null)
            return;

        package.Status = TransferStatus.Sent;
        package.ReadyForReview = true;
    }

    public void MarkFailed(Guid id)
    {
        var package = Get(id);

        if (package is null)
            return;

        package.Status = TransferStatus.Failed;
    }

    public void Remove(Guid id)
    {
        var package = Get(id);

        if (package is null)
            return;

        Packages.Remove(package);
    }
}