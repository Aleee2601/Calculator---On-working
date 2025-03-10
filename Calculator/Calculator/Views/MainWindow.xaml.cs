using System.Globalization;
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
            DataContext = new CalculatorViewModel();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Mode_Checked(null, null);
        }





        //private void Base_Checked(object sender, RoutedEventArgs e)
        //{
        //    var button = sender as RadioButton;
        //    if (button != null && button.Tag != null)
        //    {
        //        int newBase = int.Parse(button.Tag.ToString());
        //        (DataContext as CalculatorViewModel)?.SetBase(newBase);
        //    }
        //}


        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            if (btnProgrammer == null || ProgrammerPanel == null || HexButtonsPanel == null)
                return;

            if (btnProgrammer.IsChecked == true)
            {
                ProgrammerPanel.Visibility = Visibility.Visible;
                HexButtonsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ProgrammerPanel.Visibility = Visibility.Collapsed;
                HexButtonsPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void Base_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is CalculatorViewModel vm && sender is RadioButton rb && rb.Tag != null)
            {
                int newBase = int.Parse(rb.Tag.ToString());
                vm.SetBase(newBase);

                // Show HEX buttons only if base is 16
                Visibility hexVisibility = (newBase == 16) ? Visibility.Visible : Visibility.Collapsed;
                btnA.Visibility = hexVisibility;
                btnB.Visibility = hexVisibility;
                btnC.Visibility = hexVisibility;
                btnD.Visibility = hexVisibility;
                btnE.Visibility = hexVisibility;
                btnF.Visibility = hexVisibility;
            }
        }





    }
}