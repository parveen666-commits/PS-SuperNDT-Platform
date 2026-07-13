using System.Collections.Generic;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class AerbReportTemplateService
{
    public List<ReportTemplateSectionModel> CreateDefaultTemplate(
        ReportTemplateModel template)
    {
        return new List<ReportTemplateSectionModel>
        {
            new ReportTemplateSectionModel
            {
                TemplateId = template.Id,
                SectionName = "Header",
                DisplayTitle = "Inspection Report Header",
                SequenceNumber = 1,
                IsMandatory = true
            },

            new ReportTemplateSectionModel
            {
                TemplateId = template.Id,
                SectionName = "JobDetails",
                DisplayTitle = "Job & Component Details",
                SequenceNumber = 2,
                IsMandatory = true
            },

            new ReportTemplateSectionModel
            {
                TemplateId = template.Id,
                SectionName = "Technique",
                DisplayTitle = "Radiographic Technique Parameters",
                SequenceNumber = 3,
                IsMandatory = true
            },

            new ReportTemplateSectionModel
            {
                TemplateId = template.Id,
                SectionName = "Findings",
                DisplayTitle = "Inspection Findings",
                SequenceNumber = 4,
                IsMandatory = true
            },

            new ReportTemplateSectionModel
            {
                TemplateId = template.Id,
                SectionName = "Images",
                DisplayTitle = "Radiographic Images",
                SequenceNumber = 5,
                IsMandatory = false
            },

            new ReportTemplateSectionModel
            {
                TemplateId = template.Id,
                SectionName = "Approval",
                DisplayTitle = "Inspector Approval & Signature",
                SequenceNumber = 6,
                IsMandatory = true
            }
        };
    }
}