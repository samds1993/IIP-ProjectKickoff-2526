using System.Collections.ObjectModel;
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
    /// 
    public partial class MainWindow : Window
    {
        //NIEUW
        public ObservableCollection<Training> trainingenLijst { get; set; }
   = new ObservableCollection<Training>();

        public ObservableCollection<Lid> ledenLijst { get; set; }
    = new ObservableCollection<Lid>();

        //Lijst Verplaats met de public class hierboven, rest mag blijven staan
        int registratieCounter = 0;

        //Lijst Verplaats met de public class hierboven, rest mag blijven staan
        int trainingenCounter = 0;
        string[] tags = {"ABC", "DEF"};
        string[] niveau = { "A", "B", "C" };

       

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            //lijsten instellen
            //@Sam moeten we dit ook gebruiken voor de niveau's van de leden? Of stellen we het beter via de XAML in?
            cmbTagsTraining.ItemsSource = tags;
            cmbNiveauTraining.ItemsSource = niveau;


            grdStart.Visibility = Visibility.Visible;
            grdLeden.Visibility = Visibility.Hidden;
            grdTrainingen.Visibility = Visibility.Hidden;
            grdSessies.Visibility = Visibility.Hidden;

        }

        public required string LidEmail { get; set; }
        public required string LidGSM { get; set; }

        private Lid? geselecteerdLid;

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

            //AANGEPAST
            public required string Straat { get; set; }
            public required string Huisnummer { get; set; }
            public required string Postcode { get; set; }
            public required string Gemeente { get; set; }
            public required string Telefoonnummer { get; set; }
            public required string Email { get; set; }
            public required int Certificaat { get; set; }

            //AANGEPAST
            public override string ToString()
            {
                return $@"ID: {ID}
Naam: {Naam}
Voornaam: {Voornaam}
Rijksregisternummer: {Rijksregisternummer}
Straatnaam: {Straat}
Huisnummer: {Huisnummer}
Postcode: {Postcode}
Gemeente: {Gemeente}
Telefoonnummer: {Telefoonnummer}
Email-adres: {Email}
Certificaat: {Certificaat}";
            }
        }


        // AANGEPAST
        private void btnRegistratie_Click(object sender, RoutedEventArgs e)
        {
            registratieCounter++;

            var lid = new Lid
            {
                ID = registratieCounter.ToString("D8"),
                Naam = txbNaam.Text,
                Voornaam = txbVoornaam.Text,
                Rijksregisternummer = txbLidRRN.Text,
                Straat = txbLidStreet.Text,
                Huisnummer = txbLidStreetNr.Text,
                Postcode = txbLidPostcode.Text,
                Gemeente = txbLidGemeente.Text,               
                Telefoonnummer = LidGSM,
                Email = LidEmail,
                //AANGEPAST
                Certificaat = int.Parse(cbxCertificaat.Text)
            };


            ledenLijst.Add(lid);

            txtTest.Text = ledenLijst[^1].ToString();

            txbNaam.Clear();
            txbVoornaam.Clear();
            txbLidRRN.Clear();
            //AANGEPAST
            txbLidStreet.Clear();
            txbLidStreetNr.Clear();
            txbLidPostcode.Clear();
            txbLidGemeente.Clear();
            txbLidEmail.Clear();    
            txbLidGSM.Clear();
            cbxCertificaat.SelectedIndex = -1;
        }

        private void btnResetTest_Click(object sender, RoutedEventArgs e)
        {
            ledenLijst.Clear();
        }


        // Bewerken van leden informatie
        private void btnSearchID_Click(object sender, RoutedEventArgs e)
        {
            string id = txbMemberID.Text;

            geselecteerdLid = ledenLijst.FirstOrDefault(l => l.ID == id);

            if (geselecteerdLid == null)
            {
                MessageBox.Show("Geen lid gevonden met dit ID.");
                return;
            }

            txbNaam.Text = geselecteerdLid.Naam;
            txbVoornaam.Text = geselecteerdLid.Voornaam;
            txbLidRRN.Text = geselecteerdLid.Rijksregisternummer;
            //AANGEPAST
            txbLidStreet.Text = geselecteerdLid.Straat;
            txbLidStreetNr.Text = geselecteerdLid.Huisnummer;
            txbLidPostcode.Text = geselecteerdLid.Postcode;
            txbLidGemeente.Text = geselecteerdLid.Gemeente;
            txbLidGSM.Text = geselecteerdLid.Telefoonnummer;
            txbLidEmail.Text = geselecteerdLid.Email;
            cbxCertificaat.Text = geselecteerdLid.Certificaat.ToString();

            MessageBox.Show("Lid geladen. Je kan nu de gegevens aanpassen.");

        }

        private void btnSaveNewID_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdLid == null)
            {
                MessageBox.Show("Geen lid geselecteerd om te bewerken.");
                return;
            }

            geselecteerdLid.Naam = txbNaam.Text;
            geselecteerdLid.Voornaam = txbVoornaam.Text;
            geselecteerdLid.Rijksregisternummer = txbLidRRN.Text;
            //AANGEPAST
            geselecteerdLid.Straat = txbLidStreet.Text;
            geselecteerdLid.Huisnummer = txbLidStreetNr.Text;
            geselecteerdLid.Postcode = txbLidPostcode.Text;
            geselecteerdLid.Gemeente = txbLidGemeente.Text;
            geselecteerdLid.Telefoonnummer = txbLidGSM.Text;
            geselecteerdLid.Email = txbLidEmail.Text;
            geselecteerdLid.Certificaat = int.Parse(cbxCertificaat.Text);

            MessageBox.Show("Lid succesvol bijgewerkt.");

        }

        private void btnOpslaanTraining_Click(object sender, RoutedEventArgs e)
        {
            trainingenCounter++;

            Training training = new Training
            {
                ID = trainingenCounter.ToString("D4"),
                Naam = txbNaamTraining.Text,
                Titel = txbTitelTraining.Text,
                Tags = cmbTagsTraining.SelectedItem?.ToString() ?? "",
                Inhoud = txbInhoudTraining.Text,
                Plaats = txbPlaatsTraining.Text,
                Niveau = cmbNiveauTraining.SelectedItem?.ToString() ?? "",
                Datum = (dapDatumTraining.SelectedDate ?? DateTime.Now).ToString("dd/MM/yyyy"),
                BeschikbarePlaatsen = txbBeschikbarePlaatsenTraining.Text,
                Diepte = txbDiepteTraining.Text
            };

            trainingenLijst.Add(training);
            txbTestOutputTraining.Text = training.ToString();
        }

        //NIEUW
        // Registratie van trainingsessies
        private void btnSelectieDuikers_Click(object sender, RoutedEventArgs e)
        {
            var geselecteerde = lstDuikers.SelectedItems
        .Cast<Lid>()
        .ToList();

            if (geselecteerde.Count != 2)
            {
                MessageBox.Show("Selecteer precies 2 duikers.");
                return;
            }

            var d1 = geselecteerde[0];
            var d2 = geselecteerde[1];

            int som = d1.Certificaat + d2.Certificaat;

            if (som == 5)
            {
                MessageBox.Show($"{d1.Voornaam} en {d2.Voornaam} vormen samen 5 sterren ({d1.Certificaat} + {d2.Certificaat}).");
            }
            else
            {
                MessageBox.Show("De geselecteerde duikers vormen samen geen 5 sterren.");
            }

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

            if (!Regex.IsMatch(rrn, @"^\d{11}$"))
                return new ValidationResult(false, "Rijksregisternummer moet uit 11 cijfers bestaan.");

            string baseNumber = rrn.Substring(0, 9);
            string controlDigits = rrn.Substring(9, 2);

            if (!long.TryParse(baseNumber, out long baseNum) ||
                !int.TryParse(controlDigits, out int control))
            {
                return new ValidationResult(false, "Ongeldig rijksregisternummer.");
            }

            int expectedControl = 97 - (int)(baseNum % 97);

            if (expectedControl == control)
                return ValidationResult.ValidResult;

            expectedControl = 97 - (int)((2_000_000_000L + baseNum) % 97);

            if (expectedControl == control)
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Ongeldig rijksregisternummer.");
        }
    }
    //NIEUW - Validatie input bij postcode
    public class PostcodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? postcode = value as string;

            if (string.IsNullOrWhiteSpace(postcode))
                return new ValidationResult(false, "Postcode mag niet leeg zijn.");

            // Exact 4 cijfers
            string pattern = @"^\d{4}$";

            if (Regex.IsMatch(postcode, pattern))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Postcode moet uit 4 cijfers bestaan.");
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

    // training registratie code
    public class Training
    {
        public required string ID { get; set; }
        public required string Naam { get; set; }
        public required string Titel { get; set; }
        public required string Tags { get; set; }
        public required string Inhoud { get; set; }
        public required string Plaats { get; set; }
        public required string BeschikbarePlaatsen { get; set; }
        public required string Niveau { get; set; }
        public required string Diepte { get; set; }
        public required string Datum { get; set; }

        public override string ToString()
        {
            return $@"
ID: {ID}
Naam: {Naam}
Titel: {Titel}
Tags: {Tags}
Inhoud: {Inhoud}
Plaats: {Plaats}
Beschikbare plaatsen: {BeschikbarePlaatsen}
Niveau: {Niveau}
Diepte: {Diepte}
Datum: {Datum:dd/MM/yyyy}
";
        }

    }
}

