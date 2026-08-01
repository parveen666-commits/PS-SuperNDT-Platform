using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportWorkflowViewModel : INotifyPropertyChanged
{
    private readonly ReportWorkflowService _workflowService;

    private string _statusMessage = string.Empty;


    public ReportDataModel CurrentReport { get; }


    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value)
                return;

            _statusMessage = value;
            OnPropertyChanged();
        }
    }


    public ReportWorkflowViewModel()
    {
        _workflowService =
            new ReportWorkflowService();

        CurrentReport =
            new ReportDataModel
            {
                ReportNumber =
                    $"PSNDT-RPT-{System.DateTime.Now:yyyyMMdd-HHmmss}"
            };
    }


    public void CreateReport(
        string user)
    {
        _workflowService.CreateReport(
            CurrentReport,
            user);

        StatusMessage =
            "Report created successfully.";
    }


    public void SubmitApproval(
        string user,
        string level,
        string designation)
    {
        _workflowService.SubmitForApproval(
            CurrentReport,
            user,
            level,
            designation);

        StatusMessage =
            "Report submitted for approval.";
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}