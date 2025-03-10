using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Calculator.Models;

/// <summary>
/// Gestionarea starii si comportamentului aplicatiei
/// 
/// </summary>
namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private CalculatorModel _calculator = new CalculatorModel(); // Eroare: trebuie să fie static


        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        // Definire comenzi
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
        }

        private void AppendNumber(string number)
        {
            if (_displayText == "0") _displayText = number;
            else _displayText += number;
            OnPropertyChanged(nameof(DisplayText));
        }

        private void SetOperator(string op)
        {
            _displayText += $" {op} ";
            OnPropertyChanged(nameof(DisplayText));
        }

        private void CalculateResult()
        {
            try
            {
                string[] parts = _displayText.Split(' '); // Ex: "5 + 3"
                if (parts.Length < 3) return;

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
            catch (Exception ex)
            {
                DisplayText = "Eroare";
            }
        }



        private void Clear() => DisplayText = "0";
        private void ClearEntry() => DisplayText = "0";
        private void Backspace() { if (_displayText.Length > 0) DisplayText = _displayText[..^1]; }
        private void MemoryClear() => CalculatorModel.MC();
        private void MemoryPlus() => CalculatorModel.MPlus(ref _displayText);
        private void MemoryMinus() => CalculatorModel.MMinus(ref _displayText);
        private void MemoryStore() => CalculatorModel.MS(ref _displayText);
        private void MemoryRecall() { CalculatorModel.MR(ref _displayText); OnPropertyChanged(nameof(DisplayText)); }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));




        private int _currentBase = 10;

        public void SetBase(int newBase)
        {
            try
            {
                int decimalValue = Convert.ToInt32(DisplayText, _currentBase);
                _currentBase = newBase;
                DisplayText = Convert.ToString(decimalValue, newBase).ToUpper();
                OnPropertyChanged(nameof(DisplayText));
            }
            catch
            {
                DisplayText = "Eroare";
            }
        }



    }
}