using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class CalculatorViewModel : INotifyPropertyChanged
{
    private string _material = "Carbon Steel";
    private string _source = "X-Ray";

    private double _thickness;
    private double _sourceDistance = 600;

    private double _exposureTime;

    public RelayCommand CalculateCommand { get; }

    public string Material
    {
        get => _material;
        set
        {
            _material = value;
            OnPropertyChanged();
        }
    }

    public string Source
    {
        get => _source;
        set
        {
            _source = value;
            OnPropertyChanged();
        }
    }

    public double Thickness
    {
        get => _thickness;
        set
        {
            _thickness = value;
            OnPropertyChanged();
        }
    }

    public double SourceDistance
    {
        get => _sourceDistance;
        set
        {
            _sourceDistance = value;
            OnPropertyChanged();
        }
    }

    public double ExposureTime
    {
        get => _exposureTime;
        set
        {
            _exposureTime = value;
            OnPropertyChanged();
        }
    }

    public CalculatorViewModel()
    {
        CalculateCommand =
            new RelayCommand(_ => Calculate());
    }

    private void Calculate()
    {
        double materialFactor = Material switch
        {
            "Stainless Steel" => 1.4,
            "Aluminium" => 0.7,
            _ => 1.0
        };

        double sourceFactor = Source switch
        {
            "Ir-192" => 1.8,
            "Co-60" => 2.5,
            "Se-75" => 1.3,
            _ => 1.0
        };

        ExposureTime =
            Math.Round(
                (Thickness * materialFactor * sourceFactor)
                + (SourceDistance / 1000.0),
                2);
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