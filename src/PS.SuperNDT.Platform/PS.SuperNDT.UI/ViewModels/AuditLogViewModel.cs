using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public class AuditLogViewModel : INotifyPropertyChanged
{
    private readonly AuditLogService _auditLogService;

    public ObservableCollection<AuditLogModel> Logs { get; } =
        new();

    public AuditLogViewModel()
    {
        _auditLogService = new AuditLogService();

        LoadLogs();
    }

    public void LoadLogs()
    {
        Logs.Clear();

        foreach (var log in _auditLogService.GetRecent(500))
        {
            Logs.Add(log);
        }
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