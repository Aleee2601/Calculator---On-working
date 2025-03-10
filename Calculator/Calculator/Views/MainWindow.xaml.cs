using System;
using System.Windows;
using System.Windows.Controls;
using Calculator.ViewModels;

namespace Calculator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculatorViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new CalculatorViewModel();
            DataContext = viewModel;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateModeView();
        }

        private void Base_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                int newBase = int.Parse(rb.Tag.ToString());
                viewModel.SetBase(newBase);

                // Show HEX buttons only if base is 16
                Visibility hexVisibility = (newBase == 16) ? Visibility.Visible : Visibility.Collapsed;

                foreach (string hexButtonName in new[] { "btnA", "btnB", "btnC", "btnD", "btnE", "btnF" })
                {
                    if (FindName(hexButtonName) is Button btn)
                    {
                        btn.Visibility = hexVisibility;
                    }
                }
            }
        }

        private void txtDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDisplay.Text))
            {
                txtDisplay.Text = "0";
                return;
            }

            if (!double.TryParse(txtDisplay.Text, out _))
            {
                // Revert to last known valid value
                txtDisplay.Text = viewModel.DisplayText;
            }
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsProgrammerMode = !viewModel.IsProgrammerMode;
            UpdateModeView();
        }

        private void UpdateModeView()
        {
            if (StandardGrid != null && ProgrammerGrid != null)
            {
                StandardGrid.Visibility = viewModel.IsProgrammerMode ? Visibility.Collapsed : Visibility.Visible;
                ProgrammerGrid.Visibility = viewModel.IsProgrammerMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
