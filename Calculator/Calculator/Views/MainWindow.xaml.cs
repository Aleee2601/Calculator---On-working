﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Input;
using Calculator.ViewModels;

namespace Calculator.Views
{
    public partial class MainWindow : Window
    {
        private CalculatorViewModel viewModel;
 
        #region Constructor & Lifecycle
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new CalculatorViewModel();
            DataContext = viewModel;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateModeView();
            UpdateModeSwitchButton();

            if (viewModel.IsProgrammerMode)
            {
                ApplyInitialBase();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            viewModel.SaveSettings();
        }
        #endregion

        #region Base Management
        private void ApplyInitialBase()
        {
            if (viewModel == null) return;

            RadioButton target = null;
            if (viewModel.CurrentBase == 16)
                target = btnHex;
            else if (viewModel.CurrentBase == 10)
                target = btnDec;
            else if (viewModel.CurrentBase == 8)
                target = btnOct;
            else if (viewModel.CurrentBase == 2)
                target = btnBin;

            if (target != null)
            {
                target.IsChecked = true;
                Base_Checked(target, null);
            }
        }

        private void Base_Checked(object sender, RoutedEventArgs e)
        {
            if (viewModel == null) return;

            if (sender is RadioButton rb && rb.Tag != null)
            {
                if (int.TryParse(rb.Tag.ToString(), out int newBase))
                {
                    viewModel.SetBase(newBase);
                    UpdateButtonsForBase(newBase);
                }
            }
        }

        private void UpdateButtonsForBase(int baseVal)
        {
            var allButtons = GetAllButtons(ProgrammerGrid);
            foreach (var btn in allButtons)
            {
                string label = btn.Content?.ToString().ToUpper() ?? "";
                if (label.Length == 1 && "0123456789ABCDEF".Contains(label))
                {
                    btn.IsEnabled = IsValidDigitForBase(label, baseVal);
                }
                else
                {
                    btn.IsEnabled = true;
                }
            }
        }

        private bool IsValidDigitForBase(string digit, int baseVal)
        {
            return digit switch
            {
                "0" => true,
                "1" => baseVal >= 2,
                "2" => baseVal >= 3,
                "3" => baseVal >= 4,
                "4" => baseVal >= 5,
                "5" => baseVal >= 6,
                "6" => baseVal >= 7,
                "7" => baseVal >= 8,
                "8" => baseVal >= 9,
                "9" => baseVal >= 10,
                "A" => baseVal == 16,
                "B" => baseVal == 16,
                "C" => baseVal == 16,
                "D" => baseVal == 16,
                "E" => baseVal == 16,
                "F" => baseVal == 16,
                _ => false
            };
        }

        private IEnumerable<Button> GetAllButtons(DependencyObject parent)
        {
            var result = new List<Button>();
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Button btn)
                    result.Add(btn);
                else
                    result.AddRange(GetAllButtons(child));
            }
            return result;
        }
        #endregion

        #region UI Actions
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtDisplay.Text);
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
                viewModel.DisplayText = Clipboard.GetText();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtDisplay.Text);
            viewModel.DisplayText = "0";
        }

        private void btnHamburger_Click(object sender, RoutedEventArgs e)
        {
            if (btnHamburger.Content.ToString() == "☰")
                btnHamburger.Content = "▼";
            else
                btnHamburger.Content = "☰";

            popupMenu.IsOpen = !popupMenu.IsOpen;
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("File clicked!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help clicked!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnModeSwitch_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsProgrammerMode = !viewModel.IsProgrammerMode;
            UpdateModeView();
            UpdateModeSwitchButton();

            // In Programmer mode, update button state for the current base.
            if (viewModel.IsProgrammerMode)
            {
                UpdateButtonsForBase(viewModel.CurrentBase);
            }

            popupMenu.IsOpen = false;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aplicatie realizata de Alexandra Onose, grupa 10LF233", "Despre aplicatie", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region UI Helpers
        private void UpdateModeView()
        {
            if (StandardGrid != null && ProgrammerGrid != null)
            {
                StandardGrid.Visibility = viewModel.IsProgrammerMode ? Visibility.Collapsed : Visibility.Visible;
                ProgrammerGrid.Visibility = viewModel.IsProgrammerMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateModeSwitchButton()
        {
            btnModeSwitch.Content = viewModel.IsProgrammerMode ? "Switch to Standard" : "Switch to Programmer";
        }

        private void menuFile_Click(object sender, RoutedEventArgs e)
        {
            popupFile.IsOpen = true;
        }

        private void txtDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDisplay.Text))
            {
                txtDisplay.Text = "0";
                return;
            }

            int currentBase = viewModel.CurrentBase;
            try
            {
                string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                string cleanedText = txtDisplay.Text.Replace(groupSeparator, "");
                int value = Convert.ToInt32(cleanedText, currentBase);

                viewModel.DecValue = value.ToString();
                viewModel.HexValue = value.ToString("X");
                viewModel.OctValue = ConvertToOctal(value);
                viewModel.BinValue = Convert.ToString(value, 2);
            }
            catch (Exception ex)
            {
                // Optionally log or handle conversion errors.
            }
        }

        private string ConvertToOctal(int number)
        {
            if (number == 0)
                return "0";

            string octal = "";
            while (number > 0)
            {
                int remainder = number % 8;
                octal = remainder.ToString() + octal;
                number /= 8;
            }
            return octal;
        }
        #endregion

        #region Keyboard Handling
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = DataContext as CalculatorViewModel;
            if (vm == null) return;

            // Programmer mode: handle valid digit/letter keys.
            if (vm.IsProgrammerMode)
            {
                string keyStr = null;
                if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    keyStr = (e.Key - Key.D0).ToString();
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    keyStr = (e.Key - Key.NumPad0).ToString();
                }
                else if (e.Key >= Key.A && e.Key <= Key.F)
                {
                    keyStr = e.Key.ToString().ToUpper();
                }

                if (!string.IsNullOrEmpty(keyStr))
                {
                    if (IsValidDigitForBase(keyStr, vm.CurrentBase))
                    {
                        vm.NumberCommand.Execute(keyStr);
                    }
                    e.Handled = true;
                    return;
                }
            }

            // Standard mode and other key handling.
            if (e.Key == Key.Enter)
            {
                if (vm.EqualsCommand.CanExecute(null))
                    vm.EqualsCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (vm.ClearCommand.CanExecute(null))
                    vm.ClearCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                if (vm.BackspaceCommand.CanExecute(null))
                    vm.BackspaceCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string digit = (e.Key - Key.D0).ToString();
                vm.NumberCommand.Execute(digit);
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = (e.Key - Key.NumPad0).ToString();
                vm.NumberCommand.Execute(digit);
                e.Handled = true;
            }
            else if (e.Key == Key.Add || e.Key == Key.OemPlus)
            {
                vm.OperatorCommand.Execute("+");
                e.Handled = true;
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                vm.OperatorCommand.Execute("-");
                e.Handled = true;
            }
            else if (e.Key == Key.Multiply)
            {
                vm.OperatorCommand.Execute("*");
                e.Handled = true;
            }
            else if (e.Key == Key.Divide || e.Key == Key.Oem2)
            {
                vm.OperatorCommand.Execute("/");
                e.Handled = true;
            }
            else if (e.Key == Key.OemComma || e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                vm.NumberCommand.Execute(separator);
                e.Handled = true;
            }
        }
        #endregion
    }
}
