using System.Windows;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views
{
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();
            DataContext = new ShellViewModel();
        }
    }
}