using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
        List<string> ledenLijst = new List<string>();
        int registratieCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            grdStart.Visibility = Visibility.Visible;
            grdLeden.Visibility = Visibility.Hidden;
            grdTrainingen.Visibility = Visibility.Hidden;
            grdSessies.Visibility = Visibility.Hidden;

        }

        public string LidEmail { get; set; }
        public string LidGSM { get; set; }


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

        // leden registratie code
        // opslaan input in multiline string
        private void btnRegistratie_Click(object sender, RoutedEventArgs e)
        {
            registratieCounter++;

            string nieuwLid = $@"
ID: {registratieCounter.ToString("D8")}
Naam: {txbNaam.Text}
Voornaam: {txbVoornaam.Text}
Rijksregisternummer: {txbLidRRN.Text}
Adres: {txbLidAdres.Text}
Telefoonnummer: {LidGSM}
Email-adres: {LidEmail}
Certificaat: {cbxCertificaat.Text}";


            ledenLijst.Add(nieuwLid);

            txtTest.Text = ledenLijst[0];
            txbNaam.Clear();
            txbVoornaam.Clear();
            txbLidRRN.Clear();
            txbLidAdres.Clear();
            txbLidEmail.Clear();    
            txbLidGSM.Clear();
            cbxCertificaat.SelectedIndex = -1;
        }

        private void btnResetTest_Click(object sender, RoutedEventArgs e)
        {
            ledenLijst.Clear();
        }
    }

    // Validatie input bij email/gsmnummers

    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string email = value as string;
            
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.IsMatch(email, pattern))
                return ValidationResult.ValidResult;
            
            return new ValidationResult(false, "Ongeldig e-mailadres.");
        }
    }

    public class GSMValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string gsm = value as string;

            string pattern = @"^(?:\+324\d{8}|04\d{8})$";

            if (Regex.IsMatch(gsm, pattern))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Ongeldig GSM-nummer.");
        }
    }



}