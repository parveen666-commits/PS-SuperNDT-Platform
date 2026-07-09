using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();

    public ObservableCollection<ImageRecordModel> Images { get; } = new();

    private ImageRecordModel? _selectedImage;

    public ImageRecordModel? SelectedImage
    {
        get => _selectedImage;
        set
        {
            _selectedImage = value;
            OnPropertyChanged();
        }
    }

    public ReviewViewModel()
    {
        LoadImages();

        ImageViewerService.Instance.CurrentImageChanged +=
            ImageViewerService_CurrentImageChanged;
    }

    private void ImageViewerService_CurrentImageChanged(
        object? sender,
        System.EventArgs e)
    {
        LoadImages();

        SelectedImage =
            ImageViewerService.Instance.CurrentImage;
    }

    private void LoadImages()
    {
        Images.Clear();

        var records = _imageService.GetAll();

        foreach (var record in records)
        {
            Images.Add(record);
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