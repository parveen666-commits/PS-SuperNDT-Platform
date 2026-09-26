using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public partial class WeldListViewModel : ObservableObject
{
    // ============================================================
    // CURRENT JOB
    // ============================================================

    [ObservableProperty]
    private Guid _jobId;

    [ObservableProperty]
    private string _jobNumber = string.Empty;

    // ============================================================
    // WELD LIST
    // ============================================================

    [ObservableProperty]
    private ObservableCollection<WeldModel> _welds =
        new();

    [ObservableProperty]
    private WeldModel? _selectedWeld;

    // ============================================================
    // NEW WELD INPUT
    // ============================================================

    [ObservableProperty]
    private string _weldNumber = string.Empty;

    [ObservableProperty]
    private string _spoolNumber = string.Empty;

    [ObservableProperty]
    private string _lineNumber = string.Empty;

    [ObservableProperty]
    private string _jointType = string.Empty;

    [ObservableProperty]
    private string _material = string.Empty;

    [ObservableProperty]
    private double _diameter;

    [ObservableProperty]
    private double _thickness;

    [ObservableProperty]
    private string _schedule = string.Empty;

    [ObservableProperty]
    private string _technique = string.Empty;

    [ObservableProperty]
    private string _remarks = string.Empty;

    // ============================================================
    // SUMMARY
    // ============================================================

    public int TotalWelds =>
        Welds.Count;

    public int PendingWelds =>
        Welds.Count(
            weld =>
                string.Equals(
                    weld.InspectionStatus,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase));

    public int AcceptedWelds =>
        Welds.Count(
            weld =>
                string.Equals(
                    weld.InspectionStatus,
                    "Accepted",
                    StringComparison.OrdinalIgnoreCase));

    public int RepairWelds =>
        Welds.Count(
            weld =>
                string.Equals(
                    weld.InspectionStatus,
                    "Repair",
                    StringComparison.OrdinalIgnoreCase));

    public int RejectedWelds =>
        Welds.Count(
            weld =>
                string.Equals(
                    weld.InspectionStatus,
                    "Rejected",
                    StringComparison.OrdinalIgnoreCase));

    // ============================================================
    // CONSTRUCTOR
    // ============================================================

    public WeldListViewModel(
        Guid jobId,
        string jobNumber = "")
    {
        JobId = jobId;
        JobNumber = jobNumber;

        LoadWelds();
    }

    // ============================================================
    // LOAD
    // ============================================================

    [RelayCommand]
    private void LoadWelds()
    {
        if (JobId == Guid.Empty)
        {
            Welds.Clear();
            NotifySummaryChanged();
            return;
        }

        var items =
            new WeldService()
                .GetByJob(JobId);

        Welds =
            new ObservableCollection<WeldModel>(
                items);

        NotifySummaryChanged();
    }

    // ============================================================
    // ADD WELD
    // ============================================================

    [RelayCommand]
    private void AddWeld()
    {
        if (JobId == Guid.Empty)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(
                WeldNumber))
        {
            return;
        }

        var service =
            new WeldService();

        /*
         * Prevent accidental duplicate weld numbers
         * inside the same Work Order.
         */
        bool alreadyExists =
            Welds.Any(
                weld =>
                    string.Equals(
                        weld.WeldNumber?.Trim(),
                        WeldNumber.Trim(),
                        StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            return;
        }

        var weld =
            new WeldModel
            {
                Id =
                    Guid.NewGuid(),

                JobId =
                    JobId,

                WeldNumber =
                    WeldNumber.Trim(),

                SpoolNumber =
                    SpoolNumber.Trim(),

                LineNumber =
                    LineNumber.Trim(),

                JointType =
                    JointType.Trim(),

                Material =
                    Material.Trim(),

                Diameter =
                    Math.Max(
                        0,
                        Diameter),

                Thickness =
                    Math.Max(
                        0,
                        Thickness),

                Schedule =
                    Schedule.Trim(),

                Technique =
                    Technique.Trim(),

                InspectionStatus =
                    "Pending",

                TotalShots =
                    0,

                AcceptedShots =
                    0,

                RejectedShots =
                    0,

                Remarks =
                    Remarks.Trim(),

                CreatedOn =
                    DateTime.Now
            };

        service.Save(weld);

        Welds.Add(weld);

        SelectedWeld =
            weld;

        ClearNewWeldFields();

        NotifySummaryChanged();
    }

    // ============================================================
    // DELETE WELD
    // ============================================================

    [RelayCommand]
    private void DeleteWeld()
    {
        if (SelectedWeld == null)
        {
            return;
        }

        Guid weldId =
            SelectedWeld.Id;

        new WeldService()
            .Delete(weldId);

        Welds.Remove(
            SelectedWeld);

        SelectedWeld = null;

        NotifySummaryChanged();
    }

    // ============================================================
    // SELECT WELD
    // ============================================================

    partial void OnSelectedWeldChanged(
        WeldModel? value)
    {
        OnPropertyChanged(
            nameof(CanDeleteWeld));
    }

    public bool CanDeleteWeld =>
        SelectedWeld != null;

    // ============================================================
    // CLEAR INPUT
    // ============================================================

    private void ClearNewWeldFields()
    {
        WeldNumber =
            string.Empty;

        SpoolNumber =
            string.Empty;

        LineNumber =
            string.Empty;

        JointType =
            string.Empty;

        Material =
            string.Empty;

        Diameter =
            0;

        Thickness =
            0;

        Schedule =
            string.Empty;

        Technique =
            string.Empty;

        Remarks =
            string.Empty;
    }

    // ============================================================
    // SUMMARY NOTIFICATION
    // ============================================================

    private void NotifySummaryChanged()
    {
        OnPropertyChanged(
            nameof(TotalWelds));

        OnPropertyChanged(
            nameof(PendingWelds));

        OnPropertyChanged(
            nameof(AcceptedWelds));

        OnPropertyChanged(
            nameof(RepairWelds));

        OnPropertyChanged(
            nameof(RejectedWelds));
    }
}