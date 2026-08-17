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
        LogoutMenuItem.Click += LogoutMenuItem_Click;
        ExitMenuItem.Click += ExitMenuItem_Click;
    }

    private void NewJobMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!_authorizationService.CanCreateJob())
        {
            MessageBox.Show(
                "You do not have permission to create a job.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

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
            MessageBox.Show(
                "You do not have permission to open a job.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

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
            MessageBox.Show(
                "You do not have permission to close a job.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

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
}