using System;
using System.Windows;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ShellWindow : Window
{
    public ShellWindow()
    {
        InitializeComponent();

        try
        {
            DataContext = new ShellViewModel();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Shell Error");
        }
    }
}