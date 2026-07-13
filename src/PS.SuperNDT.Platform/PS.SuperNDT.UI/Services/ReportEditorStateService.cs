using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportEditorStateService
{
    private ReportEditorStateModel _state;

    public ReportEditorStateService()
    {
        _state = new ReportEditorStateModel();
    }

    public ReportEditorStateModel GetState()
    {
        return _state;
    }

    public void UpdateDraft(
        Guid reportId)
    {
        _state.ReportId = reportId;
        _state.IsDraft = true;
        _state.CurrentStatus = "Draft";
        _state.LastUpdatedOn = DateTime.Now;
    }

    public void MarkValidated()
    {
        _state.IsValidated = true;
        _state.CurrentStatus = "Validated";
        _state.LastUpdatedOn = DateTime.Now;
    }

    public void MarkPreviewGenerated()
    {
        _state.IsPreviewGenerated = true;
        _state.CurrentStatus = "Preview Generated";
        _state.LastUpdatedOn = DateTime.Now;
    }

    public void MarkPdfExported()
    {
        _state.IsPdfExported = true;
        _state.CurrentStatus = "PDF Exported";
        _state.LastUpdatedOn = DateTime.Now;
    }
}