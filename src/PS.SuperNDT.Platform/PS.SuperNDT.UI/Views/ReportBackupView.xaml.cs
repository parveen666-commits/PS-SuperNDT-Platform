using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportBackupView : UserControl
{
    public ReportBackupView()
    {
        InitializeComponent();

        DataContext =
            new ReportBackupViewModel();
    }

    private void CreateBackup_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportBackupViewModel viewModel)
        {
            viewModel.CreateBackup();
        }
    }
}