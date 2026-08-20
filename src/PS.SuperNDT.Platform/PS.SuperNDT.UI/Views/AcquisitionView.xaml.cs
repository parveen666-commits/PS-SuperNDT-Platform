using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class AcquisitionView : UserControl
{
    public AcquisitionView()
    {
        InitializeComponent();

        DataContext = new AcquisitionViewModel();
    }

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is AcquisitionViewModel viewModel)
        {
            viewModel.SaveCommand.Execute(null);
        }
    }
}