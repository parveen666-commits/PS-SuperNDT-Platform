using System;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportNumberService
{
    public string Generate()
    {
        return
            $"PSNDT-RPT-{DateTime.Now:yyyyMMdd-HHmmss}";
    }

    public string GenerateVersion()
    {
        return
            $"V{DateTime.Now:yyyyMMddHHmm}";
    }
}