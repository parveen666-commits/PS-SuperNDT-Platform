using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.Dialogs;

namespace PS.SuperNDT.UI.Controls;

public partial class MainMenu : UserControl
{
    public MainMenu()
    {
        InitializeComponent();

        NewJobMenuItem.Click += NewJobMenuItem_Click;
        ExitMenuItem.Click += ExitMenuItem_Click;
    }

    private void NewJobMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new JobDialog
        {
            Owner = Window.GetWindow(this)
        };

        dialog.ShowDialog();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}