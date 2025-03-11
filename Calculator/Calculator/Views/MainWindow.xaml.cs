using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Calculator.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // For VisualTreeHelper
using System.Collections.Generic;
using System.Linq;

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
            if (viewModel.IsProgrammerMode)
            {
                viewModel.SetBase(10);
                UpdateButtonsForBase(10);
            }
            if (btnDec.IsChecked == true)
            {
                Base_Checked(btnDec, null);
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



        private void Base_Checked(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CalculatorViewModel;
            if (vm == null)
                return;

            if (sender is RadioButton rb && rb.Tag != null)
            {
                if (int.TryParse(rb.Tag.ToString(), out int newBase))
                {
                    vm.SetBase(newBase);
                    UpdateButtonsForBase(newBase);
                }
            }
        }



        private void UpdateButtonsForBase(int baseVal)
        {
            // 1) Get all the Button controls under ProgrammerGrid
            var allButtons = GetAllButtons(ProgrammerGrid);

            // 2) For each button, if its content is 0..9 or A..F, check if valid in the chosen base
            foreach (var btn in allButtons)
            {
                string label = btn.Content?.ToString().ToUpper() ?? "";

                // If it's exactly 1 character and in 0-9A-F, then decide based on base
                if (label.Length == 1 && "0123456789ABCDEF".Contains(label))
                {
                    btn.IsEnabled = IsValidDigitForBase(label, baseVal);
                }
                else
                {
                    // Operators, memory buttons, etc. remain enabled
                    btn.IsEnabled = true;
                }
            }
        }


           private bool IsValidDigitForBase(string digit, int baseVal)
            {
                switch (digit)
                {
                    case "0": return true;  // '0' is valid in all bases
                    case "1": return baseVal >= 2;
                    case "2": return baseVal >= 3;
                    case "3": return baseVal >= 4;
                    case "4": return baseVal >= 5;
                    case "5": return baseVal >= 6;
                    case "6": return baseVal >= 7;
                    case "7": return baseVal >= 8;
                    case "8": return baseVal >= 9;
                    case "9": return baseVal >= 10;
                    case "A": return baseVal == 16;
                    case "B": return baseVal == 16;
                    case "C": return baseVal == 16;
                    case "D": return baseVal == 16;
                    case "E": return baseVal == 16;
                    case "F": return baseVal == 16;
                    default: return false;
                }
            }

        private IEnumerable<Button> GetAllButtons(DependencyObject parent)
        {
            var result = new List<Button>();
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button btn)
                {
                    result.Add(btn);
                }
                else
                {
                    // Recurse further down
                    result.AddRange(GetAllButtons(child));
                }
            }
            return result;
        }

    }
}
