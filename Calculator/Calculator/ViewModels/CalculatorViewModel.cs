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
        #region Fields & Properties

        private string _displayText = "0";
        private string _displayTextHistory = "";
        private int _currentBase = 10;
        private string _currentMode = "Standard";
        //private bool _isProgrammerMode = false;
        private double _displayFontSize = 36;
        private bool _isDigitGroupingEnabled = true;
        private CalculatorModel _calculator = new CalculatorModel();
        public ObservableCollection<string> CalculationHistory { get; set; } = new ObservableCollection<string>();
        private bool _resultShown = false;



        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        // New property for showing the history (the previous number and operator)
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

        private bool _isProgrammerMode;
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

        public string HexValue { get => _hexValue; set { _hexValue = value; OnPropertyChanged(nameof(HexValue)); } }
        public string DecValue { get => _decValue; set { _decValue = value; OnPropertyChanged(nameof(DecValue)); } }
        public string OctValue { get => _octValue; set { _octValue = value; OnPropertyChanged(nameof(OctValue)); } }
        public string BinValue { get => _binValue; set { _binValue = value; OnPropertyChanged(nameof(BinValue)); } }

        private string _hexValue = "0";
        private string _decValue = "0";
        private string _octValue = "0";
        private string _binValue = "0";

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
            NumberCommand = new RelayCommand(param => AppendNumber(param.ToString()));
            OperatorCommand = new RelayCommand(param => SetOperator(param.ToString()));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
            ClearEntryCommand = new RelayCommand(_ => DisplayText = "0");
            BackspaceCommand = new RelayCommand(_ => { if (_displayText.Length > 0) DisplayText = _displayText[..^1]; });

            MemoryClearCommand = new RelayCommand(_ => { CalculatorModel.MC(); DisplayText = "0"; });
            MemoryPlusCommand = new RelayCommand(_ => { CalculatorModel.MPlus(ref _displayText); DisplayText = "0"; });
            MemoryMinusCommand = new RelayCommand(_ => { CalculatorModel.MMinus(ref _displayText); DisplayText = "0"; });
            MemoryStoreCommand = new RelayCommand(_ => { CalculatorModel.MS(_displayText); DisplayText = "0"; });
            MemoryRecallCommand = new RelayCommand(_ => DisplayText = CalculatorModel.MR());
            MemoryListCommand = new RelayCommand(_ => DisplayText = CalculatorModel.MGreater());

            SquareRootCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Sqrt));
            SquareCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Pow));
            NegateCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Negate));
            ReciprocalCommand = new RelayCommand(_ => ApplyUnaryOp(CalculatorModel.Reciprocal));
            PercentCommand = new RelayCommand(_ => ApplyPercent());


            IsProgrammerMode = Properties.Settings.Default.IsProgrammerMode;
            CurrentBase = Properties.Settings.Default.LastBase;
            IsDigitGroupingEnabled = Properties.Settings.Default.DigitGrouping;

            SetBase(CurrentBase);


            ShowHistoryCommand = new RelayCommand(_ =>
            {
                // Creăm și afișăm fereastra de istoric
                var historyWindow = new Calculator.Views.HistoryWindow(CalculationHistory);
                historyWindow.ShowDialog();
            });
            MemoryStoreCommand = new RelayCommand(_ =>
            {
                // Stochează valoarea din display
                var storedValue = CalculatorModel.MS(DisplayText);
                if (storedValue != null)
                {
                    // Adaugă o intrare în istoric, de exemplu "MS: [valoare stocată]"
                    CalculationHistory.Add($"MS: {storedValue}");
                }
                // Resetează display-ul
                DisplayText = "0";
            });
        }


        #endregion

        #region Core Logic
        private void Clear()
        {
            DisplayText = "0"; // Resetăm display-ul
            DisplayTextHistory = ""; // Resetăm istoricul operației curente
            CalculationHistory.Clear(); // Ștergem întregul istoric de calcule
            _resultShown = false; // Resetăm flag-ul de rezultat afișat
        }


        // Metoda care inserează cifre în DisplayText (rămâne aproape neschimbată)
        private void AppendNumber(string number)
        {
            // Dacă s-a afișat rezultatul și se introduce o nouă cifră,
            // resetează calculatorul pentru a începe o nouă operație.
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

            if (unformatted == "0" && number != decimalSeparator)
                unformatted = number;
            else
                unformatted += number;

            string digitsOnly = unformatted.Replace(decimalSeparator, "");
            if (digitsOnly.Length > 16) return;

            if (decimal.TryParse(unformatted, out decimal parsed))
            {
                if (IsDigitGroupingEnabled)
                    DisplayText = unformatted.Contains(decimalSeparator)
                        ? parsed.ToString("N", CultureInfo.CurrentCulture)
                        : parsed.ToString("N0", CultureInfo.CurrentCulture);
                else
                    DisplayText = unformatted.Contains(decimalSeparator)
                        ? parsed.ToString("F", CultureInfo.CurrentCulture)
                        : parsed.ToString("F0", CultureInfo.CurrentCulture);
            }
            else
                DisplayText = unformatted;

            DisplayFontSize = digitsOnly.Length <= 8 ? 36 : 36 - 1.5 * (digitsOnly.Length - 8);
        }


        // Metoda care setează operatorul
        private void SetOperator(string op)
        {
            try
            {
                // Dacă nu avem încă nimic în "istoric", stocăm operandul curent și operatorul.
                if (string.IsNullOrEmpty(DisplayTextHistory))
                {
                    // ex: "5 +"
                    DisplayTextHistory = DisplayText + " " + op;
                    // Resetăm DisplayText pentru a introduce următorul operand
                    DisplayText = "0";
                }
                else
                {
                    // Dacă avem deja un operand+operator în DisplayTextHistory,
                    // se calculează rezultatul intermediar
                    CalculateResult();
                    // Resetăm flag-ul pentru a permite concatenarea ulterioară a numărului
                    _resultShown = false;
                    // Acum DisplayText conține rezultatul, îl stocăm cu noul operator
                    DisplayTextHistory = DisplayText + " " + op;
                    // Resetăm pentru operandul următor
                    DisplayText = "0";
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }


        // Metoda care calculează rezultatul
        private void CalculateResult()
        {
            try
            {
                // Construim expresia completă din DisplayTextHistory și DisplayText.
                // Exemplu: dacă DisplayTextHistory este "5 +" și DisplayText este "3", rezultă "5 + 3".
                string fullExpression = (DisplayTextHistory + " " + DisplayText).Trim();
                string[] parts = fullExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Dacă nu avem cel puțin 3 părți (operand, operator, operand), nu facem calculul
                if (parts.Length < 3) return;

                string resultString;

                // Mod Programmer
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
                // Mod Standard
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

                // Actualizează DisplayText cu rezultatul calculului.
                DisplayText = resultString;
                // Actualizează DisplayTextHistory pentru a afișa doar expresia completă, fără "=" și rezultat.
                DisplayTextHistory = fullExpression;

                _resultShown = true;
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }


        // Metodă pentru operatori unari (fără schimbări majore)
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

        // Metodă pentru procent (la fel, dacă folosești DisplayTextHistory trebuie adaptat)
        private void ApplyPercent()
        {
            try
            {
                // Construim expresia completă din DisplayTextHistory și DisplayText
                string fullExpression = (DisplayTextHistory + " " + DisplayText).Trim();
                string[] parts = fullExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Ne asigurăm că avem cel puțin 3 elemente: operand, operator și operand
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

                // Actualizează DisplayText cu procentul calculat
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

        #endregion


        #region Helpers
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
