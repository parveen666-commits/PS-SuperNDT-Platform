using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class AutoTransferBackgroundService : IDisposable
{
    private readonly InspectionTransferService _transferService;
    private readonly InspectionTransferWorker _transferWorker;

    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _backgroundTask;

    public bool IsRunning { get; private set; }

    public AutoTransferBackgroundService(
        InspectionTransferService transferService)
    {
        _transferService = transferService;
        _transferWorker = new InspectionTransferWorker(transferService);
    }

    public void Start()
    {
        if (IsRunning)
            return;

        IsRunning = true;

        _cancellationTokenSource = new CancellationTokenSource();

        _backgroundTask = Task.Run(
            () => ProcessQueueAsync(_cancellationTokenSource.Token),
            _cancellationTokenSource.Token);
    }

    public async Task StopAsync()
    {
        if (!IsRunning)
            return;

        IsRunning = false;

        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Cancel();
        }

        if (_backgroundTask is not null)
        {
            try
            {
                await _backgroundTask;
            }
            catch
            {
            }
        }
    }

    private async Task ProcessQueueAsync(
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var pendingItems = _transferService.Queue
                    .Where(x => x.AutoTransfer)
                    .Where(x => x.Status == TransferStatus.Pending)
                    .ToList();

                foreach (var item in pendingItems)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    await _transferWorker.TransferAsync(
                        item,
                        cancellationToken);
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(2),
                    cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}