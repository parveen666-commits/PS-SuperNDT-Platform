using System.Windows;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Dialogs;

public partial class JobDialog : Window
{
    public JobDialog()
    {
        InitializeComponent();

        DataContext = new JobDialogViewModel();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}