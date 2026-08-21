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
    private static readonly ErrorLogService ErrorLogger =
        new();

    protected override void OnStartup(
        StartupEventArgs e)
    {
        ShutdownMode =
            ShutdownMode.OnExplicitShutdown;

        DispatcherUnhandledException +=
            App_DispatcherUnhandledException;

        AppDomain.CurrentDomain.UnhandledException +=
            AppDomain_UnhandledException;

        base.OnStartup(e);

        try
        {
            ErrorLogger.Info(
                "Application",
                "PS SuperNDT application startup started.");

            var licenseService =
                new LicenseService();

            if (!licenseService.IsLicenseValid())
            {
                ErrorLogger.Warning(
                    "License",
                    "Application startup stopped because license is invalid.");

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
                ErrorLogger.Info(
                    "Authentication",
                    "Application startup stopped because login was cancelled or unsuccessful.");

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

            ErrorLogger.Info(
                "Application",
                "PS SuperNDT application started successfully.");
        }
        catch (Exception ex)
        {
            ErrorLogger.Fatal(
                "Application Startup",
                "Unhandled exception occurred during application startup.",
                ex);

            ShowStartupError(ex);

            Shutdown();
        }
    }

    private void App_DispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            ErrorLogger.Error(
                "WPF Dispatcher",
                "Unhandled UI exception occurred.",
                e.Exception);
        }
        catch
        {
            // Never allow logging failure to affect exception handling.
        }

        ShowStartupError(
            e.Exception);

        e.Handled =
            true;

        Shutdown();
    }

    private static void AppDomain_UnhandledException(
        object? sender,
        UnhandledExceptionEventArgs e)
    {
        try
        {
            if (e.ExceptionObject is Exception exception)
            {
                ErrorLogger.Fatal(
                    "Application Domain",
                    "Unhandled non-UI application exception occurred.",
                    exception);
            }
            else
            {
                ErrorLogger.Fatal(
                    "Application Domain",
                    "Unhandled application exception occurred.");
            }
        }
        catch
        {
            // Never allow logging failure to affect application shutdown.
        }
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
        try
        {
            using var db =
                new SuperNDTDbContext();

            db.Initialize();

            ErrorLogger.Info(
                "Database",
                "Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            ErrorLogger.Error(
                "Database",
                "Database initialization failed.",
                ex);

            throw;
        }
    }

    private static void RestoreLastOpenJob()
    {
        try
        {
            var jobService =
                new JobService();

            var lastOpenJob =
                jobService
                    .GetOpenJobs()
                    .FirstOrDefault();

            if (lastOpenJob != null)
            {
                CurrentJobService.Instance.SetCurrentJob(
                    lastOpenJob);

                ErrorLogger.Info(
                    "Job",
                    $"Last open job restored: {lastOpenJob.JobNumber}",
                    null,
                    lastOpenJob.JobNumber);
            }
            else
            {
                ErrorLogger.Info(
                    "Job",
                    "No previously open job was found.");
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Error(
                "Job",
                "Failed to restore last open job.",
                ex);

            throw;
        }
    }
}