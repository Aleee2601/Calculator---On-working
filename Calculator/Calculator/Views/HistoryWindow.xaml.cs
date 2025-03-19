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
            // Setăm contextul de date la colecția de istoric
            DataContext = history;
        }

        private void historyList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (historyList.SelectedItem != null)
            {
                // La dublu click se preia elementul selectat.
                // De exemplu, dacă se dorește ca rezultatul calculului să fie afișat în display:
                string selectedCalculation = historyList.SelectedItem.ToString();
                // Extragem partea de după '='
                var parts = selectedCalculation.Split('=');
                if (parts.Length == 2)
                {
                    // Trimitem rezultatul în fereastra principală
                    // Aici poți implementa o metodă prin care fereastra principală primește rezultatul.
                    MessageBox.Show($"Ai selectat: {selectedCalculation}\nRezultat: {parts[1].Trim()}");
                }
            }
        }
    }
}
