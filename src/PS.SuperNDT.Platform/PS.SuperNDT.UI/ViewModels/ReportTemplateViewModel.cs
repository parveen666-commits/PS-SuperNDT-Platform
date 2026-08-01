using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportTemplateViewModel : INotifyPropertyChanged
{
    private readonly ReportTemplateService _templateService;


    public ObservableCollection<string> Templates { get; } =
        new();


    private string _selectedTemplate = string.Empty;


    public string SelectedTemplate
    {
        get => _selectedTemplate;
        set
        {
            if (_selectedTemplate == value)
                return;

            _selectedTemplate = value;
            OnPropertyChanged();
        }
    }


    private string _description = string.Empty;


    public string Description
    {
        get => _description;
        private set
        {
            if (_description == value)
                return;

            _description = value;
            OnPropertyChanged();
        }
    }


    public ReportTemplateViewModel()
    {
        _templateService =
            new ReportTemplateService();

        LoadTemplates();
    }


    private void LoadTemplates()
    {
        Templates.Clear();

        foreach (var template in _templateService.GetAll())
        {
            Templates.Add(template.Key);
        }
    }


    public void SelectTemplate()
    {
        Description =
            _templateService.GetTemplate(
                SelectedTemplate);
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