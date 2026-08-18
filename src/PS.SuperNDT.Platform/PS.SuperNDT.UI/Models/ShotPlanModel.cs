using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.SuperNDT.UI.Models;

public sealed class ShotPlanModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

    public string PipeId { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public double PipeLengthMm { get; set; }

    public double ShotLengthMm { get; set; } = 300;

    public double OverlapMm { get; set; } = 10;

    public double StepLengthMm =>
        ShotLengthMm > OverlapMm
            ? ShotLengthMm - OverlapMm
            : 0;

    public int TotalShots { get; set; }

    public int CurrentShotNumber { get; set; }

    public double CurrentStartPositionMm { get; set; }

    public double CurrentEndPositionMm { get; set; }

    public bool RulerEnabled { get; set; } = true;

    public bool PipeIdOverlayEnabled { get; set; } = true;

    public string Direction { get; set; } =
        "LeftToRight";

    public string AcquisitionMode { get; set; } =
        "Manual";

    public string Status { get; set; } =
        "Ready";

    public DateTime CreatedOn { get; set; } =
        DateTime.Now;

    public DateTime? CompletedOn { get; set; }

    public bool IsCompleted { get; set; }

    public List<ShotPlanItemModel> Shots { get; set; } =
        new();

    public ShotPlanItemModel? CurrentShot =>
        Shots.FirstOrDefault(
            shot =>
                shot.ShotNumber ==
                CurrentShotNumber);

    public int CompletedShotCount =>
        Shots.Count(
            shot =>
                shot.IsCaptured);

    public int AcceptedShotCount =>
        Shots.Count(
            shot =>
                shot.IsAccepted);

    public int ReviewedShotCount =>
        Shots.Count(
            shot =>
                shot.IsReviewed);

    public double ProgressPercentage
    {
        get
        {
            if (TotalShots <= 0)
            {
                return 0;
            }

            return Math.Clamp(
                (double)CompletedShotCount /
                TotalShots *
                100.0,
                0,
                100);
        }
    }

    public bool HasShots =>
        Shots.Count > 0;

    public bool HasCurrentShot =>
        CurrentShot != null;

    public void SetShots(
        IEnumerable<ShotPlanItemModel> shots)
    {
        Shots =
            shots?
                .OrderBy(
                    shot =>
                        shot.ShotNumber)
                .ToList()
            ?? new List<ShotPlanItemModel>();

        TotalShots =
            Shots.Count;

        if (Shots.Count > 0)
        {
            CurrentShotNumber =
                Shots[0].ShotNumber;

            CurrentStartPositionMm =
                Shots[0].StartPositionMm;

            CurrentEndPositionMm =
                Shots[0].EndPositionMm;
        }
        else
        {
            CurrentShotNumber = 0;
            CurrentStartPositionMm = 0;
            CurrentEndPositionMm = 0;
        }

        IsCompleted = false;
        CompletedOn = null;
        Status =
            Shots.Count > 0
                ? "Ready"
                : "Empty";
    }

    public bool MoveToShot(
        int shotNumber)
    {
        ShotPlanItemModel? shot =
            Shots.FirstOrDefault(
                item =>
                    item.ShotNumber ==
                    shotNumber);

        if (shot == null)
        {
            return false;
        }

        CurrentShotNumber =
            shot.ShotNumber;

        CurrentStartPositionMm =
            shot.StartPositionMm;

        CurrentEndPositionMm =
            shot.EndPositionMm;

        return true;
    }

    public bool MoveToNextPendingShot()
    {
        ShotPlanItemModel? shot =
            Shots
                .Where(
                    item =>
                        !item.IsCaptured)
                .OrderBy(
                    item =>
                        item.ShotNumber)
                .FirstOrDefault();

        if (shot == null)
        {
            IsCompleted = true;
            Status = "Completed";
            CompletedOn ??= DateTime.Now;
            return false;
        }

        CurrentShotNumber =
            shot.ShotNumber;

        CurrentStartPositionMm =
            shot.StartPositionMm;

        CurrentEndPositionMm =
            shot.EndPositionMm;

        Status = "Acquisition";

        return true;
    }

    public void MarkCurrentShotCaptured(
        Guid? imageId,
        string imageFileName)
    {
        ShotPlanItemModel? shot =
            CurrentShot;

        if (shot == null)
        {
            return;
        }

        shot.IsCaptured = true;

        shot.ImageId = imageId;

        shot.ImageFileName =
            imageFileName ?? string.Empty;

        shot.CapturedOn =
            DateTime.Now;

        shot.Status = "Captured";

        if (CompletedShotCount >= TotalShots &&
            TotalShots > 0)
        {
            IsCompleted = true;
            Status = "Completed";
            CompletedOn ??= DateTime.Now;
            return;
        }

        MoveToNextPendingShot();
    }
}