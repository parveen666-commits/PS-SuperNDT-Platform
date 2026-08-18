using System;
using System.Collections.Generic;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ShotPlanCalculatorService
{
    public IReadOnlyList<ShotPosition> Calculate(
        double pipeLengthMm,
        double shotLengthMm,
        double overlapMm)
    {
        ValidateInputs(
            pipeLengthMm,
            shotLengthMm,
            overlapMm);

        var shots =
            new List<ShotPosition>();

        double stepLengthMm =
            shotLengthMm - overlapMm;

        int shotNumber = 1;

        double startPositionMm = 0;

        while (true)
        {
            double endPositionMm =
                startPositionMm +
                shotLengthMm;

            if (endPositionMm < pipeLengthMm)
            {
                shots.Add(
                    CreateShot(
                        shotNumber,
                        startPositionMm,
                        endPositionMm,
                        shotLengthMm,
                        overlapMm));

                shotNumber++;

                startPositionMm =
                    stepLengthMm *
                    (shotNumber - 1);

                continue;
            }

            double finalStartPositionMm =
                Math.Max(
                    0,
                    pipeLengthMm -
                    shotLengthMm);

            if (shots.Count == 0 ||
                Math.Abs(
                    shots[^1].StartPositionMm -
                    finalStartPositionMm) >
                0.001)
            {
                shots.Add(
                    CreateShot(
                        shotNumber,
                        finalStartPositionMm,
                        pipeLengthMm,
                        shotLengthMm,
                        overlapMm));
            }
            else
            {
                shots[^1].EndPositionMm =
                    pipeLengthMm;

                shots[^1].ActualCoverageMm =
                    pipeLengthMm -
                    shots[^1].StartPositionMm;

                shots[^1].RulerEndMm =
                    shots[^1].ActualCoverageMm;
            }

            break;
        }

        return shots;
    }

    public IReadOnlyList<ShotPlanItemModel> BuildShotPlanItems(
        Guid shotPlanId,
        Guid jobId,
        string pipeId,
        string weldNumber,
        double pipeLengthMm,
        double shotLengthMm,
        double overlapMm,
        string acquisitionMode = "Manual")
    {
        var calculatedShots =
            Calculate(
                pipeLengthMm,
                shotLengthMm,
                overlapMm);

        var items =
            new List<ShotPlanItemModel>();

        foreach (ShotPosition shot in calculatedShots)
        {
            items.Add(
                new ShotPlanItemModel
                {
                    ShotPlanId = shotPlanId,
                    JobId = jobId,
                    PipeId = pipeId ?? string.Empty,
                    WeldNumber = weldNumber ?? string.Empty,

                    ShotNumber =
                        shot.ShotNumber,

                    StartPositionMm =
                        shot.StartPositionMm,

                    EndPositionMm =
                        shot.EndPositionMm,

                    NominalShotLengthMm =
                        shot.NominalShotLengthMm,

                    ActualCoverageMm =
                        shot.ActualCoverageMm,

                    OverlapMm =
                        shot.OverlapMm,

                    RulerStartMm =
                        shot.RulerStartMm,

                    RulerEndMm =
                        shot.RulerEndMm,

                    AcquisitionMode =
                        string.IsNullOrWhiteSpace(
                            acquisitionMode)
                            ? "Manual"
                            : acquisitionMode,

                    Status = "Pending",

                    IsCaptured = false,

                    IsReviewed = false,

                    IsAccepted = false
                });
        }

        return items;
    }

    public double CalculateStep(
        double shotLengthMm,
        double overlapMm)
    {
        if (shotLengthMm <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(shotLengthMm),
                "Shot length must be greater than zero.");
        }

        if (overlapMm < 0)
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

        return shotLengthMm - overlapMm;
    }

    public int CalculateShotCount(
        double pipeLengthMm,
        double shotLengthMm,
        double overlapMm)
    {
        return Calculate(
            pipeLengthMm,
            shotLengthMm,
            overlapMm).Count;
    }

    private static ShotPosition CreateShot(
        int shotNumber,
        double startPositionMm,
        double endPositionMm,
        double nominalShotLengthMm,
        double overlapMm)
    {
        double actualCoverageMm =
            Math.Max(
                0,
                endPositionMm -
                startPositionMm);

        return new ShotPosition
        {
            ShotNumber = shotNumber,

            StartPositionMm =
                Math.Round(
                    startPositionMm,
                    3),

            EndPositionMm =
                Math.Round(
                    endPositionMm,
                    3),

            NominalShotLengthMm =
                Math.Round(
                    nominalShotLengthMm,
                    3),

            ActualCoverageMm =
                Math.Round(
                    actualCoverageMm,
                    3),

            OverlapMm =
                Math.Round(
                    overlapMm,
                    3),

            RulerStartMm = 0,

            RulerEndMm =
                Math.Round(
                    actualCoverageMm,
                    3)
        };
    }

    private static void ValidateInputs(
        double pipeLengthMm,
        double shotLengthMm,
        double overlapMm)
    {
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
    }

    public sealed class ShotPosition
    {
        public int ShotNumber { get; set; }

        public double StartPositionMm { get; set; }

        public double EndPositionMm { get; set; }

        public double NominalShotLengthMm { get; set; }

        public double ActualCoverageMm { get; set; }

        public double OverlapMm { get; set; }

        public double RulerStartMm { get; set; }

        public double RulerEndMm { get; set; }

        public string PositionText =>
            $"{StartPositionMm:0.###} → {EndPositionMm:0.###} mm";

        public string RulerText =>
            $"{RulerStartMm:0.###} → {RulerEndMm:0.###} mm";
    }
}