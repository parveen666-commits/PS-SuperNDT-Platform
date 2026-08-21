using System;
using System.IO;
using System.Text;

namespace PS.SuperNDT.UI.Services;

public sealed class ErrorLogService
{
    private static readonly object _lock = new();

    private readonly string _logDirectory;

    public ErrorLogService()
    {
        _logDirectory =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs");

        EnsureLogDirectory();
    }

    public void Info(
        string module,
        string message,
        string? user = null,
        string? jobNumber = null)
    {
        Write(
            "INFO",
            module,
            message,
            null,
            user,
            jobNumber);
    }

    public void Warning(
        string module,
        string message,
        string? user = null,
        string? jobNumber = null)
    {
        Write(
            "WARNING",
            module,
            message,
            null,
            user,
            jobNumber);
    }

    public void Error(
        string module,
        string message,
        Exception? exception = null,
        string? user = null,
        string? jobNumber = null)
    {
        Write(
            "ERROR",
            module,
            message,
            exception,
            user,
            jobNumber);
    }

    public void Fatal(
        string module,
        string message,
        Exception? exception = null,
        string? user = null,
        string? jobNumber = null)
    {
        Write(
            "FATAL",
            module,
            message,
            exception,
            user,
            jobNumber);
    }

    private void Write(
        string level,
        string module,
        string message,
        Exception? exception,
        string? user,
        string? jobNumber)
    {
        try
        {
            EnsureLogDirectory();

            var logFile =
                Path.Combine(
                    _logDirectory,
                    $"PS-SuperNDT-{DateTime.Now:yyyy-MM-dd}.log");

            var builder =
                new StringBuilder();

            builder.AppendLine(
                "============================================================");

            builder.AppendLine(
                $"Timestamp   : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

            builder.AppendLine(
                $"Level       : {level}");

            builder.AppendLine(
                $"Module      : {module}");

            builder.AppendLine(
                $"User        : {GetValue(user)}");

            builder.AppendLine(
                $"Job/WorkOrder: {GetValue(jobNumber)}");

            builder.AppendLine(
                $"Machine     : {Environment.MachineName}");

            builder.AppendLine(
                $"Message     : {message}");

            if (exception != null)
            {
                builder.AppendLine(
                    $"Exception   : {exception.GetType().FullName}");

                builder.AppendLine(
                    $"ExceptionMsg: {exception.Message}");

                if (exception.InnerException != null)
                {
                    builder.AppendLine(
                        $"InnerError  : {exception.InnerException.Message}");
                }

                builder.AppendLine(
                    "StackTrace  :");

                builder.AppendLine(
                    exception.StackTrace ?? "Not available");
            }

            builder.AppendLine(
                "============================================================");

            builder.AppendLine();

            lock (_lock)
            {
                File.AppendAllText(
                    logFile,
                    builder.ToString(),
                    Encoding.UTF8);
            }
        }
        catch
        {
            // Error logging must never crash the application.
        }
    }

    private void EnsureLogDirectory()
    {
        try
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(
                    _logDirectory);
            }
        }
        catch
        {
            // Logging failure must never crash the application.
        }
    }

    private static string GetValue(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "-"
            : value;
    }
}