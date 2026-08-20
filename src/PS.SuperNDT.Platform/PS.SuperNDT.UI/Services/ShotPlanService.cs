using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ShotPlanService
{
    private readonly ShotPlanCalculatorService _calculator;

    public ShotPlanService()
    {
        _calculator =
            new ShotPlanCalculatorService();
    }

    public ShotPlanModel CreatePlan(
        Guid jobId,
        string pipeId,
        string weldNumber,
        double pipeLengthMm,
        double shotLengthMm,
        double overlapMm,
        bool rulerEnabled = true,
        bool pipeIdOverlayEnabled = true,
        string acquisitionMode = "Manual",
        string direction = "LeftToRight")
    {
        ValidatePlanInputs(
            pipeId,
            pipeLengthMm,
            shotLengthMm,
            overlapMm,
            acquisitionMode,
            direction);

        var plan =
            new ShotPlanModel
            {
                JobId =
                    jobId,

                PipeId =
                    pipeId.Trim(),

                WeldNumber =
                    weldNumber?.Trim()
                    ?? string.Empty,

                PipeLengthMm =
                    pipeLengthMm,

                ShotLengthMm =
                    shotLengthMm,

                OverlapMm =
                    overlapMm,

                RulerEnabled =
                    rulerEnabled,

                PipeIdOverlayEnabled =
                    pipeIdOverlayEnabled,

                AcquisitionMode =
                    acquisitionMode,

                Direction =
                    direction,

                Status =
                    "Ready",

                IsCompleted =
                    false
            };

        var shots =
            _calculator.BuildShotPlanItems(
                plan.Id,
                jobId,
                plan.PipeId,
                plan.WeldNumber,
                pipeLengthMm,
                shotLengthMm,
                overlapMm,
                acquisitionMode);

        foreach (ShotPlanItemModel shot in shots)
        {
            shot.AcquisitionMode =
                acquisitionMode;

            shot.Status =
                "Pending";
        }

        plan.SetShots(
            shots);

        return plan;
    }

    public bool ValidatePlan(
        ShotPlanModel plan,
        out string error)
    {
        error =
            string.Empty;

        if (plan == null)
        {
            error =
                "Shot plan is required.";

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                plan.PipeId))
        {
            error =
                "Pipe ID is required.";

            return false;
        }

        if (plan.PipeLengthMm <= 0)
        {
            error =
                "Pipe length must be greater than zero.";

            return false;
        }

        if (plan.ShotLengthMm <= 0)
        {
            error =
                "Shot length must be greater than zero.";

            return false;
        }

        if (plan.OverlapMm < 0)
        {
            error =
                "Overlap cannot be negative.";

            return false;
        }

        if (plan.OverlapMm >=
            plan.ShotLengthMm)
        {
            error =
                "Overlap must be smaller than shot length.";

            return false;
        }

        if (plan.TotalShots <= 0 ||
            plan.Shots.Count == 0)
        {
            error =
                "Shot plan does not contain any shots.";

            return false;
        }

        return true;
    }

    public bool TryGetCurrentShot(
        ShotPlanModel plan,
        out ShotPlanItemModel? shot)
    {
        shot =
            null;

        if (plan == null)
        {
            return false;
        }

        shot =
            plan.CurrentShot;

        return shot != null;
    }

    public bool MoveToNextShot(
        ShotPlanModel plan)
    {
        if (plan == null)
        {
            return false;
        }

        return plan.MoveToNextPendingShot();
    }

    public bool MoveToShot(
        ShotPlanModel plan,
        int shotNumber)
    {
        if (plan == null)
        {
            return false;
        }

        return plan.MoveToShot(
            shotNumber);
    }

    private static void ValidatePlanInputs(
        string pipeId,
        double pipeLengthMm,
        double shotLengthMm,
        double overlapMm,
        string acquisitionMode,
        string direction)
    {
        if (string.IsNullOrWhiteSpace(
                pipeId))
        {
            throw new ArgumentException(
                "Pipe ID is required.",
                nameof(pipeId));
        }

        if (double.IsNaN(pipeLengthMm) ||
            double.IsInfinity(pipeLengthMm) ||
            pipeLengthMm <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pipeLengthMm),
                "Pipe length must be greater than zero.");
        }

        if (double.IsNaN(shotLengthMm) ||
            double.IsInfinity(shotLengthMm) ||
            shotLengthMm <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(shotLengthMm),
                "Shot length must be greater than zero.");
        }

        if (double.IsNaN(overlapMm) ||
            double.IsInfinity(overlapMm) ||
            overlapMm < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(overlapMm),
                "Overlap cannot be negative.");
        }

        if (overlapMm >= shotLengthMm)
        {
            throw new ArgumentException(
                "Overlap must be smaller than shot length.",
                nameof(overlapMm));
        }

        if (string.IsNullOrWhiteSpace(
                acquisitionMode))
        {
            throw new ArgumentException(
                "Acquisition mode is required.",
                nameof(acquisitionMode));
        }

        if (string.IsNullOrWhiteSpace(
                direction))
        {
            throw new ArgumentException(
                "Direction is required.",
                nameof(direction));
        }
    }
}