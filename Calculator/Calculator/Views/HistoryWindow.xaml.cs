using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Calculator.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(ObservableCollection<string> history)
        {
            InitializeComponent();
            DataContext = history;
        }

        private void historyList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (historyList.SelectedItem != null)
            {
                string selectedCalculation = historyList.SelectedItem.ToString();

                var parts = selectedCalculation.Split('=');
                if (parts.Length == 2)
                {
                    MessageBox.Show($"Ai selectat: {selectedCalculation}\nRezultat: {parts[1].Trim()}");
                }
            }
        }
    }
}
