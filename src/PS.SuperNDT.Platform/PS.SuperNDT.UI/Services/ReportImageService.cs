using System;
using System.Linq;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportImageService
{
    private readonly ObservableCollection<ReportImageModel> _images;

    public ReportImageService()
    {
        _images = new ObservableCollection<ReportImageModel>();
    }

    public ReadOnlyObservableCollection<ReportImageModel> Images =>
        new(_images);

    public ReportImageModel AddImage(
        Guid reportId,
        string imageName,
        string filePath,
        string imageType,
        string description,
        string addedBy)
    {
        var image = new ReportImageModel
        {
            ReportId = reportId,
            ImageName = imageName,
            FilePath = filePath,
            ImageType = imageType,
            Description = description,
            SequenceNumber = _images.Count + 1,
            AddedOn = DateTime.Now,
            AddedBy = addedBy
        };

        _images.Add(image);

        return image;
    }

    public bool RemoveImage(
        Guid imageId)
    {
        var image = _images
            .FirstOrDefault(x => x.Id == imageId);

        if (image == null)
        {
            return false;
        }

        _images.Remove(image);

        return true;
    }
}