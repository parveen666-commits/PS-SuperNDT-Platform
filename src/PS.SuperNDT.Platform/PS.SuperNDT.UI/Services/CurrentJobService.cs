using System;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CurrentJobService
{
    private static readonly Lazy<CurrentJobService> _instance =
        new(() => new CurrentJobService());

    public static CurrentJobService Instance => _instance.Value;

    private readonly string _stateFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "CurrentJob.txt");

    private CurrentJobService()
    {
        RestoreCurrentJob();
    }

    public JobModel? CurrentJob { get; private set; }

    public bool HasCurrentJob => CurrentJob != null;

    public event EventHandler? CurrentJobChanged;

    public void SetCurrentJob(JobModel job)
    {
        CurrentJob = job;

        File.WriteAllText(
            _stateFile,
            job.Id.ToString());

        CurrentJobChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void CloseCurrentJob()
    {
        CurrentJob = null;

        if (File.Exists(_stateFile))
        {
            File.Delete(_stateFile);
        }

        CurrentJobChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    private void RestoreCurrentJob()
    {
        try
        {
            if (!File.Exists(_stateFile))
                return;

            var text =
                File.ReadAllText(_stateFile);

            if (!Guid.TryParse(text, out var jobId))
                return;

            var service = new JobService();

            CurrentJob = service.Get(jobId);
        }
        catch
        {
            CurrentJob = null;
        }
    }
}