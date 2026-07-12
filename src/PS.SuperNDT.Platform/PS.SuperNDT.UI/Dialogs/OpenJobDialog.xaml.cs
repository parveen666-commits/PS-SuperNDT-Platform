using System.Linq;
using System.Windows;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.Dialogs;

public partial class OpenJobDialog : Window
{
    private readonly JobService _jobService = new();

    public JobModel? SelectedJob { get; private set; }

    public OpenJobDialog()
    {
        InitializeComponent();

        Loaded += OpenJobDialog_Loaded;

        OpenButton.Click += OpenButton_Click;
        CancelButton.Click += CancelButton_Click;
        SearchTextBox.TextChanged += SearchTextBox_TextChanged;
    }

    private void OpenJobDialog_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        LoadJobs();
    }

    private void SearchTextBox_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e)
    {
        var text = SearchTextBox.Text?.Trim() ?? string.Empty;

        JobsGrid.ItemsSource =
            string.IsNullOrWhiteSpace(text)
                ? _jobService.GetAll()
                : _jobService.Search(text);
    }

    private void LoadJobs()
    {
        JobsGrid.ItemsSource =
            _jobService
            .GetAll()
            .ToList();
    }

    private void OpenButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (JobsGrid.SelectedItem is not JobModel job)
        {
            MessageBox.Show(
                "Please select a job.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        SelectedJob = job;

        CurrentJobService.Instance.SetCurrentJob(job);

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}