using PS.SuperNDT.UI.Dialogs;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.Views;
using System.Windows;
using System.Windows.Controls;

namespace PS.SuperNDT.UI.Controls;

public partial class MainMenu : UserControl
{
    private readonly AuthorizationService _authorizationService;

    public MainMenu()
    {
        InitializeComponent();

        _authorizationService =
            new AuthorizationService(
                new AccessControlService(
                    new UserRoleService()));

        NewJobMenuItem.Click += NewJobMenuItem_Click;
        OpenJobMenuItem.Click += OpenJobMenuItem_Click;
        CloseJobMenuItem.Click += CloseJobMenuItem_Click;

        UserManagementMenuItem.Click +=
            UserManagementMenuItem_Click;

        AuditLogMenuItem.Click +=
            AuditLogMenuItem_Click;

        LogoutMenuItem.Click += LogoutMenuItem_Click;
        ExitMenuItem.Click += ExitMenuItem_Click;
    }

    private void NewJobMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_authorizationService.CanCreateJob())
        {
            ShowAccessDenied(
                "You do not have permission to create a job.");

            return;
        }

        var dialog = new NewJobDialog
        {
            Owner = Window.GetWindow(this)
        };

        dialog.ShowDialog();
    }

    private void OpenJobMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_authorizationService.CanOpenJob())
        {
            ShowAccessDenied(
                "You do not have permission to open a job.");

            return;
        }

        var dialog = new OpenJobDialog
        {
            Owner = Window.GetWindow(this)
        };

        dialog.ShowDialog();
    }

    private void CloseJobMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_authorizationService.CanOpenJob())
        {
            ShowAccessDenied(
                "You do not have permission to close a job.");

            return;
        }

        if (!CurrentJobService.Instance.HasCurrentJob)
        {
            MessageBox.Show(
                "No active job.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var result = MessageBox.Show(
            "Close current job?",
            "PS SuperNDT",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        CurrentJobService.Instance.CloseCurrentJob();

        MessageBox.Show(
            "Job closed successfully.",
            "PS SuperNDT",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void UserManagementMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_authorizationService.CanManageUsers())
        {
            ShowAccessDenied(
                "You do not have permission to manage users.");

            return;
        }

        var window = new Window
        {
            Title = "PS SuperNDT - User Management",
            Width = 1100,
            Height = 700,
            MinWidth = 900,
            MinHeight = 550,
            WindowStartupLocation =
                WindowStartupLocation.CenterOwner,
            Owner = Window.GetWindow(this),
            Content = new UserManagementView()
        };

        window.ShowDialog();
    }

    private void AuditLogMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_authorizationService.CanViewAuditLog())
        {
            ShowAccessDenied(
                "You do not have permission to view the audit log.");

            return;
        }

        var window = new Window
        {
            Title = "PS SuperNDT - Audit Log",
            Width = 1200,
            Height = 700,
            MinWidth = 950,
            MinHeight = 550,
            WindowStartupLocation =
                WindowStartupLocation.CenterOwner,
            Owner = Window.GetWindow(this),
            Content = new AuditLogView()
        };

        window.ShowDialog();
    }

    private void LogoutMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        var username =
            UserSessionService.Instance.Username;

        var result = MessageBox.Show(
            "Do you want to logout from PS SuperNDT?",
            "Logout",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        var auditLogService =
            new AuditLogService();

        auditLogService.Add(
            username,
            "Logout",
            "Security",
            "User logged out successfully.");

        UserSessionService.Instance.Logout();

        var currentWindow =
            Window.GetWindow(this);

        if (currentWindow == null)
        {
            Application.Current.Shutdown();
            return;
        }

        currentWindow.Hide();

        var loginWindow =
            new Window
            {
                Title = "PS SuperNDT Login",
                Width = 500,
                Height = 450,
                WindowStartupLocation =
                    WindowStartupLocation.CenterScreen,
                ResizeMode =
                    ResizeMode.NoResize,
                Content = new LoginView()
            };

        var loginResult =
            loginWindow.ShowDialog();

        if (loginResult == true)
        {
            var shell =
                new ShellWindow();

            Application.Current.MainWindow =
                shell;

            shell.Show();

            currentWindow.Close();

            return;
        }

        Application.Current.Shutdown();
    }

    private void ExitMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private static void ShowAccessDenied(
        string message)
    {
        MessageBox.Show(
            message,
            "Access Denied",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}