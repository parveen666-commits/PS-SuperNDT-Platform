using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportBackupViewModel : INotifyPropertyChanged
{
    private readonly ReportBackupService _backupService;

    private string _sourceFile = string.Empty;
    private string _backupFile = string.Empty;
    private string _statusMessage = string.Empty;

    public ReportBackupViewModel()
    {
        _backupService = new ReportBackupService();
    }

    public string SourceFile
    {
        get => _sourceFile;
        set
        {
            if (_sourceFile == value)
                return;

            _sourceFile = value;
            OnPropertyChanged();
        }
    }

    public string BackupFile
    {
        get => _backupFile;
        private set
        {
            if (_backupFile == value)
                return;

            _backupFile = value;
            OnPropertyChanged();
        }
    }

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

    public void CreateBackup()
    {
        if (string.IsNullOrWhiteSpace(SourceFile))
        {
            StatusMessage = "Select report file.";
            return;
        }

        BackupFile =
            _backupService.Backup(SourceFile);

        StatusMessage =
            "Backup created successfully.";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}