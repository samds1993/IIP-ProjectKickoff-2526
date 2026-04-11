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
        List<Lid> ledenLijst = new();
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

        public required string LidEmail { get; set; }
        public required string LidGSM { get; set; }


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
        // opslaan input met gebruik van een class
        public class Lid
        {
            public required string ID { get; set; }
            public required string Naam { get; set; }
            public required string Voornaam { get; set; }
            public required string Rijksregisternummer { get; set; }
            public required string Adres { get; set; }
            public required string Telefoonnummer { get; set; }
            public required string Email { get; set; }
            public required string Certificaat { get; set; }

            public override string ToString()
            {
                return $@"ID: {ID}
Naam: {Naam}
Voornaam: {Voornaam}
Rijksregisternummer: {Rijksregisternummer}
Adres: {Adres}
Telefoonnummer: {Telefoonnummer}
Email-adres: {Email}
Certificaat: {Certificaat}";
            }
        }



        private void btnRegistratie_Click(object sender, RoutedEventArgs e)
        {
            registratieCounter++;

            var lid = new Lid
            {
                ID = registratieCounter.ToString("D8"),
                Naam = txbNaam.Text,
                Voornaam = txbVoornaam.Text,
                Rijksregisternummer = txbLidRRN.Text,
                Adres = txbLidAdres.Text,
                Telefoonnummer = LidGSM,
                Email = LidEmail,
                Certificaat = cbxCertificaat.Text
            };


            ledenLijst.Add(lid);

            txtTest.Text = ledenLijst[0].ToString();

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

    // Validatie input bij email

    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? email = value as string;

            if (string.IsNullOrWhiteSpace(email))
                return new ValidationResult(false, "E-mail mag niet leeg zijn.");

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (Regex.IsMatch(email, pattern))
                return ValidationResult.ValidResult;
            
            return new ValidationResult(false, "Ongeldig e-mailadres.");
        }
    }

    // Validatie input bij GSM nummer
    public class GSMValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? gsm = value as string;

            if (string.IsNullOrWhiteSpace(gsm))
                return new ValidationResult(false, "GSM-nummer mag niet leeg zijn.");

            string pattern = @"^(?:\+324\d{8}|04\d{8})$";

            if (Regex.IsMatch(gsm, pattern))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Ongeldig GSM-nummer.");
        }
    }

    // Validatie input bij rijksregisternummer

    public class RijksregisterValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? rrn = value as string;

            if (string.IsNullOrWhiteSpace(rrn))
                return new ValidationResult(false, "Rijksregisternummer mag niet leeg zijn.");

            // Enkel cijfers en exact 11 karakters
            if (!Regex.IsMatch(rrn, @"^\d{11}$"))
                return new ValidationResult(false, "Rijksregisternummer moet uit 11 cijfers bestaan.");

            string baseNumber = rrn.Substring(0, 9);
            string controlDigits = rrn.Substring(9, 2);

            if (!long.TryParse(baseNumber, out long baseNum) ||
                !int.TryParse(controlDigits, out int control))
            {
                return new ValidationResult(false, "Ongeldig rijksregisternummer.");
            }

            // Controle voor personen geboren vóór 2000
            int expectedControl = 97 - (int)(baseNum % 97);

            if (expectedControl == control)
                return ValidationResult.ValidResult;

            // Controle voor personen geboren na 2000
            expectedControl = 97 - (int)((2_000_000_000L + baseNum) % 97);

            if (expectedControl == control)
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Ongeldig rijksregisternummer.");
        }
    }

    // registratie knop uitschakelen bij foute ingave

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(bool)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}