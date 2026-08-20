using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
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

        db.Initialize();

        ShowDatabaseDiagnostics();
    }

    private static void ShowDatabaseDiagnostics()
    {
        var databasePath =
            System.IO.Path.GetFullPath(
                "PS_SuperNDT.db");

        var builder =
            new StringBuilder();

        builder.AppendLine(
            "PS SuperNDT Database Diagnostic");

        builder.AppendLine();

        builder.AppendLine(
            "Actual DB Path:");

        builder.AppendLine(
            databasePath);

        builder.AppendLine();

        builder.AppendLine(
            "DB Exists:");

        builder.AppendLine(
            System.IO.File.Exists(databasePath)
                ? "YES"
                : "NO");

        builder.AppendLine();

        builder.AppendLine(
            "Images Columns:");

        var columns =
            new List<string>();

        using var connection =
            new SqliteConnection(
                "Data Source=PS_SuperNDT.db");

        connection.Open();

        using var command =
            connection.CreateCommand();

        command.CommandText =
            "PRAGMA table_info(Images);";

        using var reader =
            command.ExecuteReader();

        while (reader.Read())
        {
            columns.Add(
                reader.GetString(1));
        }

        if (columns.Count == 0)
        {
            builder.AppendLine(
                "Images table NOT FOUND.");
        }
        else
        {
            foreach (var column in columns)
            {
                builder.AppendLine(
                    " - " + column);
            }
        }

        builder.AppendLine();

        builder.AppendLine(
            "Overlap Column:");

        builder.AppendLine(
            columns.Any(
                x => string.Equals(
                    x,
                    "Overlap",
                    StringComparison.OrdinalIgnoreCase))
                ? "FOUND"
                : "MISSING");

        MessageBox.Show(
            builder.ToString(),
            "PS SuperNDT DB Diagnostic",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
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