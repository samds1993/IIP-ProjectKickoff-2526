using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfBubbelvrienden
{
    public partial class MainWindow : Window
    {
        private List<MemberRecord> leden = new List<MemberRecord>();
        private List<Training> trainingen = new List<Training>();
        private int volgendeMemberId = 1;
        private MemberRecord? huidigBewerktLid = null;

        public MainWindow()
        {
            InitializeComponent();

            cbxCertificaat.SelectedIndex = 0;
            cmbTagsTraining.SelectedIndex = 0;
            cmbNiveauTraining.SelectedIndex = 0;

            ToonStart();
            VernieuwLedenOverzicht();
            VernieuwTrainingenOverzicht();
        }

        private void VerbergAlleSchermen()
        {
            grdStart.Visibility = Visibility.Collapsed;
            grdLeden.Visibility = Visibility.Collapsed;
            grdTrainingen.Visibility = Visibility.Collapsed;
        }

        private void ToonStart()
        {
            VerbergAlleSchermen();
            grdStart.Visibility = Visibility.Visible;
        }

        private void ToonLeden()
        {
            VerbergAlleSchermen();
            grdLeden.Visibility = Visibility.Visible;
        }

        private void ToonTrainingen()
        {
            VerbergAlleSchermen();
            grdTrainingen.Visibility = Visibility.Visible;
        }

        private void btnMenuStart_Click(object sender, RoutedEventArgs e)
        {
            ToonStart();
        }

        private void btnMenuLeden_Click(object sender, RoutedEventArgs e)
        {
            ToonLeden();
        }

        private void btnMenuTrainingen_Click(object sender, RoutedEventArgs e)
        {
            ToonTrainingen();
        }

        private int GeselecteerdeSterren()
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

            string fout = ValidatieHelper.ValideerLid(
                txbNaam.Text.Trim(),
                txbVoornaam.Text.Trim(),
                txbLidRRN.Text.Trim(),
                txbLidAdres.Text.Trim(),
                txbLidGSM.Text.Trim(),
                txbLidEmail.Text.Trim());

            if (fout != "")
            {
                txtLedenFout.Text = fout;
                return;
            }

            bool rrnBestaatAl = leden.Any(l => l.Rijksregisternummer == txbLidRRN.Text.Trim());
            if (rrnBestaatAl)
            {
                txtLedenFout.Text = "Er bestaat al een lid met dit rijksregisternummer.";
                return;
            }

            MemberRecord lid = new MemberRecord();
            lid.MemberId = volgendeMemberId;
            lid.Naam = txbNaam.Text.Trim();
            lid.Voornaam = txbVoornaam.Text.Trim();
            lid.Rijksregisternummer = txbLidRRN.Text.Trim();
            lid.Adres = txbLidAdres.Text.Trim();
            lid.Telefoonnummer = txbLidGSM.Text.Trim();
            lid.Emailadres = txbLidEmail.Text.Trim();
            lid.Sterren = GeselecteerdeSterren();

            leden.Add(lid);
            volgendeMemberId++;

            MaakLedenFormulierLeeg();
            VernieuwLedenOverzicht();
            txtLedenFout.Text = "Lid succesvol toegevoegd.";
        }

        private void btnResetTest_Click(object sender, RoutedEventArgs e)
        {
            leden.Clear();
            volgendeMemberId = 1;
            huidigBewerktLid = null;
            MaakLedenFormulierLeeg();
            VernieuwLedenOverzicht();
            txtLedenFout.Text = "Ledenlijst werd gereset.";
        }

        private void btnSearchID_Click(object sender, RoutedEventArgs e)
        {
            txtLedenFout.Text = "";

            int gezochtId;
            if (!int.TryParse(txbMemberID.Text.Trim(), out gezochtId))
            {
                txtLedenFout.Text = "Geef een geldig numeriek lid-ID in.";
                return;
            }

            MemberRecord? gevonden = leden.FirstOrDefault(l => l.MemberId == gezochtId);
            if (gevonden == null)
            {
                txtLedenFout.Text = "Geen lid gevonden met dit ID.";
                return;
            }

            huidigBewerktLid = gevonden;

            txbNaam.Text = gevonden.Naam;
            txbVoornaam.Text = gevonden.Voornaam;
            txbLidRRN.Text = gevonden.Rijksregisternummer;
            txbLidAdres.Text = gevonden.Adres;
            txbLidGSM.Text = gevonden.Telefoonnummer;
            txbLidEmail.Text = gevonden.Emailadres;
            cbxCertificaat.SelectedIndex = gevonden.Sterren - 1;

            txtLedenFout.Text = "Lid geladen. Pas de velden aan en klik op Opslaan.";
        }

        private void btnSaveNewID_Click(object sender, RoutedEventArgs e)
        {
            txtLedenFout.Text = "";

            if (huidigBewerktLid == null)
            {
                txtLedenFout.Text = "Zoek eerst een lid-ID op.";
                return;
            }

            string fout = ValidatieHelper.ValideerLid(
                txbNaam.Text.Trim(),
                txbVoornaam.Text.Trim(),
                txbLidRRN.Text.Trim(),
                txbLidAdres.Text.Trim(),
                txbLidGSM.Text.Trim(),
                txbLidEmail.Text.Trim());

            if (fout != "")
            {
                txtLedenFout.Text = fout;
                return;
            }

            bool rrnBestaatAl = leden.Any(l =>
                l.MemberId != huidigBewerktLid.MemberId &&
                l.Rijksregisternummer == txbLidRRN.Text.Trim());

            if (rrnBestaatAl)
            {
                txtLedenFout.Text = "Dit rijksregisternummer wordt al gebruikt door een ander lid.";
                return;
            }

            huidigBewerktLid.Naam = txbNaam.Text.Trim();
            huidigBewerktLid.Voornaam = txbVoornaam.Text.Trim();
            huidigBewerktLid.Rijksregisternummer = txbLidRRN.Text.Trim();
            huidigBewerktLid.Adres = txbLidAdres.Text.Trim();
            huidigBewerktLid.Telefoonnummer = txbLidGSM.Text.Trim();
            huidigBewerktLid.Emailadres = txbLidEmail.Text.Trim();
            huidigBewerktLid.Sterren = GeselecteerdeSterren();

            VernieuwLedenOverzicht();
            txtLedenFout.Text = "Lid succesvol aangepast.";
        }

        private void btnImportCsv_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV-bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    leden = CsvHelper.ImporteerLeden(dialog.FileName);
                    if (leden.Count > 0)
                    {
                        volgendeMemberId = leden.Max(l => l.MemberId) + 1;
                    }
                    else
                    {
                        volgendeMemberId = 1;
                    }

                    VernieuwLedenOverzicht();
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
                    CsvHelper.ExporteerLeden(dialog.FileName, leden);
                    txtLedenFout.Text = "CSV succesvol geëxporteerd.";
                }
                catch (Exception ex)
                {
                    txtLedenFout.Text = "Fout bij exporteren: " + ex.Message;
                }
            }
        }

        private void VernieuwLedenOverzicht()
        {
            if (leden.Count == 0)
            {
                txtTest.Text = "Geen leden geregistreerd.";
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (MemberRecord lid in leden)
            {
                sb.AppendLine("ID: " + lid.MemberId);
                sb.AppendLine("Naam: " + lid.Voornaam + " " + lid.Naam);
                sb.AppendLine("RRN: " + lid.Rijksregisternummer);
                sb.AppendLine("Adres: " + lid.Adres);
                sb.AppendLine("Tel: " + lid.Telefoonnummer);
                sb.AppendLine("Email: " + lid.Emailadres);
                sb.AppendLine("Sterren: " + lid.Sterren);
                sb.AppendLine("------------------------");
            }

            txtTest.Text = sb.ToString();
        }

        private void MaakLedenFormulierLeeg()
        {
            txbNaam.Text = "";
            txbVoornaam.Text = "";
            txbLidRRN.Text = "";
            txbLidAdres.Text = "";
            txbLidGSM.Text = "";
            txbLidEmail.Text = "";
            txbMemberID.Text = "";
            cbxCertificaat.SelectedIndex = 0;
            huidigBewerktLid = null;
        }

        private int GeselecteerdNiveauTraining()
        {
            ComboBoxItem? item = cmbNiveauTraining.SelectedItem as ComboBoxItem;
            if (item == null || item.Content == null)
            {
                return 1;
            }

            return int.Parse(item.Content.ToString() ?? "1");
        }

        private string GeselecteerdeTagTraining()
        {
            ComboBoxItem? item = cmbTagsTraining.SelectedItem as ComboBoxItem;
            if (item == null || item.Content == null)
            {
                return "";
            }

            return item.Content.ToString() ?? "";
        }

        private void btnTrainingToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbNaamTraining.Text) ||
                string.IsNullOrWhiteSpace(txbTitelTraining.Text) ||
                string.IsNullOrWhiteSpace(txbInhoudTraining.Text) ||
                string.IsNullOrWhiteSpace(txbPlaatsTraining.Text) ||
                string.IsNullOrWhiteSpace(txbBeschikbarePlaatsenTraining.Text) ||
                string.IsNullOrWhiteSpace(txbDiepteTraining.Text) ||
                dapDatumTraining.SelectedDate == null)
            {
                txtTrainingenInfo.Text = "Vul alle velden van de training in.";
                return;
            }

            int beschikbarePlaatsen;
            int diepte;

            if (!int.TryParse(txbBeschikbarePlaatsenTraining.Text.Trim(), out beschikbarePlaatsen) || beschikbarePlaatsen <= 0)
            {
                txtTrainingenInfo.Text = "Beschikbare plaatsen moet een positief getal zijn.";
                return;
            }

            if (!int.TryParse(txbDiepteTraining.Text.Trim(), out diepte) || diepte <= 0)
            {
                txtTrainingenInfo.Text = "Diepte moet een positief getal zijn.";
                return;
            }

            Training training = new Training();
            training.Naam = txbNaamTraining.Text.Trim();
            training.Titel = txbTitelTraining.Text.Trim();
            training.Tags = GeselecteerdeTagTraining();
            training.Inhoud = txbInhoudTraining.Text.Trim();
            training.Plaats = txbPlaatsTraining.Text.Trim();
            training.BeschikbarePlaatsen = beschikbarePlaatsen;
            training.Niveau = GeselecteerdNiveauTraining();
            training.Diepte = diepte;
            training.Datum = dapDatumTraining.SelectedDate.Value;

            trainingen.Add(training);

            VernieuwTrainingenOverzicht();
            txtTrainingenInfo.Text = "Training toegevoegd." + Environment.NewLine + Environment.NewLine + txtTrainingenInfo.Text;
            MaakTrainingFormulierLeeg();
        }

        private void VernieuwTrainingenOverzicht()
        {
            if (trainingen.Count == 0)
            {
                txtTrainingenInfo.Text = "Nog geen trainingen toegevoegd.";
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (Training training in trainingen)
            {
                sb.AppendLine("Naam: " + training.Naam);
                sb.AppendLine("Titel: " + training.Titel);
                sb.AppendLine("Tags: " + training.Tags);
                sb.AppendLine("Inhoud: " + training.Inhoud);
                sb.AppendLine("Plaats: " + training.Plaats);
                sb.AppendLine("Beschikbare plaatsen: " + training.BeschikbarePlaatsen);
                sb.AppendLine("Niveau: " + training.Niveau);
                sb.AppendLine("Diepte: " + training.Diepte);
                sb.AppendLine("Datum: " + training.Datum.ToShortDateString());
                sb.AppendLine("------------------------");
            }

            txtTrainingenInfo.Text = sb.ToString();
        }

        private void MaakTrainingFormulierLeeg()
        {
            txbNaamTraining.Text = "";
            txbTitelTraining.Text = "";
            txbInhoudTraining.Text = "";
            txbPlaatsTraining.Text = "";
            txbBeschikbarePlaatsenTraining.Text = "";
            txbDiepteTraining.Text = "";
            dapDatumTraining.SelectedDate = null;
            cmbTagsTraining.SelectedIndex = 0;
            cmbNiveauTraining.SelectedIndex = 0;
        }
    }
}