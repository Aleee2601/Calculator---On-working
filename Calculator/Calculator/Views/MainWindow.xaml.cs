using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Calculator.ViewModels;

namespace Calculator.Views
{
/// <summary>
/// Interaction logic for MainWindow.xaml\
/// Nu trb sa contina logica
/// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CalculatorViewModel(); // Setează ViewModel-ul
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            

        }
    }
}