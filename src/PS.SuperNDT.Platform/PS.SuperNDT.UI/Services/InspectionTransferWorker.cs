using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class InspectionTransferWorker
{
    private readonly InspectionTransferService _transferService;

    public InspectionTransferWorker(InspectionTransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task TransferAsync(
        InspectionTransferModel item,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _transferService.MarkSending(item.Id);

            await Task.Delay(500, cancellationToken);

            if (string.IsNullOrWhiteSpace(item.Destination))
                throw new InvalidOperationException("Destination path not configured.");

            if (!Directory.Exists(item.Destination))
                Directory.CreateDirectory(item.Destination);

            if (File.Exists(item.ImagePath))
            {
                var destinationFile =
                    Path.Combine(
                        item.Destination,
                        Path.GetFileName(item.ImagePath));

                File.Copy(
                    item.ImagePath,
                    destinationFile,
                    true);
            }

            _transferService.MarkSent(item.Id);
        }
        catch (Exception ex)
        {
            _transferService.MarkFailed(item.Id, ex.Message);
        }
    }
}