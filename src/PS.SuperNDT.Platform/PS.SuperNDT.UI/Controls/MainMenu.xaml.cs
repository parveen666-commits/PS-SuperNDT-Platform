using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.Dialogs;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI.Controls;

public partial class MainMenu : UserControl
{
    public MainMenu()
    {
        InitializeComponent();

        NewJobMenuItem.Click += NewJobMenuItem_Click;
        OpenJobMenuItem.Click += OpenJobMenuItem_Click;
        CloseJobMenuItem.Click += CloseJobMenuItem_Click;
        ExitMenuItem.Click += ExitMenuItem_Click;
    }

    private void NewJobMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
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

    private void ExitMenuItem_Click(
        object sender,
        RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}