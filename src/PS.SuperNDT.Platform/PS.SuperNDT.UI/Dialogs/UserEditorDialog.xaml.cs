using System.Windows;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Dialogs;

public partial class UserEditorDialog : Window
{
    public UserModel User { get; }

    public UserEditorDialog(UserModel? user = null)
    {
        InitializeComponent();

        User = user ?? new UserModel();

        DataContext = User;

        UsernameTextBox.Text = User.Username;
        FullNameTextBox.Text = User.FullName;
        PasswordTextBox.Password = User.Password;
        ActiveCheckBox.IsChecked = User.IsActive;

        RoleComboBox.SelectedItem =
            RoleComboBox.Items
                .OfType<System.Windows.Controls.ComboBoxItem>()
                .FirstOrDefault(x =>
                    string.Equals(
                        x.Content?.ToString(),
                        User.Role.ToString(),
                        System.StringComparison.OrdinalIgnoreCase));
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                UsernameTextBox.Text))
        {
            MessageBox.Show(
                "Username is required.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            UsernameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(
                FullNameTextBox.Text))
        {
            MessageBox.Show(
                "Full Name is required.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            FullNameTextBox.Focus();
            return;
        }

        User.Username =
            UsernameTextBox.Text.Trim();

        User.FullName =
            FullNameTextBox.Text.Trim();

        User.Password =
            PasswordTextBox.Password;

        User.IsActive =
            ActiveCheckBox.IsChecked == true;

        var selectedRole =
            (RoleComboBox.SelectedItem as
                System.Windows.Controls.ComboBoxItem)
            ?.Content?.ToString();

        if (System.Enum.TryParse<UserRole>(
                selectedRole,
                true,
                out var role))
        {
            User.Role = role;
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}