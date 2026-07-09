using System.Linq;
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

        var licenseService = new LicenseService();

        if (!licenseService.IsLicenseValid())
        {
            MessageBox.Show(
                "License expired. Please activate your software.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();

            return;
        }

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

        var lastOpenJob =
            jobService.GetOpenJobs()
                      .FirstOrDefault();

        if (lastOpenJob != null)
        {
            CurrentJobService.Instance.SetCurrentJob(
                lastOpenJob);
        }
    }
}