using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PS.SuperNDT.UI.Views;

public sealed class DefectDialog : Window
{
    private readonly ComboBox _defectTypeComboBox;
    private readonly ComboBox _severityComboBox;
    private readonly TextBox _positionTextBox;
    private readonly TextBox _lengthTextBox;
    private readonly TextBox _widthTextBox;
    private readonly TextBox _remarksTextBox;

    public string DefectType =>
        _defectTypeComboBox.SelectedItem?.ToString()
        ?? "UNCLASSIFIED";

    public string Severity =>
        _severityComboBox.SelectedItem?.ToString()
        ?? "UNCLASSIFIED";

    public double Position { get; private set; }

    public double Length { get; private set; }

    public double DefectWidth { get; private set; }

    public string Remarks =>
        _remarksTextBox.Text.Trim();

    public DefectDialog(
        double position = 0,
        double length = 0,
        double width = 0)
        : this(
            "UNCLASSIFIED",
            "UNCLASSIFIED",
            position,
            length,
            width,
            "")
    {
    }

    public DefectDialog(
        string defectType,
        string severity,
        double position,
        double length,
        double width,
        string remarks)
    {
        Title = "DEFECT DETAILS";

        Width = 470;
        Height = 560;

        MinWidth = 430;
        MinHeight = 520;

        WindowStartupLocation =
            WindowStartupLocation.CenterOwner;

        ResizeMode =
            ResizeMode.NoResize;

        ShowInTaskbar = false;

        Background =
            new SolidColorBrush(
                Color.FromRgb(
                    31,
                    35,
                    42));

        Foreground =
            Brushes.White;

        position =
            Math.Max(
                0,
                position);

        length =
            Math.Max(
                0,
                length);

        width =
            Math.Max(
                0,
                width);

        defectType =
            string.IsNullOrWhiteSpace(defectType)
                ? "UNCLASSIFIED"
                : defectType.Trim();

        severity =
            string.IsNullOrWhiteSpace(severity)
                ? "UNCLASSIFIED"
                : severity.Trim();

        remarks ??= "";

        Grid root =
            new Grid
            {
                Margin =
                    new Thickness(22)
            };

        root.RowDefinitions.Add(
            new RowDefinition
            {
                Height =
                    GridLength.Auto
            });

        root.RowDefinitions.Add(
            new RowDefinition
            {
                Height =
                    new GridLength(
                        1,
                        GridUnitType.Star)
            });

        root.RowDefinitions.Add(
            new RowDefinition
            {
                Height =
                    GridLength.Auto
            });

        // =====================================================
        // HEADER
        // =====================================================

        Border header =
            new Border
            {
                Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            52,
                            59,
                            70)),

                CornerRadius =
                    new CornerRadius(6),

                Padding =
                    new Thickness(
                        14,
                        11,
                        14,
                        11),

