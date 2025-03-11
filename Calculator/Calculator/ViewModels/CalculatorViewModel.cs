using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Calculator.Models;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private int _currentBase = 10;
        private string _currentMode = "Standard";
        private bool _isProgrammerMode = false;
        private CalculatorModel _calculator = new CalculatorModel();

        // Proprietate nouă pentru font size-ul display-ului
        private double _displayFontSize = 36;
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

        // Metodă de concatenare a cifrelor cu formatare automată și limitare la 16 cifre
        private void AppendNumber(string number)
        {
            // Obține separatorii conform culturii curente
            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // Elimină separatorii din textul curent pentru a obține șirul "brut"
            string unformatted = _displayText.Replace(groupSeparator, "");

            // Dacă se încearcă adăugarea separatorului zecimal și acesta este deja prezent, nu-l adaugă
            if (number == decimalSeparator && unformatted.Contains(decimalSeparator))
                return;

            // Dacă DisplayText este "0" și nu se adaugă separatorul zecimal, înlocuiește cu cifra introdusă
            if (unformatted == "0" && number != decimalSeparator)
                unformatted = number;
            else
                unformatted += number;

            // Extrage doar cifrele (ignorând separatorul zecimal)
            string digitsOnly = unformatted.Replace(decimalSeparator, "");

            // Dacă adăugarea ar face ca numărul să aibă mai mult de 16 cifre, nu adaugă
            if (digitsOnly.Length > 16)
                return;

            // Încearcă să parsezi șirul ca număr
            if (decimal.TryParse(unformatted, out decimal parsed))
            {
                // Dacă șirul conține separatorul zecimal, folosește formatul "N", altfel "N0"
                if (unformatted.Contains(decimalSeparator))
                    DisplayText = parsed.ToString("N", CultureInfo.CurrentCulture);
                else
                    DisplayText = parsed.ToString("N0", CultureInfo.CurrentCulture);
            }
            else
            {
                DisplayText = unformatted;
            }

            // Ajustează automat font size-ul astfel încât numărul să se încadreze:
            // - Dacă avem 8 sau mai puține cifre: fontul rămâne 36.
            // - Dacă avem între 9 și 16 cifre: reduce liniar de la 36 la 24.
            int digitCount = digitsOnly.Length;
            if (digitCount <= 8)
            {
                DisplayFontSize = 36;
            }
            else if (digitCount <= 16)
            {
                // Calculul liniei: la 8 cifre, fontul este 36; la 16 cifre, fontul devine 24.
                // Formula: font = 36 - (digitCount - 8) * ((36 - 24) / (16 - 8)) = 36 - 1.5 * (digitCount - 8)
                DisplayFontSize = 36 - 1.5 * (digitCount - 8);
            }

            OnPropertyChanged(nameof(DisplayText));
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

        // Comenzile existente
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

        // Comenzile pentru operațiile suplimentare
        public ICommand SquareRootCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand NegateCommand { get; }
        public ICommand ReciprocalCommand { get; }

        public CalculatorViewModel()
        {
            NumberCommand = new RelayCommand(param => AppendNumber(param.ToString()));
            OperatorCommand = new RelayCommand(param => SetOperator(param.ToString()));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
            ClearEntryCommand = new RelayCommand(_ => ClearEntry());
            BackspaceCommand = new RelayCommand(_ => Backspace());
            MemoryClearCommand = new RelayCommand(_ => MemoryClear());
            MemoryPlusCommand = new RelayCommand(_ => MemoryPlus());
            MemoryMinusCommand = new RelayCommand(_ => MemoryMinus());
            MemoryStoreCommand = new RelayCommand(_ => MemoryStore());
            MemoryRecallCommand = new RelayCommand(_ => MemoryRecall());

            // Inițializare comenzi operații suplimentare
            SquareRootCommand = new RelayCommand(_ => SquareRoot());
            SquareCommand = new RelayCommand(_ => Square());
            NegateCommand = new RelayCommand(_ => Negate());
            ReciprocalCommand = new RelayCommand(_ => Reciprocal());
        }

        private void SetOperator(string op)
        {
            _displayText += $" {op} ";
            OnPropertyChanged(nameof(DisplayText));
        }

        // Funcții helper pentru conversia numerelor între baze
        private int ConvertToDecimal(string number, int fromBase)
        {
            return Convert.ToInt32(number, fromBase);
        }

        private string ConvertFromDecimal(int number, int toBase)
        {
            return Convert.ToString(number, toBase).ToUpper();
        }

        // Calculul rezultatului – se face conversia dacă suntem în modul Programmer
        private void CalculateResult()
        {
            try
            {
                string[] parts = _displayText.Split(' ');
                if (parts.Length < 3) return;

                if (IsProgrammerMode && _currentBase != 10)
                {
                    int operand1 = ConvertToDecimal(parts[0], _currentBase);
                    string op = parts[1];
                    int operand2 = ConvertToDecimal(parts[2], _currentBase);
                    int result = op switch
                    {
                        "+" => (int)CalculatorModel.Add(operand1, operand2),
                        "-" => (int)CalculatorModel.Subtract(operand1, operand2),
                        "*" => (int)CalculatorModel.Multiply(operand1, operand2),
                        "/" => (int)CalculatorModel.Divide(operand1, operand2),
                        "%" => (int)CalculatorModel.Modulo(operand1, operand2),
                        _ => throw new InvalidOperationException("Operator necunoscut")
                    };
                    DisplayText = ConvertFromDecimal(result, _currentBase);
                }
                else
                {
                    double operand1 = double.Parse(parts[0]);
                    string op = parts[1];
                    double operand2 = double.Parse(parts[2]);
                    double result = op switch
                    {
                        "+" => CalculatorModel.Add(operand1, operand2),
                        "-" => CalculatorModel.Subtract(operand1, operand2),
                        "*" => CalculatorModel.Multiply(operand1, operand2),
                        "/" => CalculatorModel.Divide(operand1, operand2),
                        "%" => CalculatorModel.Modulo(operand1, operand2),
                        _ => throw new InvalidOperationException("Operator necunoscut")
                    };
                    DisplayText = result.ToString();
                }
                // Reformatăm rezultatul (apelul de AppendNumber cu string gol recalculă fontul și formatarea)
                AppendNumber("");
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private void Clear() => DisplayText = "0";
        private void ClearEntry() => DisplayText = "0";
        private void Backspace()
        {
            if (_displayText.Length > 0)
                DisplayText = _displayText[..^1];
        }

        private void MemoryClear()
        {
            CalculatorModel.MC();
            DisplayText = "0";
            OnPropertyChanged(nameof(DisplayText));
        }
        private void MemoryPlus()
        {
            CalculatorModel.MPlus(ref _displayText);
            OnPropertyChanged(nameof(DisplayText));
        }
        private void MemoryMinus()
        {
            CalculatorModel.MMinus(ref _displayText);
            OnPropertyChanged(nameof(DisplayText));
        }
        private void MemoryStore()
        {
            CalculatorModel.MS(ref _displayText);
            OnPropertyChanged(nameof(DisplayText));
        }
        private void MemoryRecall()
        {
            CalculatorModel.MR(ref _displayText);
            OnPropertyChanged(nameof(DisplayText));
        }

        // Operații suplimentare – aplică conversiile dacă suntem în modul Programmer
        private void Square()
        {
            try
            {
                if (IsProgrammerMode && _currentBase != 10)
                {
                    int value = ConvertToDecimal(DisplayText, _currentBase);
                    int result = (int)CalculatorModel.Pow(value);
                    DisplayText = ConvertFromDecimal(result, _currentBase);
                }
                else
                {
                    double value = double.Parse(DisplayText);
                    double result = CalculatorModel.Pow(value);
                    DisplayText = result.ToString();
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private void SquareRoot()
        {
            try
            {
                if (IsProgrammerMode && _currentBase != 10)
                {
                    int value = ConvertToDecimal(DisplayText, _currentBase);
                    int result = (int)CalculatorModel.Sqrt(value);
                    DisplayText = ConvertFromDecimal(result, _currentBase);
                }
                else
                {
                    double value = double.Parse(DisplayText);
                    double result = CalculatorModel.Sqrt(value);
                    DisplayText = result.ToString();
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private void Negate()
        {
            try
            {
                if (IsProgrammerMode && _currentBase != 10)
                {
                    int value = ConvertToDecimal(DisplayText, _currentBase);
                    int result = (int)CalculatorModel.Negate(value);
                    DisplayText = ConvertFromDecimal(result, _currentBase);
                }
                else
                {
                    double value = double.Parse(DisplayText);
                    double result = CalculatorModel.Negate(value);
                    DisplayText = result.ToString();
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        private void Reciprocal()
        {
            try
            {
                if (IsProgrammerMode && _currentBase != 10)
                {
                    int value = ConvertToDecimal(DisplayText, _currentBase);
                    int result = (int)CalculatorModel.Reciprocal(value);
                    DisplayText = ConvertFromDecimal(result, _currentBase);
                }
                else
                {
                    double value = double.Parse(DisplayText);
                    double result = CalculatorModel.Reciprocal(value);
                    DisplayText = result.ToString();
                }
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        // Funcție pentru setarea bazei de afișare – convertește numărul din baza curentă în noua bază
        public void SetBase(int newBase)
        {
            try
            {
                int decimalValue = ConvertToDecimal(DisplayText, _currentBase);
                _currentBase = newBase;
                DisplayText = ConvertFromDecimal(decimalValue, newBase);
                OnPropertyChanged(nameof(DisplayText));
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Proprietăți suplimentare pentru afișarea valorilor în modul Programmer
        private string _hexValue = "0";
        public string HexValue
        {
            get => _hexValue;
            set
            {
                if (_hexValue != value)
                {
                    _hexValue = value;
                    OnPropertyChanged(nameof(HexValue));
                }
            }
        }
        private string _decValue = "0";
        public string DecValue
        {
            get => _decValue;
            set
            {
                if (_decValue != value)
                {
                    _decValue = value;
                    OnPropertyChanged(nameof(DecValue));
                }
            }
        }
        private string _octValue = "0";
        public string OctValue
        {
            get => _octValue;
            set
            {
                if (_octValue != value)
                {
                    _octValue = value;
                    OnPropertyChanged(nameof(OctValue));
                }
            }
        }
        private string _binValue = "0";
        public string BinValue
        {
            get => _binValue;
            set
            {
                if (_binValue != value)
                {
                    _binValue = value;
                    OnPropertyChanged(nameof(BinValue));
                }
            }
        }
    }
}
