using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ImageReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageReviewService _service = new();
    private readonly RtrReviewFilterService _rtrFilterService = new();
    private readonly AuditLogService _auditLogService = new();
    private readonly ReviewHistoryService _reviewHistoryService = new();

    private readonly UserSessionService _userSessionService =
        UserSessionService.Instance;

    private ImageReviewModel? _selectedReview;
    private ImageRecordModel? _selectedImage;
    private Guid _currentExposureId;

    public ObservableCollection<ImageReviewModel> Reviews { get; }
        = new();

    public ObservableCollection<ImageRecordModel> FilteredImages { get; }
        = new();

    public ObservableCollection<AuditLogModel> ReviewHistory { get; }
        = new();

    public RtrReviewFilterModel RtrFilter { get; }
        = new();

    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }

    public ICommand AcceptSelectedCommand { get; }
    public ICommand RejectSelectedCommand { get; }
    public ICommand HoldSelectedCommand { get; }

    public ICommand RefreshHistoryCommand { get; }

    public ImageReviewModel? SelectedReview
    {
        get => _selectedReview;
        set
        {
            if (ReferenceEquals(
                    _selectedReview,
                    value))
            {
                return;
            }

            _selectedReview = value;

            if (_selectedReview != null)
            {
                _selectedImage = null;
                OnPropertyChanged(
                    nameof(SelectedImage));
            }

            OnPropertyChanged();
            OnPropertyChanged(
                nameof(CanReviewSelected));

            RefreshHistory();
        }
    }

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

            _selectedImage = value;

            if (_selectedImage != null)
            {
                var review =
                    FindReviewForImage(
                        _selectedImage);

                if (review != null)
                {
                    _selectedReview = review;

                    OnPropertyChanged(
                        nameof(SelectedReview));
                }
            }

            OnPropertyChanged();
            OnPropertyChanged(
                nameof(CanReviewSelected));

            RefreshHistory();
        }
    }

    public Guid CurrentExposureId
    {
        get => _currentExposureId;
        set
        {
            if (_currentExposureId == value)
                return;

            _currentExposureId = value;

            OnPropertyChanged();
        }
    }

    public bool CanReviewSelected =>
        SelectedReview != null ||
        SelectedImage != null;

    public ImageReviewViewModel()
    {
        ApplyFilterCommand =
            new DelegateCommand(
                ApplyRtrFilter);

        ClearFilterCommand =
            new DelegateCommand(
                ClearRtrFilter);

        AcceptSelectedCommand =
            new DelegateCommand(
                AcceptSelected);

        RejectSelectedCommand =
            new DelegateCommand(
                RejectSelected);

        HoldSelectedCommand =
            new DelegateCommand(
                HoldSelected);

        RefreshHistoryCommand =
            new DelegateCommand(
                RefreshHistory);
    }

    public void Load(Guid exposureId)
    {
        CurrentExposureId =
            exposureId;

        Reviews.Clear();

        foreach (var review in
                 _service.GetByExposure(
                     exposureId))
        {
            Reviews.Add(review);
        }

        ApplyRtrFilter();

        RefreshHistory();
    }

    public void ApplyRtrFilter()
    {
        FilteredImages.Clear();

        foreach (var image in
                 _rtrFilterService.GetFiltered(
                     RtrFilter))
        {
            FilteredImages.Add(image);
        }

        SelectedImage = null;
        SelectedReview = null;

        RefreshHistory();
    }

    public void ClearRtrFilter()
    {
        RtrFilter.Clear();

        ApplyRtrFilter();
    }

    public void RefreshHistory()
    {
        ReviewHistory.Clear();

        if (SelectedReview != null)
        {
            foreach (var item in
                     _reviewHistoryService.GetByImage(
                         SelectedReview.ImageName))
            {
                ReviewHistory.Add(item);
            }

            return;
        }

        if (SelectedImage != null)
        {
            foreach (var item in
                     _reviewHistoryService.GetByImage(
                         SelectedImage.FileName))
            {
                ReviewHistory.Add(item);
            }

            return;
        }

        foreach (var item in
                 _reviewHistoryService.GetRecent(100))
        {
            ReviewHistory.Add(item);
        }
    }

    public void AddNew()
    {
        var review =
            new ImageReviewModel
            {
                ExposureId =
                    CurrentExposureId,

                ImageName =
                    $"IMG-{DateTime.Now:yyyyMMdd-HHmmss}",

                Result =
                    "PENDING",

                ReviewDate =
                    DateTime.Now,

                Reviewer =
                    GetCurrentUsername(),

                IsReviewed =
                    false,

                IsAccepted =
                    false
            };

        _service.Save(review);

        WriteAudit(
            "CREATE_REVIEW",
            $"Created RTR review record for image {review.ImageName}");

        Load(CurrentExposureId);
    }

    public void AcceptSelected()
    {
        SaveSelectedDecision(
            "ACCEPTED");
    }

    public void RejectSelected()
    {
        SaveSelectedDecision(
            "REJECTED");
    }

    public void HoldSelected()
    {
        SaveSelectedDecision(
            "PENDING");
    }

    private void SaveSelectedDecision(
        string result)
    {
        ImageReviewModel? review =
            SelectedReview;

        if (review == null &&
            SelectedImage != null)
        {
            review =
                FindReviewForImage(
                    SelectedImage);

            if (review == null)
            {
                review =
                    CreateReviewFromImage(
                        SelectedImage);
            }

            SelectedReview =
                review;
        }

        if (review == null)
            return;

        review.Reviewer =
            GetCurrentUsername();

        review.ReviewDate =
            DateTime.Now;

        review.Result =
            result;

        review.IsReviewed =
            result != "PENDING";

        review.IsAccepted =
            result == "ACCEPTED";

        if (string.IsNullOrWhiteSpace(
                review.DefectType))
        {
            review.DefectType =
                RtrFilter.DefectType;
        }

        if (string.IsNullOrWhiteSpace(
                review.AcceptanceCode))
        {
            review.AcceptanceCode =
                RtrFilter.AcceptanceCode;
        }

        _service.SaveReviewDecision(
            review,
            result,
            GetCurrentUsername());

        WriteAudit(
            result == "ACCEPTED"
                ? "ACCEPT_REVIEW"
                : result == "REJECTED"
                    ? "REJECT_REVIEW"
                    : "HOLD_REVIEW",
            $"{result} RTR image {review.ImageName}");

        Load(CurrentExposureId);

        SelectReviewById(
            review.Id);

        RefreshHistory();
    }

    private ImageReviewModel CreateReviewFromImage(
        ImageRecordModel image)
    {
        return new ImageReviewModel
        {
            Id = Guid.NewGuid(),

            ExposureId =
                CurrentExposureId,

            ImageName =
                image.FileName,

            FilePath =
                image.FilePath,

            Reviewer =
                GetCurrentUsername(),

            ReviewDate =
                DateTime.Now,

            Result =
                "PENDING",

            IsReviewed =
                false,

            IsAccepted =
                false,

            DefectType =
                RtrFilter.DefectType,

            AcceptanceCode =
                RtrFilter.AcceptanceCode,

            Remarks =
                string.Empty
        };
    }

    private void SelectReviewById(
        Guid reviewId)
    {
        foreach (var review in Reviews)
        {
            if (review.Id != reviewId)
                continue;

            _selectedReview =
                review;

            OnPropertyChanged(
                nameof(SelectedReview));

            OnPropertyChanged(
                nameof(CanReviewSelected));

            RefreshHistory();

            return;
        }
    }

    public void DeleteSelected()
    {
        if (SelectedReview == null)
            return;

        string imageName =
            SelectedReview.ImageName;

        _service.Delete(
            SelectedReview.Id);

        WriteAudit(
            "DELETE_REVIEW",
            $"Deleted RTR review for image {imageName}");

        Load(CurrentExposureId);
    }

    private ImageReviewModel? FindReviewForImage(
        ImageRecordModel image)
    {
        foreach (var review in Reviews)
        {
            if (string.Equals(
                    review.ImageName,
                    image.FileName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return review;
            }

            if (review.ExposureId == image.JobId &&
                !string.IsNullOrWhiteSpace(
                    review.FilePath) &&
                string.Equals(
                    review.FilePath,
                    image.FilePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                return review;
            }
        }

        return null;
    }

    private string GetCurrentUsername()
    {
        if (string.IsNullOrWhiteSpace(
                _userSessionService.Username))
        {
            return "Guest";
        }

        return _userSessionService.Username;
    }

    private void WriteAudit(
        string action,
        string description)
    {
        try
        {
            _auditLogService.Add(
                GetCurrentUsername(),
                action,
                "RTR REVIEW",
                description);
        }
        catch
        {
            // Review operation must not fail
            // because audit logging failed.
        }
    }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        public DelegateCommand(
            Action execute)
        {
            _execute =
                execute ??
                throw new ArgumentNullException(
                    nameof(execute));
        }

        public bool CanExecute(
            object? parameter)
        {
            return true;
        }

        public void Execute(
            object? parameter)
        {
            _execute();
        }

        public event EventHandler?
            CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }
}