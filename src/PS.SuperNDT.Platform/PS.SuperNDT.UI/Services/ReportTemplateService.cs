using System.Collections.Generic;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportTemplateService
{
    private readonly Dictionary<string, string> _templates = new();


    public ReportTemplateService()
    {
        RegisterDefaultTemplates();
    }


    private void RegisterDefaultTemplates()
    {
        AddTemplate(
            "RT",
            "Radiographic Testing Inspection Report");


        AddTemplate(
            "UT",
            "Ultrasonic Testing Inspection Report");


        AddTemplate(
            "MT",
            "Magnetic Particle Testing Inspection Report");


        AddTemplate(
            "PT",
            "Liquid Penetrant Testing Inspection Report");
    }


    public void AddTemplate(
        string code,
        string description)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        _templates[code] =
            description;
    }


    public IReadOnlyDictionary<string, string> GetAll()
    {
        return _templates;
    }


    public string GetTemplate(
        string code)
    {
        if (_templates.TryGetValue(
                code,
                out var template))
        {
            return template;
        }

        return string.Empty;
    }
}