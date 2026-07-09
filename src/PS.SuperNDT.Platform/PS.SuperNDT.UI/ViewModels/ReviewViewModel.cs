using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();

    private string _currentJob = "No Active Job";
    private ImageRecordModel? _selectedImage;

    public ObservableCollection<ImageRecordModel> Images { get; } = new();

    public RelayCommand OpenImageCommand { get; }

    public string CurrentJob
    {
        get => _currentJob;
        set
        {
            _currentJob = value;
            OnPropertyChanged();
        }
    }

    public ImageRecordModel? SelectedImage
    {
        get => _selectedImage;
        set
        {
            _selectedImage = value;
            OnPropertyChanged();

            if (_selectedImage != null)
            {
                ImageViewerService.Instance.OpenImage(
                    _selectedImage);
            }
        }
    }

    public ReviewViewModel()
    {
        OpenImageCommand = new RelayCommand(
            _ => OpenSelectedImage());

        LoadImages();

        CurrentJobService.Instance.CurrentJobChanged += (_, _) =>
        {
            LoadImages();
        };
    }

    private void LoadImages()
    {
        Images.Clear();

        var job = CurrentJobService.Instance.CurrentJob;

        if (job == null)
        {
            CurrentJob = "No Active Job";
            return;
        }

        CurrentJob = job.JobNumber;

        foreach (var image in _imageService.GetByJob(job.Id))
        {
            Images.Add(image);
        }
    }

    private void OpenSelectedImage()
    {
        if (SelectedImage == null)
            return;

        ImageViewerService.Instance.OpenImage(
            SelectedImage);
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