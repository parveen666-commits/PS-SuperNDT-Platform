using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ImageViewerView : UserControl
{
    public ImageViewerView()
    {
        InitializeComponent();

        DataContext = new ImageViewerViewModel();
    }
}