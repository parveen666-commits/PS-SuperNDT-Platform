using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CurrentJobService
{
    private static readonly Lazy<CurrentJobService> _instance =
        new(() => new CurrentJobService());

    public static CurrentJobService Instance => _instance.Value;

    private JobModel? _currentJob;

    public event EventHandler<JobModel?>? CurrentJobChanged;

    private CurrentJobService()
    {
    }

    public JobModel? CurrentJob => _currentJob;

    public bool HasCurrentJob => _currentJob != null;

    public bool HasActiveJob => _currentJob != null;

    public void SetCurrentJob(JobModel job)
    {
        _currentJob = job;
        CurrentJobChanged?.Invoke(this, _currentJob);
    }

    public void CloseCurrentJob()
    {
        if (_currentJob != null)
        {
            _currentJob.IsClosed = true;
        }

        _currentJob = null;
        CurrentJobChanged?.Invoke(this, null);
    }

    public void Clear()
    {
        _currentJob = null;
        CurrentJobChanged?.Invoke(this, null);
    }

    public string GetCurrentJobNumber()
    {
        return _currentJob?.JobNumber ?? string.Empty;
    }
}