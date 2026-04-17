using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfBubbelvrienden
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Training> trainingenLijst { get; set; } = new ObservableCollection<Training>();
        public ObservableCollection<Lid> ledenLijst { get; set; } = new ObservableCollection<Lid>();

        private int registratieCounter = 0;
        private int trainingenCounter = 0;

        private string[] tags = { "ABC", "DEF", "Open Water", "Zwembad", "Nachtduik" };
        private string[] niveau = { "1", "2", "3", "4", "5" };

        private Lid? geselecteerdLid;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            cmbTagsTraining.ItemsSource = tags;
            cmbNiveauTraining.ItemsSource = niveau;

            cbxCertificaat.SelectedIndex = 0;
            cmbTagsTraining.SelectedIndex = 0;
            cmbNiveauTraining.SelectedIndex = 0;

            grdStart.Visibility = Visibility.Visible;
            grdLeden.Visibility = Visibility.Collapsed;
            grdTrainingen.Visibility = Visibility.Collapsed;
            grdSessies.Visibility = Visibility.Collapsed;

            VernieuwLedenOutput();
            VernieuwTrainingenOutput();
        }

        private void ResetGrids()
        {
            grdStart.Visibility = Visibility.Collapsed;
            grdLeden.Visibility = Visibility.Collapsed;
            grdTrainingen.Visibility = Visibility.Collapsed;
            grdSessies.Visibility = Visibility.Collapsed;
        }

        private void btnMenuStart_Click(object sender, RoutedEventArgs e)
        {
            ResetGrids();
            grdStart.Visibility = Visibility.Visible;
        }

        private void btnMenuLeden_Click(object sender, RoutedEventArgs e)
        {
            ResetGrids();
            grdLeden.Visibility = Visibility.Visible;
        }

        private void btnMenuTrainingen_Click(object sender, RoutedEventArgs e)
        {
            ResetGrids();
            grdTrainingen.Visibility = Visibility.Visible;
        }

        private void btnMenuSessies_Click(object sender, RoutedEventArgs e)
        {
            ResetGrids();
            grdSessies.Visibility = Visibility.Visible;
        }

        private int GeselecteerdCertificaat()
        {
            ComboBoxItem? item = cbxCertificaat.SelectedItem as ComboBoxItem;
            if (item == null || item.Content == null)
            {
                return 1;
            }

            return int.Parse(item.Content.ToString() ?? "1");
        }

        private void btnRegistratie_Click(object sender, RoutedEventArgs e)
        {
            txtLedenFout.Text = "";

            string foutmelding = ValidatieHelper.ValideerLid(
                txbNaam.Text.Trim(),
                txbVoornaam.Text.Trim(),
                txbLidRRN.Text.Trim(),
                txbLidStreet.Text.Trim(),
                txbLidStreetNr.Text.Trim(),
                txbLidPostcode.Text.Trim(),
                txbLidGemeente.Text.Trim(),
                txbLidGSM.Text.Trim(),
                txbLidEmail.Text.Trim());

            if (foutmelding != "")
            {
                txtLedenFout.Text = foutmelding;
                return;
            }

            bool rrnBestaatAl = ledenLijst.Any(l => l.Rijksregisternummer == txbLidRRN.Text.Trim());
            if (rrnBestaatAl)
            {
                txtLedenFout.Text = "Er bestaat al een lid met dit rijksregisternummer.";
                return;
            }

            registratieCounter++;

            Lid lid = new Lid();
            lid.ID = registratieCounter.ToString("D8");
            lid.Naam = txbNaam.Text.Trim();
            lid.Voornaam = txbVoornaam.Text.Trim();
            lid.Rijksregisternummer = txbLidRRN.Text.Trim();
            lid.Straat = txbLidStreet.Text.Trim();
            lid.Huisnummer = txbLidStreetNr.Text.Trim();
            lid.Postcode = txbLidPostcode.Text.Trim();
            lid.Gemeente = txbLidGemeente.Text.Trim();
            lid.Telefoonnummer = txbLidGSM.Text.Trim();
            lid.Email = txbLidEmail.Text.Trim();
            lid.Certificaat = GeselecteerdCertificaat();

            ledenLijst.Add(lid);

            MaakLidFormulierLeeg();
            VernieuwLedenOutput();
            txtLedenFout.Text = "Lid succesvol toegevoegd.";
        }

        private void btnResetTest_Click(object sender, RoutedEventArgs e)
        {
            ledenLijst.Clear();
            registratieCounter = 0;
            geselecteerdLid = null;
            txtLedenFout.Text = "";
            txtTest.Text = "Geen leden geregistreerd.";
            txtSessieOutput.Text = "";
        }

        private void btnSearchID_Click(object sender, RoutedEventArgs e)
        {
            string id = txbMemberID.Text.Trim();

            geselecteerdLid = ledenLijst.FirstOrDefault(l => l.ID == id);

            if (geselecteerdLid == null)
            {
                MessageBox.Show("Geen lid gevonden met dit ID.");
                return;
            }

            txbNaam.Text = geselecteerdLid.Naam;
            txbVoornaam.Text = geselecteerdLid.Voornaam;
            txbLidRRN.Text = geselecteerdLid.Rijksregisternummer;
            txbLidStreet.Text = geselecteerdLid.Straat;
            txbLidStreetNr.Text = geselecteerdLid.Huisnummer;
            txbLidPostcode.Text = geselecteerdLid.Postcode;
            txbLidGemeente.Text = geselecteerdLid.Gemeente;
            txbLidGSM.Text = geselecteerdLid.Telefoonnummer;
            txbLidEmail.Text = geselecteerdLid.Email;
            cbxCertificaat.SelectedIndex = geselecteerdLid.Certificaat - 1;

            MessageBox.Show("Lid geladen. Je kan nu de gegevens aanpassen.");
            //zouden we hier de btnRegistratie best verbergen en als er gesaved wordt, opnieuw zichtbaar maken?
        }

        private void btnSaveNewID_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdLid == null)
            {
                MessageBox.Show("Geen lid geselecteerd om te bewerken.");
                return;
            }

            string foutmelding = ValidatieHelper.ValideerLid(
                txbNaam.Text.Trim(),
                txbVoornaam.Text.Trim(),
                txbLidRRN.Text.Trim(),
                txbLidStreet.Text.Trim(),
                txbLidStreetNr.Text.Trim(),
                txbLidPostcode.Text.Trim(),
                txbLidGemeente.Text.Trim(),
                txbLidGSM.Text.Trim(),
                txbLidEmail.Text.Trim());

            if (foutmelding != "")
            {
                txtLedenFout.Text = foutmelding;
                return;
            }

            bool rrnBestaatAl = ledenLijst.Any(l =>
                l.ID != geselecteerdLid.ID &&
                l.Rijksregisternummer == txbLidRRN.Text.Trim());

            if (rrnBestaatAl)
            {
                txtLedenFout.Text = "Dit rijksregisternummer bestaat al.";
                return;
            }

            geselecteerdLid.Naam = txbNaam.Text.Trim();
            geselecteerdLid.Voornaam = txbVoornaam.Text.Trim();
            geselecteerdLid.Rijksregisternummer = txbLidRRN.Text.Trim();
            geselecteerdLid.Straat = txbLidStreet.Text.Trim();
            geselecteerdLid.Huisnummer = txbLidStreetNr.Text.Trim();
            geselecteerdLid.Postcode = txbLidPostcode.Text.Trim();
            geselecteerdLid.Gemeente = txbLidGemeente.Text.Trim();
            geselecteerdLid.Telefoonnummer = txbLidGSM.Text.Trim();
            geselecteerdLid.Email = txbLidEmail.Text.Trim();
            geselecteerdLid.Certificaat = GeselecteerdCertificaat();

            VernieuwLedenOutput();
            txtLedenFout.Text = "Lid succesvol bijgewerkt.";
            MessageBox.Show("Lid succesvol bijgewerkt.");
            MaakLidFormulierLeeg();
        }

        private void btnImportCsv_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV-bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ledenLijst.Clear();

                    var importLeden = CsvHelper.ImporteerLeden(dialog.FileName);
                    foreach (var lid in importLeden)
                    {
                        ledenLijst.Add(lid);
                    }

                    if (ledenLijst.Count > 0)
                    {
                        registratieCounter = ledenLijst
                            .Select(l => int.TryParse(l.ID, out int id) ? id : 0)
                            .Max();
                    }
                    else
                    {
                        registratieCounter = 0;
                    }

                    VernieuwLedenOutput();
                    txtLedenFout.Text = "CSV succesvol geïmporteerd.";
                }
                catch (Exception ex)
                {
                    txtLedenFout.Text = "Fout bij importeren: " + ex.Message;
                }
            }
        }

        private void btnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV-bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*";
            dialog.FileName = "leden.csv";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    CsvHelper.ExporteerLeden(dialog.FileName, ledenLijst.ToList());
                    txtLedenFout.Text = "CSV succesvol geëxporteerd.";
                }
                catch (Exception ex)
                {
                    txtLedenFout.Text = "Fout bij exporteren: " + ex.Message;
                }
            }
        }

        private void VernieuwLedenOutput()
        {
            if (ledenLijst.Count == 0)
            {
                txtTest.Text = "Geen leden geregistreerd.";
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (Lid lid in ledenLijst)
            {
                sb.AppendLine(lid.ToString());
                sb.AppendLine("------------------------");
            }

            txtTest.Text = sb.ToString();
        }

        private void MaakLidFormulierLeeg()
        {
            txbNaam.Clear();
            txbVoornaam.Clear();
            txbLidRRN.Clear();
            txbLidStreet.Clear();
            txbLidStreetNr.Clear();
            txbLidPostcode.Clear();
            txbLidGemeente.Clear();
            txbLidEmail.Clear();
            txbLidGSM.Clear();
            txbMemberID.Clear();
            cbxCertificaat.SelectedIndex = 0;
            geselecteerdLid = null;
        }

        private void btnOpslaanTraining_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbNaamTraining.Text) ||
                string.IsNullOrWhiteSpace(txbTitelTraining.Text) ||
                string.IsNullOrWhiteSpace(txbInhoudTraining.Text) ||
                string.IsNullOrWhiteSpace(txbPlaatsTraining.Text) ||
                string.IsNullOrWhiteSpace(txbBeschikbarePlaatsenTraining.Text) ||
                string.IsNullOrWhiteSpace(txbDiepteTraining.Text) ||
                dapDatumTraining.SelectedDate == null)
            {
                txbTestOutputTraining.Text = "Vul alle velden van de training in.";
                return;
            }

            int beschikbarePlaatsen;
            int diepte;

            if (!int.TryParse(txbBeschikbarePlaatsenTraining.Text.Trim(), out beschikbarePlaatsen) || beschikbarePlaatsen <= 0)
            {
                txbTestOutputTraining.Text = "Beschikbare plaatsen moet een positief getal zijn.";
                return;
            }

            if (!int.TryParse(txbDiepteTraining.Text.Trim(), out diepte) || diepte <= 0)
            {
                txbTestOutputTraining.Text = "Diepte moet een positief getal zijn.";
                return;
            }

            trainingenCounter++;

            Training training = new Training();
            training.ID = trainingenCounter.ToString("D4");
            training.Naam = txbNaamTraining.Text.Trim();
            training.Titel = txbTitelTraining.Text.Trim();
            training.Tags = cmbTagsTraining.SelectedItem != null ? cmbTagsTraining.SelectedItem.ToString() ?? "" : "";
            training.Inhoud = txbInhoudTraining.Text.Trim();
            training.Plaats = txbPlaatsTraining.Text.Trim();
            training.Niveau = cmbNiveauTraining.SelectedItem != null ? cmbNiveauTraining.SelectedItem.ToString() ?? "" : "";
            training.Datum = dapDatumTraining.SelectedDate.Value;
            training.BeschikbarePlaatsen = beschikbarePlaatsen;
            training.Diepte = diepte;

            trainingenLijst.Add(training);

            VernieuwTrainingenOutput();
            MaakTrainingFormulierLeeg();
        }

        private void VernieuwTrainingenOutput()
        {
            if (trainingenLijst.Count == 0)
            {
                txbTestOutputTraining.Text = "Nog geen trainingen toegevoegd.";
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (Training training in trainingenLijst)
            {
                sb.AppendLine(training.ToString());
                sb.AppendLine("------------------------");
            }

            txbTestOutputTraining.Text = sb.ToString();
        }

        private void MaakTrainingFormulierLeeg()
        {
            txbNaamTraining.Clear();
            txbTitelTraining.Clear();
            txbInhoudTraining.Clear();
            txbPlaatsTraining.Clear();
            txbBeschikbarePlaatsenTraining.Clear();
            txbDiepteTraining.Clear();
            dapDatumTraining.SelectedDate = null;
            cmbTagsTraining.SelectedIndex = 0;
            cmbNiveauTraining.SelectedIndex = 0;
        }

        private void btnSelectieDuikers_Click(object sender, RoutedEventArgs e)
        {
            Training? gekozenTraining = lstTrainingen.SelectedItem as Training;

            if (gekozenTraining == null)
            {
                txtSessieOutput.Text = "Selecteer eerst een training.";
                return;
            }

            var geselecteerde = lstDuikers.SelectedItems.Cast<Lid>().ToList();

            if (geselecteerde.Count != 2)
            {
                txtSessieOutput.Text = "Selecteer precies 2 duikers.";
                return;
            }

            Lid d1 = geselecteerde[0];
            Lid d2 = geselecteerde[1];

            int som = d1.Certificaat + d2.Certificaat;

            bool geldig = false;

            if (som >= 5)
            {
                geldig = true;
            }
            else if (d1.Certificaat == 2 && d2.Certificaat == 2 && gekozenTraining.Diepte <= 20)
            {
                geldig = true;
            }

            if (geldig)
            {
                txtSessieOutput.Text =
                    d1.Voornaam + " en " + d2.Voornaam +
                    " vormen een geldig team voor training '" + gekozenTraining.Naam + "'.";
            }
            else
            {
                txtSessieOutput.Text =
                    "De geselecteerde duikers vormen geen geldig team voor deze training.";
            }
        }
    }
}