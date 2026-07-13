using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ImageReviewView : UserControl
{
    public ImageReviewView()
    {
        InitializeComponent();

        DataContext = new ImageReviewViewModel();
    }
}