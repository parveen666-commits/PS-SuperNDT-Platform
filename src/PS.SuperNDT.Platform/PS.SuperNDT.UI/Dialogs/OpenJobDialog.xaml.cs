using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.Dialogs;

public partial class OpenJobDialog : Window
{
    private readonly JobService _jobService;

    private List<JobModel> _jobs = new();

    public JobModel? SelectedJob { get; private set; }

    public OpenJobDialog()
    {
        InitializeComponent();

        _jobService = new JobService();

        LoadJobs();

        OpenButton.Click += OpenButton_Click;
        CancelButton.Click += CancelButton_Click;
        SearchTextBox.TextChanged += SearchTextBox_TextChanged;
    }

    private void LoadJobs()
    {
        _jobs = _jobService.GetOpenJobs();

        JobsGrid.ItemsSource = _jobs;
    }

    private void SearchTextBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        string text = SearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            JobsGrid.ItemsSource = _jobs;
            return;
        }

        JobsGrid.ItemsSource =
            _jobs.Where(x =>
                    (x.JobNumber?.Contains(text, System.StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Customer?.Contains(text, System.StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Project?.Contains(text, System.StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Component?.Contains(text, System.StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Operator?.Contains(text, System.StringComparison.OrdinalIgnoreCase) ?? false))
                 .ToList();
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        if (JobsGrid.SelectedItem is not JobModel job)
        {
            MessageBox.Show(
                "Please select a job.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        CurrentJobService.Instance.OpenJob(job);

        SelectedJob = job;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}