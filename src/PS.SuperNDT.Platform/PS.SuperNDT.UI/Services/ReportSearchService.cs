using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportSearchService
{
    private readonly ReportStorageService _storageService;


    public ReportSearchService()
    {
        _storageService =
            new ReportStorageService();
    }


    public IEnumerable<ReportDataModel> Search(
        string searchText)
    {
        var reports =
            _storageService.GetAll();


        if (string.IsNullOrWhiteSpace(searchText))
        {
            return reports;
        }


        return reports.Where(
            x =>
                Contains(
                    x.ReportNumber,
                    searchText)
                ||
                Contains(
                    x.Customer,
                    searchText)
                ||
                Contains(
                    x.Project,
                    searchText)
                ||
                Contains(
                    x.Component,
                    searchText)
                ||
                Contains(
                    x.Operator,
                    searchText));
    }


    private static bool Contains(
        string source,
        string value)
    {
        return source.Contains(
            value,
            StringComparison.OrdinalIgnoreCase);
    }
}