                Margin =
                    new Thickness(
                        0,
                        0,
                        0,
                        16)
            };

        TextBlock headerText =
            new TextBlock
            {
                Text =
                    "DEFECT DETAILS",

                Foreground =
                    Brushes.White,

                FontSize = 15,

                FontWeight =
                    FontWeights.Bold,

                VerticalAlignment =
                    VerticalAlignment.Center
            };

        header.Child =
            headerText;

        Grid.SetRow(
            header,
            0);

        root.Children.Add(
            header);

        // =====================================================
        // FORM
        // =====================================================

        Grid form =
            new Grid();

        form.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width =
                    new GridLength(125)
            });

        form.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width =
                    new GridLength(
                        1,
                        GridUnitType.Star)
            });

        for (int i = 0; i < 6; i++)
        {
            form.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        i == 5
                            ? new GridLength(130)
                            : new GridLength(48)
                });
        }

        // =====================================================
        // DEFECT TYPE
        // =====================================================

        AddLabel(
            form,
            "DEFECT TYPE",
            0);

        _defectTypeComboBox =
            CreateComboBox(
                new[]
                {
                    "UNCLASSIFIED",
                    "CRACK",
                    "LACK OF FUSION",
                    "LACK OF PENETRATION",
                    "POROSITY",
                    "SLAG INCLUSION",
                    "UNDERCUT",
                    "TUNGSTEN INCLUSION",
                    "BURN THROUGH",
                    "LINEAR INDICATION",
                    "NON-LINEAR INDICATION",
                    "OTHER"
                });

        Grid.SetRow(
            _defectTypeComboBox,
            0);

        Grid.SetColumn(
            _defectTypeComboBox,
            1);

        form.Children.Add(
            _defectTypeComboBox);

        // =====================================================
        // SEVERITY
        // =====================================================

        AddLabel(
            form,
            "SEVERITY",
            1);

        _severityComboBox =
            CreateComboBox(
                new[]
                {
                    "UNCLASSIFIED",
                    "MINOR",
                    "MAJOR",
                    "CRITICAL"
                });

        Grid.SetRow(
            _severityComboBox,
            1);

        Grid.SetColumn(
            _severityComboBox,
            1);

        form.Children.Add(
            _severityComboBox);

        // =====================================================
        // POSITION
        // =====================================================

        AddLabel(
            form,
            "POSITION (mm)",
            2);

        _positionTextBox =
            CreateTextBox(
                position.ToString("0.##"));

        Grid.SetRow(
            _positionTextBox,
            2);

        Grid.SetColumn(
            _positionTextBox,
            1);

        form.Children.Add(
            _positionTextBox);

        // =====================================================
        // LENGTH
        // =====================================================

        AddLabel(
            form,
            "LENGTH (mm)",
            3);

        _lengthTextBox =
            CreateTextBox(
                length.ToString("0.##"));

        Grid.SetRow(
            _lengthTextBox,
            3);

        Grid.SetColumn(
            _lengthTextBox,
            1);

        form.Children.Add(
            _lengthTextBox);

        // =====================================================
        // WIDTH
        // =====================================================

        AddLabel(
            form,
            "WIDTH (mm)",
            4);

        _widthTextBox =
            CreateTextBox(
                width.ToString("0.##"));

        Grid.SetRow(
            _widthTextBox,
            4);

        Grid.SetColumn(
            _widthTextBox,
            1);

        form.Children.Add(
            _widthTextBox);

        // =====================================================
        // REMARKS
        // =====================================================

        AddLabel(
            form,
            "REMARKS",
            5);

        _remarksTextBox =
            new TextBox
            {
                Text =
                    remarks,

                Margin =
                    new Thickness(
                        6,
                        4,
                        0,
                        6),

                Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            21,
                            26,
                            32)),

                Foreground =
                    Brushes.White,

                BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            70,
                            81,
                            94)),

                BorderThickness =
                    new Thickness(1),

                Padding =
                    new Thickness(8),

                FontSize = 12,

                AcceptsReturn = true,

                TextWrapping =
                    TextWrapping.Wrap,

                VerticalScrollBarVisibility =
                    ScrollBarVisibility.Auto
            };

        Grid.SetRow(
            _remarksTextBox,
            5);

        Grid.SetColumn(
            _remarksTextBox,
            1);

        form.Children.Add(
            _remarksTextBox);

        Grid.SetRow(
            form,
            1);

        root.Children.Add(
            form);

        // =====================================================
        // BUTTONS
        // =====================================================

        StackPanel buttons =
            new StackPanel
            {
                Orientation =
                    Orientation.Horizontal,

                HorizontalAlignment =
                    HorizontalAlignment.Right,

                Margin =
                    new Thickness(
                        0,
                        14,
                        0,
                        0)
            };

        Button cancelButton =
            new Button
            {
                Content =
                    "CANCEL",

                Width = 105,

                Height = 38,

                Margin =
                    new Thickness(5),

                Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            70,
                            77,
                            88)),

                Foreground =
                    Brushes.White,

                BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            95,
                            105,
                            118)),

                FontWeight =
                    FontWeights.Bold
            };

        cancelButton.Click +=
            CancelButton_Click;

        Button saveButton =
            new Button
            {
                Content =
                    "SAVE DEFECT",

                Width = 125,

                Height = 38,

                Margin =
                    new Thickness(5),

                Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            38,
                            105,
                            75)),

                Foreground =
                    Brushes.White,

                BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            77,
                            155,
                            113)),

                FontWeight =
                    FontWeights.Bold
            };

        saveButton.Click +=
            SaveButton_Click;

        buttons.Children.Add(
            cancelButton);

        buttons.Children.Add(
            saveButton);

        Grid.SetRow(
            buttons,
            2);

        root.Children.Add(
            buttons);

        Content =
            root;

        // =====================================================
        // LOAD EXISTING VALUES
        // =====================================================

        SetComboBoxValue(
            _defectTypeComboBox,
            defectType);

        SetComboBoxValue(
            _severityComboBox,
            severity);
    }

    // =========================================================
    // SET COMBOBOX VALUE
    // =========================================================

    private static void SetComboBoxValue(
        ComboBox comboBox,
        string value)
    {
        for (
            int i = 0;
            i < comboBox.Items.Count;
            i++)
        {
            if (string.Equals(
                    comboBox.Items[i]?.ToString(),
                    value,
                    StringComparison.OrdinalIgnoreCase))
            {
                comboBox.SelectedIndex = i;

                return;
            }
        }

        comboBox.SelectedIndex = 0;
    }

    // =========================================================
    // LABEL
    // =========================================================

    private static void AddLabel(
        Grid grid,
        string text,
        int row)
    {
        TextBlock label =
            new TextBlock
            {
                Text =
                    text,

                Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            185,
                            193,
                            203)),

                FontSize = 11,

                FontWeight =
                    FontWeights.Bold,

                VerticalAlignment =
                    VerticalAlignment.Center,

                Margin =
                    new Thickness(
                        0,
                        0,
                        8,
                        0)
            };

        Grid.SetRow(
            label,
            row);

        Grid.SetColumn(
            label,
            0);

        grid.Children.Add(
            label);
    }

    // =========================================================
    // COMBOBOX
    // =========================================================

    private static ComboBox CreateComboBox(
        string[] items)
    {
        SolidColorBrush darkBackground =
            new SolidColorBrush(
                Color.FromRgb(
                    21,
                    26,
                    32));

        SolidColorBrush borderBrush =
            new SolidColorBrush(
                Color.FromRgb(
                    70,
                    81,
                    94));

        SolidColorBrush hoverBackground =
            new SolidColorBrush(
                Color.FromRgb(
                    48,
                    58,
                    72));

        SolidColorBrush selectedBackground =
            new SolidColorBrush(
                Color.FromRgb(
                    55,
                    67,
                    82));

        ComboBox comboBox =
            new ComboBox
            {
                Margin =
                    new Thickness(
                        6,
                        4,
                        0,
                        4),

                Background =
                    darkBackground,

                Foreground =
                    Brushes.White,

                BorderBrush =
                    borderBrush,

                BorderThickness =
                    new Thickness(1),

                FontSize = 12,

                Padding =
                    new Thickness(
                        8,
                        2,
                        8,
                        2),

                HorizontalContentAlignment =
                    HorizontalAlignment.Left,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                FocusVisualStyle =
                    null
            };

        Style itemStyle =
            new Style(
                typeof(ComboBoxItem));

        itemStyle.Setters.Add(
            new Setter(
                Control.BackgroundProperty,
                darkBackground));

        itemStyle.Setters.Add(
            new Setter(
                Control.ForegroundProperty,
                Brushes.White));

        itemStyle.Setters.Add(
            new Setter(
                Control.BorderBrushProperty,
                Brushes.Transparent));

        itemStyle.Setters.Add(
            new Setter(
                Control.PaddingProperty,
                new Thickness(
                    8,
                    6,
                    8,
                    6)));

        itemStyle.Setters.Add(
            new Setter(
                Control.HorizontalContentAlignmentProperty,
                HorizontalAlignment.Left));

        itemStyle.Setters.Add(
            new Setter(
                Control.VerticalContentAlignmentProperty,
                VerticalAlignment.Center));

        Trigger selectedTrigger =
            new Trigger
            {
                Property =
                    ComboBoxItem.IsSelectedProperty,

                Value = true
            };

        selectedTrigger.Setters.Add(
            new Setter(
                Control.BackgroundProperty,
                selectedBackground));

        selectedTrigger.Setters.Add(
            new Setter(
                Control.ForegroundProperty,
                Brushes.White));

        itemStyle.Triggers.Add(
            selectedTrigger);

        Trigger mouseOverTrigger =
            new Trigger
            {
                Property =
                    ComboBoxItem.IsMouseOverProperty,

                Value = true
            };

        mouseOverTrigger.Setters.Add(
            new Setter(
                Control.BackgroundProperty,
                hoverBackground));

        mouseOverTrigger.Setters.Add(
            new Setter(
                Control.ForegroundProperty,
                Brushes.White));

        itemStyle.Triggers.Add(
            mouseOverTrigger);

        comboBox.ItemContainerStyle =
            itemStyle;

        ControlTemplate toggleTemplate =
            new ControlTemplate(
                typeof(ToggleButton));

        FrameworkElementFactory toggleGrid =
            new FrameworkElementFactory(
                typeof(Grid));

        FrameworkElementFactory toggleBorder =
            new FrameworkElementFactory(
                typeof(Border));

        toggleBorder.SetValue(
            Border.BackgroundProperty,
            darkBackground);

        toggleBorder.SetValue(
            Border.BorderBrushProperty,
            borderBrush);

        toggleBorder.SetValue(
            Border.BorderThicknessProperty,
            new Thickness(1));

        toggleGrid.AppendChild(
            toggleBorder);

        FrameworkElementFactory arrow =
            new FrameworkElementFactory(
                typeof(TextBlock));

        arrow.SetValue(
            TextBlock.TextProperty,
            "▼");

        arrow.SetValue(
            TextBlock.ForegroundProperty,
            new SolidColorBrush(
                Color.FromRgb(
                    185,
                    193,
                    203)));

        arrow.SetValue(
            TextBlock.FontSizeProperty,
            9.0);

        arrow.SetValue(
            TextBlock.HorizontalAlignmentProperty,
            HorizontalAlignment.Right);

        arrow.SetValue(
            TextBlock.VerticalAlignmentProperty,
            VerticalAlignment.Center);

        arrow.SetValue(
            FrameworkElement.MarginProperty,
            new Thickness(
                0,
                0,
                9,
                0));

        arrow.SetValue(
            UIElement.IsHitTestVisibleProperty,
            false);

        toggleGrid.AppendChild(
            arrow);

        toggleTemplate.VisualTree =
            toggleGrid;

        ControlTemplate comboTemplate =
            new ControlTemplate(
                typeof(ComboBox));

        FrameworkElementFactory rootGrid =
            new FrameworkElementFactory(
                typeof(Grid));

        FrameworkElementFactory toggleButton =
            new FrameworkElementFactory(
                typeof(ToggleButton));

        toggleButton.Name =
            "ToggleButton";

        toggleButton.SetValue(
            Control.TemplateProperty,
            toggleTemplate);

        toggleButton.SetValue(
            UIElement.FocusableProperty,
            false);

        toggleButton.SetValue(
            ToggleButton.ClickModeProperty,
            ClickMode.Press);

        toggleButton.SetBinding(
            ToggleButton.IsCheckedProperty,
            new Binding(
                "IsDropDownOpen")
            {
                RelativeSource =
                    new RelativeSource(
                        RelativeSourceMode.TemplatedParent),

                Mode =
                    BindingMode.TwoWay
            });

        rootGrid.AppendChild(
            toggleButton);

        FrameworkElementFactory contentPresenter =
            new FrameworkElementFactory(
                typeof(ContentPresenter));

        contentPresenter.Name =
            "ContentSite";

        contentPresenter.SetValue(
            FrameworkElement.MarginProperty,
            new Thickness(
                10,
                0,
                35,
                0));

        contentPresenter.SetValue(
            FrameworkElement.HorizontalAlignmentProperty,
            HorizontalAlignment.Left);

        contentPresenter.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            VerticalAlignment.Center);

        contentPresenter.SetValue(
            UIElement.IsHitTestVisibleProperty,
            false);

        contentPresenter.SetValue(
            ContentPresenter.ContentProperty,
            new TemplateBindingExtension(
                ComboBox.SelectionBoxItemProperty));

        contentPresenter.SetValue(
            ContentPresenter.ContentTemplateProperty,
            new TemplateBindingExtension(
                ComboBox.SelectionBoxItemTemplateProperty));

        contentPresenter.SetValue(
            ContentPresenter.ContentTemplateSelectorProperty,
            new TemplateBindingExtension(
                ItemsControl.ItemTemplateSelectorProperty));

        contentPresenter.SetValue(
            TextElement.ForegroundProperty,
            Brushes.White);

        contentPresenter.SetValue(
            TextElement.FontSizeProperty,
            12.0);

        rootGrid.AppendChild(
            contentPresenter);

        FrameworkElementFactory popup =
            new FrameworkElementFactory(
                typeof(Popup));

        popup.Name =
            "Popup";

        popup.SetValue(
            Popup.PlacementProperty,
            PlacementMode.Bottom);

        popup.SetValue(
            Popup.AllowsTransparencyProperty,
            true);

        popup.SetValue(
            Popup.FocusableProperty,
            false);

        popup.SetBinding(
            Popup.IsOpenProperty,
            new Binding(
                "IsDropDownOpen")
            {
                RelativeSource =
                    new RelativeSource(
                        RelativeSourceMode.TemplatedParent),

                Mode =
                    BindingMode.TwoWay
            });

        FrameworkElementFactory dropDown =
            new FrameworkElementFactory(
                typeof(Grid));

        dropDown.SetValue(
            FrameworkElement.MinWidthProperty,
            new TemplateBindingExtension(
                FrameworkElement.ActualWidthProperty));

        dropDown.SetValue(
            FrameworkElement.MaxHeightProperty,
            new TemplateBindingExtension(
                ComboBox.MaxDropDownHeightProperty));

        FrameworkElementFactory dropDownBorder =
            new FrameworkElementFactory(
                typeof(Border));

        dropDownBorder.Name =
            "DropDownBorder";

        dropDownBorder.SetValue(
            Border.BackgroundProperty,
            darkBackground);

        dropDownBorder.SetValue(
            Border.BorderBrushProperty,
            borderBrush);

        dropDownBorder.SetValue(
            Border.BorderThicknessProperty,
            new Thickness(1));

        dropDownBorder.SetValue(
            Border.CornerRadiusProperty,
            new CornerRadius(2));

        FrameworkElementFactory scrollViewer =
            new FrameworkElementFactory(
                typeof(ScrollViewer));

        scrollViewer.SetValue(
            ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollBarVisibility.Auto);

        scrollViewer.SetValue(
            ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollBarVisibility.Disabled);

        FrameworkElementFactory itemsPresenter =
            new FrameworkElementFactory(
                typeof(ItemsPresenter));

        itemsPresenter.SetValue(
            KeyboardNavigation.DirectionalNavigationProperty,
            KeyboardNavigationMode.Contained);

        scrollViewer.AppendChild(
            itemsPresenter);

        dropDownBorder.AppendChild(
            scrollViewer);

        dropDown.AppendChild(
            dropDownBorder);

        popup.AppendChild(
            dropDown);

        rootGrid.AppendChild(
            popup);

        comboTemplate.VisualTree =
            rootGrid;

        comboBox.Template =
            comboTemplate;

        foreach (string item in items)
        {
            comboBox.Items.Add(
                item);
        }

        return comboBox;
    }

    // =========================================================
    // TEXTBOX
    // =========================================================

    private static TextBox CreateTextBox(
        string text)
    {
        return new TextBox
        {
            Text =
                text,

            Margin =
                new Thickness(
                    6,
                    4,
                    0,
                    4),

            Background =
                new SolidColorBrush(
                    Color.FromRgb(
                        21,
                        26,
                        32)),

            Foreground =
                Brushes.White,

            BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(
                        70,
                        81,
                        94)),

            BorderThickness =
                new Thickness(1),

            Padding =
                new Thickness(
                    8,
                    4,
                    8,
                    4),

            FontSize = 12
        };
    }

    // =========================================================
    // SAVE
    // =========================================================

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!double.TryParse(
                _positionTextBox.Text.Trim(),
                out double position) ||
            position < 0)
        {
            MessageBox.Show(
                "Please enter a valid defect position.",
                "Invalid Position",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            _positionTextBox.Focus();

            return;
        }

        if (!double.TryParse(
                _lengthTextBox.Text.Trim(),
                out double length) ||
            length <= 0)
        {
            MessageBox.Show(
                "Please enter a valid defect length.",
                "Invalid Length",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            _lengthTextBox.Focus();

            return;
        }

        if (!double.TryParse(
                _widthTextBox.Text.Trim(),
                out double defectWidth) ||
            defectWidth <= 0)
        {
            MessageBox.Show(
                "Please enter a valid defect width.",
                "Invalid Width",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            _widthTextBox.Focus();

            return;
        }

        Position =
            position;

        Length =
            length;

        DefectWidth =
            defectWidth;

        DialogResult =
            true;
    }

    // =========================================================
    // CANCEL
    // =========================================================

    private void CancelButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult =
            false;
    }
}