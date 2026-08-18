using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
            if (ReferenceEquals(
                    _selectedImage,
                    value))
            {
                return;
            }

            _selectedImage =
                value;

            OnPropertyChanged();

            if (value != null)
            {
                ImageViewerService.Instance.OpenImage(
                    value);
            }
            else
            {
                ImageViewerService.Instance.Clear();
            }
        }
    }

    public ReviewViewModel()
    {
        LoadImages();

        ImageViewerService.Instance.CurrentImageChanged +=
            ImageViewerService_CurrentImageChanged;

        var currentImage =
            ImageViewerService.Instance.CurrentImage;

        if (currentImage != null)
        {
            _selectedImage =
                currentImage;

            OnPropertyChanged(
                nameof(SelectedImage));
        }
        else
        {
            SelectFirstAvailableImage();
        }
    }

    private void LoadImages()
    {
        Images.Clear();

        var records =
            _imageService.GetAll();

        foreach (var record in records)
        {
            Images.Add(record);
        }
    }

    private void SelectFirstAvailableImage()
    {
        if (Images.Count == 0)
        {
            SelectedImage = null;
            return;
        }

        foreach (var image in Images)
        {
            if (IsImageFileAvailable(image))
            {
                SelectedImage = image;
                return;
            }
        }

        /*
         * If the database contains records but the
         * physical image file is not currently found,
         * still select the first database record so
         * the user can see its information and path.
         */

        SelectedImage =
            Images[0];
    }

    private static bool IsImageFileAvailable(
        ImageRecordModel image)
    {
        if (string.IsNullOrWhiteSpace(
                image.FilePath))
        {
            return false;
        }

        try
        {
            return File.Exists(
                image.FilePath);
        }
        catch
        {
            return false;
        }
    }

    private void ImageViewerService_CurrentImageChanged(
        object? sender,
        System.EventArgs e)
    {
        var currentImage =
            ImageViewerService.Instance.CurrentImage;

        if (ReferenceEquals(
                _selectedImage,
                currentImage))
        {
            return;
        }

        _selectedImage =
            currentImage;

        OnPropertyChanged(
            nameof(SelectedImage));
    }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}