using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ImageReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageReviewService _service = new();

    private ImageReviewModel? _selectedReview;
    private Guid _currentExposureId;

    public ObservableCollection<ImageReviewModel> Reviews { get; } = new();

    public ImageReviewModel? SelectedReview
    {
        get => _selectedReview;
        set
        {
            _selectedReview = value;
            OnPropertyChanged();
        }
    }

    public Guid CurrentExposureId
    {
        get => _currentExposureId;
        set
        {
            _currentExposureId = value;
            OnPropertyChanged();
        }
    }

    public void Load(Guid exposureId)
    {
        CurrentExposureId = exposureId;

        Reviews.Clear();

        foreach (var review in _service.GetByExposure(exposureId))
        {
            Reviews.Add(review);
        }
    }

    public void AddNew()
    {
        var review = new ImageReviewModel
        {
            ExposureId = CurrentExposureId,
            ImageName = $"IMG-{DateTime.Now:yyyyMMdd-HHmmss}",
            Result = "Pending",
            ReviewDate = DateTime.Now
        };

        _service.Save(review);

        Load(CurrentExposureId);
    }

    public void AcceptSelected()
    {
        if (SelectedReview == null)
            return;

        _service.MarkAccepted(SelectedReview.Id);

        Load(CurrentExposureId);
    }

    public void RejectSelected()
    {
        if (SelectedReview == null)
            return;

        _service.MarkRejected(SelectedReview.Id);

        Load(CurrentExposureId);
    }

    public void DeleteSelected()
    {
        if (SelectedReview == null)
            return;

        _service.Delete(SelectedReview.Id);

        Load(CurrentExposureId);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}