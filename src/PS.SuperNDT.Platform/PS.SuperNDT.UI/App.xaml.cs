using System;
using System.Linq;
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

        db.Database.EnsureCreated();

        EnsureReviewColumns();
    }

    private static void EnsureReviewColumns()
    {
        const string connectionString =
            "Data Source=PS_SuperNDT.db";

        using var connection =
            new SqliteConnection(connectionString);

        connection.Open();

        AddColumnIfMissing(
            connection,
            "ReviewStatus",
            "TEXT NOT NULL DEFAULT 'PENDING'");

        AddColumnIfMissing(
            connection,
            "ReviewedBy",
            "TEXT NOT NULL DEFAULT ''");

        AddColumnIfMissing(
            connection,
            "ReviewedOn",
            "TEXT NULL");
    }

    private static void AddColumnIfMissing(
        SqliteConnection connection,
        string columnName,
        string columnDefinition)
    {
        using var checkCommand =
            connection.CreateCommand();

        checkCommand.CommandText =
            "SELECT COUNT(*) " +
            "FROM pragma_table_info('Images') " +
            "WHERE name = $columnName;";

        checkCommand.Parameters.AddWithValue(
            "$columnName",
            columnName);

        var exists =
            Convert.ToInt32(
                checkCommand.ExecuteScalar()) > 0;

        if (exists)
            return;

        using var alterCommand =
            connection.CreateCommand();

        alterCommand.CommandText =
            $"ALTER TABLE Images " +
            $"ADD COLUMN {columnName} {columnDefinition};";

        alterCommand.ExecuteNonQuery();
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