using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI;

public partial class App : Application
{
    protected override void OnStartup(
        StartupEventArgs e)
    {
        ShutdownMode =
            ShutdownMode.OnExplicitShutdown;

        DispatcherUnhandledException +=
            App_DispatcherUnhandledException;

        base.OnStartup(e);

        try
        {
            var licenseService =
                new LicenseService();


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


            var loginWindow =
                new Window
                {
                    Title =
                        "PS SuperNDT Login",

                    Width =
                        500,

                    Height =
                        450,

                    WindowStartupLocation =
                        WindowStartupLocation.CenterScreen,

                    ResizeMode =
                        ResizeMode.NoResize,

                    Content =
                        new LoginView()
                };


            bool? loginResult =
                loginWindow.ShowDialog();


            if (loginResult != true)
            {
                Shutdown();

                return;
            }


            var shell =
                new ShellWindow();


            MainWindow =
                shell;


            ShutdownMode =
                ShutdownMode.OnMainWindowClose;


            shell.Show();
        }
        catch (Exception ex)
        {
            ShowStartupError(ex);

            Shutdown();
        }
    }


    private void App_DispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        ShowStartupError(e.Exception);

        e.Handled = true;

        Shutdown();
    }


    private static void ShowStartupError(
        Exception ex)
    {
        MessageBox.Show(
            ex.ToString(),
            "PS SuperNDT Startup Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }


    private static void InitializeDatabase()
    {
        using var db =
            new SuperNDTDbContext();

        db.Database.EnsureCreated();
    }


    private static void RestoreLastOpenJob()
    {
        var jobService =
            new JobService();


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