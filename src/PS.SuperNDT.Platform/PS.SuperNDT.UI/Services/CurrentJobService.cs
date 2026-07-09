using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CurrentJobService
{
    private static readonly Lazy<CurrentJobService> _instance =
        new(() => new CurrentJobService());

    public static CurrentJobService Instance => _instance.Value;

    private readonly JobService _jobService = new();

    private CurrentJobService()
    {
    }

    public JobModel? CurrentJob { get; private set; }

    public bool HasCurrentJob => CurrentJob != null;

    public event EventHandler? CurrentJobChanged;

    public void SetCurrentJob(JobModel job)
    {
        ArgumentNullException.ThrowIfNull(job);

        CurrentJob = job;

        CurrentJobChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void OpenJob(JobModel job)
    {
        ArgumentNullException.ThrowIfNull(job);

        CurrentJob = job;

        CurrentJobChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void CloseCurrentJob()
    {
        if (CurrentJob == null)
            return;

        _jobService.CloseJob(CurrentJob.Id);

        CurrentJob.IsClosed = true;

        CurrentJobChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void ClearCurrentJob()
    {
        CurrentJob = null;

        CurrentJobChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public string GetCurrentJobNumber()
    {
        return CurrentJob?.JobNumber ?? "No Active Job";
    }
}