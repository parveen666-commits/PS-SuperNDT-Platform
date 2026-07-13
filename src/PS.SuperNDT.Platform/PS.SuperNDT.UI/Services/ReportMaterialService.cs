using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportMaterialService
{
    public ReportMaterialModel Create(
        Guid reportId,
        string materialName,
        string materialGrade,
        string specification,
        double thickness,
        string weldingProcess,
        string jointType)
    {
        return new ReportMaterialModel
        {
            ReportId = reportId,
            MaterialName = materialName,
            MaterialGrade = materialGrade,
            MaterialSpecification = specification,
            Thickness = thickness,
            WeldingProcess = weldingProcess,
            JointType = jointType
        };
    }

    public bool Validate(
        ReportMaterialModel material)
    {
        if (material == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(material.MaterialName))
        {
            return false;
        }

        if (material.Thickness <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(material.JointType))
        {
            return false;
        }

        return true;
    }
}