using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ProjectService
{
    public void Save(ProjectModel project)
    {
        ArgumentNullException.ThrowIfNull(project);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<ProjectModel>()
            .FirstOrDefault(x => x.Id == project.Id);

        if (existing == null)
        {
            db.Set<ProjectModel>().Add(project);
        }
        else
        {
            db.Entry(existing)
              .CurrentValues
              .SetValues(project);
        }

        db.SaveChanges();
    }

    public ProjectModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ProjectModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<ProjectModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ProjectModel>()
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .ToList();
    }

    public List<ProjectModel> Search(string text)
    {
        text ??= string.Empty;

        using var db = new SuperNDTDbContext();

        return db.Set<ProjectModel>()
            .AsNoTracking()
            .Where(x =>
                x.ProjectName.Contains(text) ||
                x.ProjectCode.Contains(text) ||
                x.CustomerName.Contains(text))
            .OrderBy(x => x.ProjectName)
            .ToList();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var project = db.Set<ProjectModel>()
            .FirstOrDefault(x => x.Id == id);

        if (project == null)
            return;

        db.Set<ProjectModel>().Remove(project);

        db.SaveChanges();
    }
}