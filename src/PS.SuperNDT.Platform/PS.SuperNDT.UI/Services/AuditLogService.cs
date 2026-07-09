using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class AuditLogService
{
    private readonly string _auditFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "auditlog.json");

    public List<AuditLogModel> GetAll()
    {
        try
        {
            if (!File.Exists(_auditFile))
            {
                return new List<AuditLogModel>();
            }

            var json =
                File.ReadAllText(_auditFile);

            var logs =
                JsonSerializer.Deserialize<List<AuditLogModel>>(json);

            return logs ??
                   new List<AuditLogModel>();
        }
        catch
        {
            return new List<AuditLogModel>();
        }
    }

    public void Add(
        string username,
        string action,
        string module,
        string description)
    {
        var logs = GetAll();

        logs.Add(
            new AuditLogModel
            {
                Timestamp = DateTime.Now,
                Username = username,
                Action = action,
                Module = module,
                Description = description,
                MachineName = Environment.MachineName
            });

        Save(logs);
    }

    public List<AuditLogModel> GetRecent(
        int count)
    {
        return GetAll()
            .OrderByDescending(x => x.Timestamp)
            .Take(count)
            .ToList();
    }

    private void Save(
        List<AuditLogModel> logs)
    {
        var json =
            JsonSerializer.Serialize(
                logs,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            _auditFile,
            json);
    }
}