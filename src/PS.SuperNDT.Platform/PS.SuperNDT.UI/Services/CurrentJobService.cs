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
    private WorkOrderModel? _currentWorkOrder;

    public event EventHandler<JobModel?>? CurrentJobChanged;

    public event EventHandler<WorkOrderModel?>? CurrentWorkOrderChanged;

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

    public WorkOrderModel? CurrentWorkOrder
    {
        get
        {
            EnsureCurrentJobLoaded();
            return _currentWorkOrder;
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

    public bool HasCurrentWorkOrder
    {
        get
        {
            EnsureCurrentJobLoaded();
            return _currentWorkOrder != null;
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

    public bool HasActiveWorkOrder
    {
        get
        {
            EnsureCurrentJobLoaded();

            return _currentWorkOrder != null &&
                   !string.Equals(
                       _currentWorkOrder.Status,
                       "CLOSED",
                       StringComparison.OrdinalIgnoreCase);
        }
    }

    public void SetCurrentJob(JobModel job)
    {
        ArgumentNullException.ThrowIfNull(job);

        _currentJob = job;

        LoadWorkOrderForCurrentJob();

        CurrentJobChanged?.Invoke(
            this,
            _currentJob);

        CurrentWorkOrderChanged?.Invoke(
            this,
            _currentWorkOrder);
    }

    public void SetCurrentWorkOrder(WorkOrderModel workOrder)
    {
        ArgumentNullException.ThrowIfNull(workOrder);

        _currentWorkOrder = workOrder;

        CurrentWorkOrderChanged?.Invoke(
            this,
            _currentWorkOrder);

        LoadLegacyJobForWorkOrder();

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
        if (_currentJob == null &&
            _currentWorkOrder == null)
        {
            return;
        }

        var jobId =
            _currentJob?.Id ??
            _currentWorkOrder?.Id ??
            Guid.Empty;

        try
        {
            if (jobId != Guid.Empty)
            {
                var jobService = new JobService();

                jobService.CloseJob(jobId);
            }

            if (_currentWorkOrder != null)
            {
                var workOrderService = new WorkOrderService();

                workOrderService.Close(
                    _currentWorkOrder.Id);
            }
        }
        catch
        {
            // Keep the current in-memory state consistent
            // even if persistence fails.
        }

        if (_currentJob != null)
        {
            _currentJob.IsClosed = true;
        }

        if (_currentWorkOrder != null)
        {
            _currentWorkOrder.Status = "CLOSED";
            _currentWorkOrder.ClosedOn = DateTime.Now;
        }

        _currentJob = null;
        _currentWorkOrder = null;

        CurrentJobChanged?.Invoke(
            this,
            null);

        CurrentWorkOrderChanged?.Invoke(
            this,
            null);
    }

    public void Clear()
    {
        _currentJob = null;
        _currentWorkOrder = null;

        CurrentJobChanged?.Invoke(
            this,
            null);

        CurrentWorkOrderChanged?.Invoke(
            this,
            null);
    }

    public string GetCurrentJobNumber()
    {
        return CurrentJob?.JobNumber ??
               CurrentWorkOrder?.WorkOrderNumber ??
               string.Empty;
    }

    public string GetCurrentWorkOrderNumber()
    {
        return CurrentWorkOrder?.WorkOrderNumber ??
               CurrentJob?.JobNumber ??
               string.Empty;
    }

    private void EnsureCurrentJobLoaded(
        bool forceReload = false)
    {
        if (!forceReload &&
            _currentJob != null)
        {
            if (_currentWorkOrder == null)
            {
                LoadWorkOrderForCurrentJob();
            }

            return;
        }

        try
        {
            var jobService = new JobService();

            var openJobs = jobService.GetOpenJobs();

            var latestOpenJob = openJobs
                .OrderByDescending(x => x.CreatedOn)
                .FirstOrDefault();

            if (latestOpenJob != null)
            {
                bool jobChanged =
                    _currentJob == null ||
                    _currentJob.Id != latestOpenJob.Id;

                _currentJob = latestOpenJob;

                LoadWorkOrderForCurrentJob();

                if (jobChanged)
                {
                    CurrentJobChanged?.Invoke(
                        this,
                        _currentJob);
                }

                CurrentWorkOrderChanged?.Invoke(
                    this,
                    _currentWorkOrder);

                return;
            }

            _currentJob = null;
            _currentWorkOrder = null;
        }
        catch
        {
            // Do not crash the application during startup
            // if the database is temporarily unavailable.
        }
    }

    private void LoadWorkOrderForCurrentJob()
    {
        if (_currentJob == null)
        {
            _currentWorkOrder = null;
            return;
        }

        try
        {
            var workOrderService = new WorkOrderService();

            _currentWorkOrder =
                workOrderService.Get(
                    _currentJob.Id);

            if (_currentWorkOrder != null)
            {
                SyncLegacyJobFromWorkOrder(
                    _currentWorkOrder);
            }
        }
        catch
        {
            _currentWorkOrder = null;
        }
    }

    private void LoadLegacyJobForWorkOrder()
    {
        if (_currentWorkOrder == null)
        {
            _currentJob = null;
            return;
        }

        try
        {
            var jobService = new JobService();

            _currentJob =
                jobService.Get(
                    _currentWorkOrder.Id);

            if (_currentJob == null)
            {
                _currentJob = CreateLegacyJobFromWorkOrder(
                    _currentWorkOrder);

                jobService.Save(_currentJob);
            }
        }
        catch
        {
            // Keep Work Order available even if
            // legacy Job synchronization fails.
        }
    }

    private static JobModel CreateLegacyJobFromWorkOrder(
        WorkOrderModel workOrder)
    {
        return new JobModel
        {
            Id = workOrder.Id,
            JobNumber = workOrder.WorkOrderNumber,
            Customer = workOrder.Customer,
            Project = workOrder.Project,
            Component = workOrder.Component,
            Operator = workOrder.AssignedOperator,
            Procedure = workOrder.Procedure,
            Material = workOrder.Material,
            Remark = workOrder.Remark,
            CreatedOn = workOrder.CreatedOn,
            IsClosed =
                string.Equals(
                    workOrder.Status,
                    "CLOSED",
                    StringComparison.OrdinalIgnoreCase)
        };
    }

    private static void SyncLegacyJobFromWorkOrder(
        WorkOrderModel workOrder)
    {
        // This method intentionally contains no database write.
        // The Work Order remains the master record.
    }
}