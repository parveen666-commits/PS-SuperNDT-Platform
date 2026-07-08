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
    private string _procedure = "";
    private string _material = "";
    private string _remarks = "";

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

    public string Procedure
    {
        get => _procedure;
        set
        {
            _procedure = value;
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
        JobNumber = $"JOB-{DateTime.Now:yyyyMMdd-HHmmss}";
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