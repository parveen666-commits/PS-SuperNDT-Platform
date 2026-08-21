using System;
using System.Linq;
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

    public JobModel? CurrentJob
    {
        get
        {
            EnsureCurrentJobLoaded();
            return _currentJob;
        }
    }

    public bool HasCurrentJob
    {
        get
        {
            EnsureCurrentJobLoaded();
            return _currentJob != null;
        }
    }

    public bool HasActiveJob
    {
        get
        {
            EnsureCurrentJobLoaded();
            return _currentJob != null &&
                   !_currentJob.IsClosed;
        }
    }

    public void SetCurrentJob(JobModel job)
    {
        ArgumentNullException.ThrowIfNull(job);

        _currentJob = job;

        CurrentJobChanged?.Invoke(
            this,
            _currentJob);
    }

    public void RestoreCurrentJob()
    {
        EnsureCurrentJobLoaded(forceReload: true);
    }

    public void CloseCurrentJob()
    {
        if (_currentJob == null)
            return;

        try
        {
            var jobService = new JobService();

            jobService.CloseJob(_currentJob.Id);

            _currentJob.IsClosed = true;
        }
        catch
        {
            // Keep the current in-memory state consistent
            // even if persistence fails.
            _currentJob.IsClosed = true;
        }

        _currentJob = null;

        CurrentJobChanged?.Invoke(
            this,
            null);
    }

    public void Clear()
    {
        _currentJob = null;

        CurrentJobChanged?.Invoke(
            this,
            null);
    }

    public string GetCurrentJobNumber()
    {
        return CurrentJob?.JobNumber ?? string.Empty;
    }

    private void EnsureCurrentJobLoaded(
        bool forceReload = false)
    {
        if (!forceReload && _currentJob != null)
            return;

        try
        {
            var jobService = new JobService();

            var openJobs = jobService.GetOpenJobs();

            var latestOpenJob = openJobs
                .OrderByDescending(x => x.CreatedOn)
                .FirstOrDefault();

            if (latestOpenJob != null)
            {
                bool changed =
                    _currentJob == null ||
                    _currentJob.Id != latestOpenJob.Id;

                _currentJob = latestOpenJob;

                if (changed)
                {
                    CurrentJobChanged?.Invoke(
                        this,
                        _currentJob);
                }

                return;
            }

            _currentJob = null;
        }
        catch
        {
            // Do not crash the application during startup
            // if the database is temporarily unavailable.
        }
    }
}