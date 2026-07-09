using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ImageViewerService
{
    private static readonly Lazy<ImageViewerService> _instance =
        new(() => new ImageViewerService());

    public static ImageViewerService Instance => _instance.Value;

    private ImageViewerService()
    {
    }

    public ImageRecordModel? CurrentImage { get; private set; }

    public event EventHandler? CurrentImageChanged;

    public void OpenImage(ImageRecordModel image)
    {
        ArgumentNullException.ThrowIfNull(image);

        CurrentImage = image;

        CurrentImageChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void CloseImage()
    {
        CurrentImage = null;

        CurrentImageChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public bool HasImage =>
        CurrentImage != null;
}