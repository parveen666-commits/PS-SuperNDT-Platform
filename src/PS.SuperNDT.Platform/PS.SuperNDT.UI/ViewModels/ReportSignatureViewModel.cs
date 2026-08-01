using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportSignatureViewModel : INotifyPropertyChanged
{
    private readonly ReportSignatureService _signatureService;


    public ObservableCollection<ReportSignatureModel> Signatures { get; } =
        new();


    private ReportSignatureModel? _selectedSignature;


    public ReportSignatureModel? SelectedSignature
    {
        get => _selectedSignature;
        set
        {
            if (_selectedSignature == value)
                return;

            _selectedSignature = value;
            OnPropertyChanged();
        }
    }


    private string _statusMessage = string.Empty;


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


    public ReportSignatureViewModel()
    {
        _signatureService =
            new ReportSignatureService();

        Load();
    }


    private void Load()
    {
        Signatures.Clear();

        foreach (var item in _signatureService.GetAll())
        {
            Signatures.Add(item);
        }
    }


    public void AddSignature(
        ReportSignatureModel signature)
    {
        _signatureService.Add(signature);

        StatusMessage =
            "Signature added successfully.";

        Load();
    }


    public void VerifySignature()
    {
        if (SelectedSignature == null)
        {
            StatusMessage =
                "Select signature.";

            return;
        }


        _signatureService.Verify(
            SelectedSignature.Id);


        StatusMessage =
            "Signature verified.";

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