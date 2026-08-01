using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportArchiveService
{
    private readonly List<TransferArchiveRecordModel> _archiveRecords = new();

    public IReadOnlyList<TransferArchiveRecordModel> GetAll()
    {
        return _archiveRecords;
    }

    public void Add(TransferArchiveRecordModel record)
    {
        ArgumentNullException.ThrowIfNull(record);

        _archiveRecords.Add(record);
    }

    public IEnumerable<TransferArchiveRecordModel> GetByJob(string jobNumber)
    {
        return _archiveRecords.Where(x =>
            string.Equals(x.JobNumber, jobNumber, StringComparison.OrdinalIgnoreCase));
    }

    public bool Verify(string archivePath)
    {
        return File.Exists(archivePath);
    }

    public void Clear()
    {
        _archiveRecords.Clear();
    }
}