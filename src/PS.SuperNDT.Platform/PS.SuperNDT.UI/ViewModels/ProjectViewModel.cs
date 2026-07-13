using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ProjectViewModel : INotifyPropertyChanged
{
    private readonly ProjectService _projectService = new();

    private ProjectModel? _selectedProject;
    private string _searchText = string.Empty;

    public ObservableCollection<ProjectModel> Projects { get; } = new();

    public ProjectModel? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public ProjectViewModel()
    {
        LoadProjects();
    }

    public void LoadProjects()
    {
        Projects.Clear();

        foreach (var project in _projectService.GetAll())
        {
            Projects.Add(project);
        }
    }

    public void Search()
    {
        Projects.Clear();

        foreach (var project in _projectService.Search(SearchText))
        {
            Projects.Add(project);
        }
    }

    public void Save(ProjectModel project)
    {
        _projectService.Save(project);
        LoadProjects();
    }

    public void DeleteSelected()
    {
        if (SelectedProject == null)
            return;

        _projectService.Delete(SelectedProject.Id);

        LoadProjects();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}