using System.Windows;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var shell = new ShellWindow();
        shell.Show();
    }
}