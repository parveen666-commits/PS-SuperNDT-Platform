using PS.SuperNDT.UI.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class AcquisitionViewModel : INotifyPropertyChanged
{
    private string _jobNumber = "";
    private string _operator = "";
    private string _customer = "";
    private string _partNumber = "";

    private string _detectorStatus = "Disconnected";

    public string JobNumber
    {
        get => _jobNumber;
        set
        {
            _jobNumber = value;
            OnPropertyChanged();
        }
    }

    public string Operator
    {
        get => _operator;
        set
        {
            _operator = value;
            OnPropertyChanged();
        }
    }

    public string Customer
    {
        get => _customer;
        set
        {
            _customer = value;
            OnPropertyChanged();
        }
    }

    public string PartNumber
    {
        get => _partNumber;
        set
        {
            _partNumber = value;
            OnPropertyChanged();
        }
    }

    public string DetectorStatus
    {
        get => _detectorStatus;
        set
        {
            _detectorStatus = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand ConnectCommand { get; }

    public RelayCommand CaptureCommand { get; }

    public RelayCommand SaveCommand { get; }

    public RelayCommand RetakeCommand { get; }

    public AcquisitionViewModel()
    {
        ConnectCommand = new RelayCommand(_ => Connect());

        CaptureCommand = new RelayCommand(_ => Capture());

        SaveCommand = new RelayCommand(_ => Save());

        RetakeCommand = new RelayCommand(_ => Retake());
    }

    private void Connect()
    {
        DetectorStatus = "Connected";

        MessageBox.Show("Detector Connected");
    }

    private void Capture()
    {
        MessageBox.Show("Capture Started");
    }

    private void Save()
    {
        MessageBox.Show("Image Saved");
    }

    private void Retake()
    {
        MessageBox.Show("Ready For Next Shot");
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