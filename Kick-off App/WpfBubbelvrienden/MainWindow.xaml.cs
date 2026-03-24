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

namespace WpfBubbelvrienden
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            grdStart.Visibility = Visibility.Visible;
            grdLeden.Visibility = Visibility.Hidden;
            grdTrainingen.Visibility = Visibility.Hidden;
            grdSessies.Visibility = Visibility.Hidden;
        }

        private void resetgrids()
        {
            grdStart.Visibility = Visibility.Hidden;
            grdLeden.Visibility = Visibility.Hidden;
            grdTrainingen.Visibility = Visibility.Hidden;
            grdSessies.Visibility = Visibility.Hidden;
        }
        private void btnMenuStart_Click(object sender, RoutedEventArgs e)
        {
            resetgrids();
            grdStart.Visibility = Visibility.Visible;
        }

        private void btnMenuLeden_Click(object sender, RoutedEventArgs e)
        {
            resetgrids();
            grdLeden.Visibility = Visibility.Visible;
        }

        private void btnMenuTrainingen_Click(object sender, RoutedEventArgs e)
        {
            resetgrids();
            grdTrainingen.Visibility = Visibility.Visible;
        }

        private void btnMenuSessies_Click(object sender, RoutedEventArgs e)
        {
            resetgrids();
            grdSessies.Visibility = Visibility.Visible;
        }

        private void btnRegistratie_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}