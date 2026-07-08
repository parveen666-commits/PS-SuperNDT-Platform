using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CurrentJobService
{
    private static readonly Lazy<CurrentJobService> _instance =
        new(() => new CurrentJobService());

    public static CurrentJobService Instance => _instance.Value;

    private CurrentJobService()
    {
    }

    public JobModel? CurrentJob { get; private set; }

    public bool HasCurrentJob => CurrentJob != null;

    public event EventHandler? CurrentJobChanged;

    public void SetCurrentJob(JobModel job)
    {
        CurrentJob = job;
        CurrentJobChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CloseCurrentJob()
    {
        CurrentJob = null;
        CurrentJobChanged?.Invoke(this, EventArgs.Empty);
    }
}