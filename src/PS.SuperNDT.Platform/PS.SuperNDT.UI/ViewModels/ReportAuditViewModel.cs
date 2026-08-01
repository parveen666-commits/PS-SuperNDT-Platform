using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportAuditViewModel : INotifyPropertyChanged
{
    private readonly ReportAuditService _auditService;


    public ObservableCollection<ReportAuditModel> AuditLogs { get; } =
        new();


    public ReportAuditViewModel()
    {
        _auditService =
            new ReportAuditService();

        Load();
    }


    private void Load()
    {
        AuditLogs.Clear();


        foreach (var item in _auditService.GetAll())
        {
            AuditLogs.Add(item);
        }
    }


    public void AddLog(
        ReportAuditModel audit)
    {
        _auditService.Record(
            audit.ReportId,
            audit.ReportNumber,
            audit.Action,
            audit.Description,
            audit.PerformedBy);

        Load();
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