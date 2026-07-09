using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ImageViewerViewModel : INotifyPropertyChanged
{
    private ImageRecordModel? _currentImage;

    public ImageRecordModel? CurrentImage
    {
        get => _currentImage;
        set
        {
            _currentImage = value;
            OnPropertyChanged();
        }
    }

    public string ImageInfo
    {
        get
        {
            if (CurrentImage == null)
                return "No Image Selected";

            return
                $"Frame : {CurrentImage.FrameNumber}\n" +
                $"Size : {CurrentImage.ImageWidth} x {CurrentImage.ImageHeight}\n" +
                $"Bit Depth : {CurrentImage.BitDepth}\n" +
                $"kV : {CurrentImage.KV}\n" +
                $"mA : {CurrentImage.MA}\n" +
                $"Exposure : {CurrentImage.ExposureTime}";
        }
    }

    public ImageViewerViewModel()
    {
        CurrentImage =
            ImageViewerService.Instance.CurrentImage;

        ImageViewerService.Instance.CurrentImageChanged += (_, _) =>
        {
            CurrentImage =
                ImageViewerService.Instance.CurrentImage;

            OnPropertyChanged(nameof(ImageInfo));
        };
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