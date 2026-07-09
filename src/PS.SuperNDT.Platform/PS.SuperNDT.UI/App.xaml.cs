using System.Windows;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        InitializeDatabase();
        RestoreLastOpenJob();

        var shell = new ShellWindow();
        shell.Show();
    }

    private static void InitializeDatabase()
    {
        using var db = new SuperNDTDbContext();

        db.Database.EnsureCreated();
    }

    private static void RestoreLastOpenJob()
    {
        var jobService = new JobService();

        var lastOpenJob = jobService.GetOpenJobs()
                                    .FirstOrDefault();

        if (lastOpenJob != null)
        {
            CurrentJobService.Instance.OpenJob(lastOpenJob);
        }
    }
}