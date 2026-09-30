using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class JobDialogViewModel : INotifyPropertyChanged
{
    private string _jobNumber = "";
    private string _customer = "";
    private string _project = "";
    private string _component = "";
    private string _weldNumber = "";
    private string _operator = "";
    private string _inspector = "";
    private string _procedure = "";
    private string _technique = "";
    private string _material = "";
    private string _materialSpecification = "";
    private string _drawingNumber = "";
    private string _purchaseOrder = "";
    private string _inspectionStandard = "";
    private string _acceptanceStandard = "";
    private string _remarks = "";

    private double _nominalThicknessMm;
    private double _pipeDiameterMm;
    private double _pipeLengthMm;
    private double _defaultShotSizeMm = 300;
    private double _defaultOverlapPercent = 10;

    private DateTime _jobDate = DateTime.Today;

    public string JobNumber
    {
        get => _jobNumber;
        set
        {
            _jobNumber = value;
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

    public string Project
    {
        get => _project;
        set
        {
            _project = value;
            OnPropertyChanged();
        }
    }

    public string Component
    {
        get => _component;
        set
        {
            _component = value;
            OnPropertyChanged();
        }
    }

    public string WeldNumber
    {
        get => _weldNumber;
        set
        {
            _weldNumber = value;
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

    public string Inspector
    {
        get => _inspector;
        set
        {
            _inspector = value;
            OnPropertyChanged();
        }
    }

    public string Procedure
    {
        get => _procedure;
        set
        {
            _procedure = value;
            OnPropertyChanged();
        }
    }

    public string Technique
    {
        get => _technique;
        set
        {
            _technique = value;
            OnPropertyChanged();
        }
    }

    public string Material
    {
        get => _material;
        set
        {
            _material = value;
            OnPropertyChanged();
        }
    }

    public string MaterialSpecification
    {
        get => _materialSpecification;
        set
        {
            _materialSpecification = value;
            OnPropertyChanged();
        }
    }

    public string DrawingNumber
    {
        get => _drawingNumber;
        set
        {
            _drawingNumber = value;
            OnPropertyChanged();
        }
    }

    public string PurchaseOrder
    {
        get => _purchaseOrder;
        set
        {
            _purchaseOrder = value;
            OnPropertyChanged();
        }
    }

    public string InspectionStandard
    {
        get => _inspectionStandard;
        set
        {
            _inspectionStandard = value;
            OnPropertyChanged();
        }
    }

    public string AcceptanceStandard
    {
        get => _acceptanceStandard;
        set
        {
            _acceptanceStandard = value;
            OnPropertyChanged();
        }
    }

    public double NominalThicknessMm
    {
        get => _nominalThicknessMm;
        set
        {
            _nominalThicknessMm = value;
            OnPropertyChanged();
        }
    }

    public double PipeDiameterMm
    {
        get => _pipeDiameterMm;
        set
        {
            _pipeDiameterMm = value;
            OnPropertyChanged();
        }
    }

    public double PipeLengthMm
    {
        get => _pipeLengthMm;
        set
        {
            _pipeLengthMm = value;
            OnPropertyChanged();
        }
    }

    public double DefaultShotSizeMm
    {
        get => _defaultShotSizeMm;
        set
        {
            _defaultShotSizeMm = value;
            OnPropertyChanged();
        }
    }

    public double DefaultOverlapPercent
    {
        get => _defaultOverlapPercent;
        set
        {
            _defaultOverlapPercent = value;
            OnPropertyChanged();
        }
    }

    public DateTime JobDate
    {
        get => _jobDate;
        set
        {
            _jobDate = value;
            OnPropertyChanged();
        }
    }

    public string Remarks
    {
        get => _remarks;
        set
        {
            _remarks = value;
            OnPropertyChanged();
        }
    }

    public JobDialogViewModel()
    {
        JobNumber = $"WO-{DateTime.Now:yyyyMMdd-HHmmss}";
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