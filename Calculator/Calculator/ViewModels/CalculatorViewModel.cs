using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Calculator.Models;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        #region Fields
        // Backing fields for properties
        private string _displayText = "0";
        private string _displayTextHistory = "";
        private int _currentBase = 10;
        private string _currentMode = "Standard";
        private double _displayFontSize = 36;
        private bool _isDigitGroupingEnabled = true;
        private bool _resultShown = false;
        private bool _isProgrammerMode;
        private bool _isPrecedenceModeEnabled = false;

        // Fields for conversion values
        private string _hexValue = "0";
        private string _decValue = "0";
        private string _octValue = "0";
        private string _binValue = "0";

        // Instance of the calculator model and calculation history
        private CalculatorModel _calculator = new CalculatorModel();
        #endregion

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> CalculationHistory { get; set; } = new ObservableCollection<string>();

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged(nameof(DisplayText));
                    UpdateConversionValues();
                }
            }
        }

        public string DisplayTextHistory
        {
            get => _displayTextHistory;
            set
            {
                if (_displayTextHistory != value)
                {
                    _displayTextHistory = value;
                    OnPropertyChanged(nameof(DisplayTextHistory));
                }
            }
        }

        public double DisplayFontSize
        {
            get => _displayFontSize;
            set
            {
                if (_displayFontSize != value)
                {
                    _displayFontSize = value;
                    OnPropertyChanged(nameof(DisplayFontSize));
                }
            }
        }

        public string CurrentMode
        {
            get => _currentMode;
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    OnPropertyChanged(nameof(CurrentMode));
                }
            }
        }

        public bool IsProgrammerMode
        {
            get => _isProgrammerMode;
            set
            {
                if (_isProgrammerMode != value)
                {
                    _isProgrammerMode = value;
                    OnPropertyChanged(nameof(IsProgrammerMode));
                    CurrentMode = _isProgrammerMode ? "Programmer" : "Standard";
                }
            }
        }

        public int CurrentBase
        {
            get => _currentBase;
            set
            {
                if (_currentBase != value)
                {
                    _currentBase = value;
                    OnPropertyChanged(nameof(CurrentBase));
                }
            }
        }

        public bool IsDigitGroupingEnabled
        {
            get => _isDigitGroupingEnabled;
            set
            {
                if (_isDigitGroupingEnabled != value)
                {
                    _isDigitGroupingEnabled = value;
                    OnPropertyChanged(nameof(IsDigitGroupingEnabled));
                    AppendNumber("");
                }
            }
        }

        public bool IsPrecedenceModeEnabled
        {
            get => _isPrecedenceModeEnabled;
            set
            {
                if (_isPrecedenceModeEnabled != value)
                {
                    _isPrecedenceModeEnabled = value;
                    OnPropertyChanged(nameof(IsPrecedenceModeEnabled));
                }
            }
        }

        public string HexValue
        {
            get => _hexValue;
            set { _hexValue = value; OnPropertyChanged(nameof(HexValue)); }
        }

        public string DecValue
        {
            get => _decValue;
            set { _decValue = value; OnPropertyChanged(nameof(DecValue)); }
        }

        public string OctValue
        {
            get => _octValue;
            set { _octValue = value; OnPropertyChanged(nameof(OctValue)); }
        }

        public string BinValue
        {
            get => _binValue;
            set { _binValue = value; OnPropertyChanged(nameof(BinValue)); }
        }
        #endregion

        #region Commands
        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryPlusCommand { get; }
        public ICommand MemoryMinusCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemoryListCommand { get; }
        public ICommand SquareRootCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand NegateCommand { get; }
        public ICommand ReciprocalCommand { get; }
        public ICommand PercentCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        #endregion

        #region Constructor
        public CalculatorViewModel()
        {
            // Command Initialization
            NumberCommand = new RelayCommand(param => AppendNumber(param.ToString()));
            OperatorCommand = new RelayCommand(param => SetOperator(param.ToString()));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
            ClearEntryCommand = new RelayCommand(_ => DisplayText = "0");
            BackspaceCommand = new RelayCommand(_ =>
            {
                if (_displayText.Length > 0)
                    DisplayText = _displayText[..^1];
            });

            MemoryClearCommand = new RelayCommand(_ => { CalculatorModel.MC(); DisplayText = "0"; });
            MemoryPlusCommand = new RelayCommand(_ =>
            {
                CalculatorModel.MPlus(DisplayText);
                CalculationHistory.Add($"M+: {CalculatorModel.MR()}");
                DisplayText = "0";
            });
            MemoryMinusCommand = new RelayCommand(_ =>
            {
                CalculatorModel.MMinus(DisplayText);
                DisplayText = "0";
            });
            MemoryStoreCommand = new RelayCommand(_ => { CalculatorModel.MS(DisplayText); DisplayText = "0"; });
            MemoryRecallCommand = new RelayCommand(_ => DisplayText = CalculatorModel.MR());
            MemoryListCommand = new RelayCommand(_ => DisplayText = CalculatorModel.MGreater());

            SquareRootCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Sqrt));
            SquareCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Pow));
            NegateCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Negate));
            ReciprocalCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Reciprocal));
            PercentCommand = new RelayCommand(_ => ApplyPercent());

            // Load saved settings
            IsProgrammerMode = Properties.Settings.Default.IsProgrammerMode;
            CurrentBase = Properties.Settings.Default.LastBase;
            IsDigitGroupingEnabled = Properties.Settings.Default.DigitGrouping;
            SetBase(CurrentBase);

            ShowHistoryCommand = new RelayCommand(_ =>
            {
                // Create and show the history window
                var historyWindow = new Calculator.Views.HistoryWindow(CalculationHistory);
                historyWindow.ShowDialog();
            });

            // Overriding MemoryStoreCommand for history update
            MemoryStoreCommand = new RelayCommand(_ =>
            {
                var storedValue = CalculatorModel.MS(DisplayText);
                if (!string.IsNullOrEmpty(storedValue) && storedValue != "0")
                {
                    CalculationHistory.Add($"MS: {storedValue}");
                }
                DisplayText = "0";
            });
        }
        #endregion

        #region Core Logic
        private void Clear()
        {
            DisplayText = "0";           // Reset the display
            DisplayTextHistory = "";     // Clear the operation history
            CalculationHistory.Clear();  // Clear calculation history
            _resultShown = false;        // Reset the result flag
        }

        private void AppendNumber(string number)
        {
            if (_resultShown)
            {
                DisplayText = "0";
                DisplayTextHistory = "";
                _resultShown = false;
            }

            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string unformatted = DisplayText.Replace(groupSeparator, "");

            if (number == decimalSeparator && unformatted.Contains(decimalSeparator))
                return;

            unformatted = (unformatted == "0" && number != decimalSeparator)
                ? number
                : unformatted + number;

            string digitsOnly = unformatted.Replace(decimalSeparator, "");
            if (digitsOnly.Length > 16) return;

            if (decimal.TryParse(unformatted, out decimal parsed))
            {
                DisplayText = IsDigitGroupingEnabled
                    ? (unformatted.Contains(decimalSeparator)
                        ? parsed.ToString("N", CultureInfo.CurrentCulture)
                        : parsed.ToString("N0", CultureInfo.CurrentCulture))
                    : (unformatted.Contains(decimalSeparator)
                        ? parsed.ToString("F", CultureInfo.CurrentCulture)
                        : parsed.ToString("F0", CultureInfo.CurrentCulture));
            }
            else
            {
                DisplayText = unformatted;
            }

            DisplayFontSize = digitsOnly.Length <= 8 ? 36 : 36 - 1.5 * (digitsOnly.Length - 8);
        }

        private void SetOperator(string op)
        {
            try
            {
                if (IsPrecedenceModeEnabled)
                {
                    DisplayTextHistory += DisplayText + " " + op + " ";
                    DisplayText = "0";
                }
                else
                {
                    if (string.IsNullOrEmpty(DisplayTextHistory))
                    {
                        DisplayTextHistory = DisplayText + " " + op;
                        DisplayText = "0";
                    }
                    else
                    {
                        CalculateResult();
                        DisplayTextHistory = DisplayText + " " + op;
                        DisplayText = "0";
                    }
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private void CalculateResult()
        {
            try
            {
                if (IsPrecedenceModeEnabled)
                {
                    string fullExpression = (DisplayTextHistory + DisplayText).Trim();
                    double result = EvaluateExpression(fullExpression);
                    DisplayText = result.ToString();
                    DisplayTextHistory = fullExpression;
                    _resultShown = true;
                }
                else
                {
                    string fullExpression = (DisplayTextHistory + " " + DisplayText).Trim();
                    string[] parts = fullExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 3) return;

                    string resultString;

                    if (IsProgrammerMode && _currentBase != 10)
                    {
                        int op1 = Convert.ToInt32(parts[0], _currentBase);
                        int op2 = Convert.ToInt32(parts[2], _currentBase);
                        int res = parts[1] switch
                        {
                            "+" => (int)CalculatorModel.Add(op1, op2),
                            "-" => (int)CalculatorModel.Subtract(op1, op2),
                            "*" => (int)CalculatorModel.Multiply(op1, op2),
                            "/" => (int)CalculatorModel.Divide(op1, op2),
                            "%" => (int)CalculatorModel.Modulo(op1, op2, "%"),
                            _ => throw new InvalidOperationException()
                        };
                        resultString = Convert.ToString(res, _currentBase).ToUpper();
                    }
                    else
                    {
                        double op1 = double.Parse(parts[0]);
                        double op2 = double.Parse(parts[2]);
                        double res = parts[1] switch
                        {
                            "+" => CalculatorModel.Add(op1, op2),
                            "-" => CalculatorModel.Subtract(op1, op2),
                            "*" => CalculatorModel.Multiply(op1, op2),
                            "/" => CalculatorModel.Divide(op1, op2),
                            "%" => CalculatorModel.Modulo(op1, op2, "%"),
                            _ => throw new InvalidOperationException()
                        };
                        resultString = res.ToString();
                    }

                    DisplayText = resultString;
                    DisplayTextHistory = fullExpression;
                    _resultShown = true;
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private double EvaluateExpression(string expression)
        {
            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            expression = expression.Replace(groupSeparator, "");
            var table = new System.Data.DataTable();
            object result = table.Compute(expression, "");
            return Convert.ToDouble(result);
        }

        private void ApplyUnaryOp(Func<double, double> op)
        {
            try
            {
                if (IsProgrammerMode && _currentBase != 10)
                {
                    int val = Convert.ToInt32(DisplayText, _currentBase);
                    int res = (int)op(val);
                    DisplayText = Convert.ToString(res, _currentBase).ToUpper();
                }
                else
                {
                    double val = double.Parse(DisplayText);
                    double res = op(val);
                    DisplayText = res.ToString();
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private void ApplyPercent()
        {
            try
            {
                string fullExpression = (DisplayTextHistory + " " + DisplayText).Trim();
                string[] parts = fullExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                    return;

                double op1 = double.Parse(parts[0]);
                string op = parts[1];
                double op2 = double.Parse(parts[2]);

                double percent = op switch
                {
                    "+" or "-" => op1 * op2 / 100,
                    "*" or "/" => op2 / 100,
                    _ => 0
                };

                DisplayText = percent.ToString();
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        public void SetBase(int newBase)
        {
            try
            {
                int decVal = Convert.ToInt32(DisplayText, _currentBase);
                CurrentBase = newBase;
                DisplayText = Convert.ToString(decVal, newBase).ToUpper();
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.IsProgrammerMode = IsProgrammerMode;
            Properties.Settings.Default.LastBase = CurrentBase;
            Properties.Settings.Default.DigitGrouping = IsDigitGroupingEnabled;
            Properties.Settings.Default.Save();
        }

        private void UpdateConversionValues()
        {
            try
            {
                int currentBase = CurrentBase;
                string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
                string cleanedText = DisplayText.Replace(groupSeparator, "");
                int value = Convert.ToInt32(cleanedText, currentBase);

                DecValue = value.ToString();
                HexValue = value.ToString("X");
                OctValue = ConvertToOctal(value);
                BinValue = Convert.ToString(value, 2);
            }
            catch (Exception)
            {
                // Optionally handle conversion errors here.
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

        #region Helpers
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}
