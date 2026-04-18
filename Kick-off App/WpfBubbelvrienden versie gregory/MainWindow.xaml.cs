using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfBubbelvrienden
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Training> trainingenLijst { get; set; } = new ObservableCollection<Training>();
        public ObservableCollection<Lid> ledenLijst { get; set; } = new ObservableCollection<Lid>();
        public ObservableCollection<Lid> beschikbareDuikersLijst { get; set; } = new ObservableCollection<Lid>();
        public ObservableCollection<TrainingsSessie> sessiesLijst { get; set; } = new ObservableCollection<TrainingsSessie>();

        private int registratieCounter = 0;
        private int trainingenCounter = 0;

        private string[] tags = { "Open Water", "Zwembad", "Nachtduik" };
        private string[] niveau = { "1", "2", "3", "4", "5" };

        private Lid? geselecteerdLid;
        private Training? geselecteerdeTraining;

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

            MaakStartData();
            BerekenTellers();
            VernieuwLedenOutput();
            VernieuwTrainingenOutput();
            VernieuwBeschikbareDuikers();
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
            BerekenTellers();
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
            VernieuwBeschikbareDuikers();
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
            VernieuwBeschikbareDuikers();
            txtLedenFout.Text = "Lid succesvol toegevoegd.";
        }

        private void btnVerhoogCertificaat_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdLid == null)
            {
                txtLedenFout.Text = "Zoek eerst een lid om het certificaat te verhogen.";
                return;
            }

            if (geselecteerdLid.Certificaat >= 5)
            {
                txtLedenFout.Text = "Dit lid heeft al het hoogste certificaat.";
                return;
            }

            geselecteerdLid.Certificaat++;
            cbxCertificaat.SelectedIndex = geselecteerdLid.Certificaat - 1;
            VernieuwLedenOutput();
            VernieuwBeschikbareDuikers();
            txtLedenFout.Text = "Certificaat verhoogd.";
        }

        private void btnVerwijderLid_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdLid == null)
            {
                txtLedenFout.Text = "Zoek eerst een lid om te verwijderen.";
                return;
            }

            for (int i = sessiesLijst.Count - 1; i >= 0; i--)
            {
                if (sessiesLijst[i].Duiker1ID == geselecteerdLid.ID || sessiesLijst[i].Duiker2ID == geselecteerdLid.ID)
                {
                    sessiesLijst.RemoveAt(i);
                }
            }

            ledenLijst.Remove(geselecteerdLid);
            geselecteerdLid = null;
            MaakLidFormulierLeeg();
            VernieuwLedenOutput();
            VernieuwBeschikbareDuikers();
            txtLedenFout.Text = "Lid verwijderd.";
        }

        private void btnResetTest_Click(object sender, RoutedEventArgs e)
        {
            ledenLijst.Clear();
            sessiesLijst.Clear();
            registratieCounter = 0;
            geselecteerdLid = null;
            txtLedenFout.Text = "";
            txtTest.Text = "Geen leden geregistreerd.";
            txtSessieOutput.Text = "";
            VernieuwBeschikbareDuikers();
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
            VernieuwBeschikbareDuikers();
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
                    VernieuwBeschikbareDuikers();
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
                string.IsNullOrWhiteSpace(txbStartUurTraining.Text) ||
                string.IsNullOrWhiteSpace(txbEindUurTraining.Text) ||
                dapDatumTraining.SelectedDate == null)
            {
                txtTrainingStatus.Text = "Vul alle velden van de training in.";
                return;
            }

            int beschikbarePlaatsen;
            int diepte;

            if (!int.TryParse(txbBeschikbarePlaatsenTraining.Text.Trim(), out beschikbarePlaatsen) || beschikbarePlaatsen <= 0)
            {
                txtTrainingStatus.Text = "Beschikbare plaatsen moet een positief getal zijn.";
                return;
            }

            if (!int.TryParse(txbDiepteTraining.Text.Trim(), out diepte) || diepte <= 0)
            {
                txtTrainingStatus.Text = "Diepte moet een positief getal zijn.";
                return;
            }

            TimeSpan startUur;
            TimeSpan eindUur;

            if (!TimeSpan.TryParse(txbStartUurTraining.Text.Trim(), out startUur))
            {
                txtTrainingStatus.Text = "Startuur is ongeldig. Gebruik bv. 18:00.";
                return;
            }

            if (!TimeSpan.TryParse(txbEindUurTraining.Text.Trim(), out eindUur))
            {
                txtTrainingStatus.Text = "Einduur is ongeldig. Gebruik bv. 20:00.";
                return;
            }

            if (eindUur <= startUur)
            {
                txtTrainingStatus.Text = "Einduur moet later zijn dan startuur.";
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
            training.StartUur = startUur;
            training.EindUur = eindUur;

            training.IngeschrevenStudenten.Add("Student A");
            training.IngeschrevenStudenten.Add("Student B");
            training.IngeschrevenStudenten.Add("Student C");

            trainingenLijst.Add(training);

            geselecteerdeTraining = training;
            lstIngeschrevenStudenten.ItemsSource = geselecteerdeTraining.IngeschrevenStudenten;
            lstGeaccepteerdeStudenten.ItemsSource = geselecteerdeTraining.GeaccepteerdeStudenten;

            VernieuwTrainingenOutput();
            MaakTrainingFormulierLeeg();
            txtTrainingStatus.Text = "Training succesvol toegevoegd.";
            BerekenTellers();
        }

        private void btnAccepteerStudent_Click(object sender, RoutedEventArgs e)
        {
            if (geselecteerdeTraining == null)
            {
                txtTrainingStatus.Text = "Selecteer eerst een training.";
                return;
            }

            if (lstIngeschrevenStudenten.SelectedItem == null)
            {
                txtTrainingStatus.Text = "Selecteer eerst een ingeschreven student.";
                return;
            }

            if (geselecteerdeTraining.IsVol)
            {
                txtTrainingStatus.Text = "Deze training is al volzet.";
                return;
            }

            string student = lstIngeschrevenStudenten.SelectedItem.ToString() ?? "";
            if (student == "")
            {
                return;
            }

            geselecteerdeTraining.IngeschrevenStudenten.Remove(student);
            geselecteerdeTraining.GeaccepteerdeStudenten.Add(student);

            lstIngeschrevenStudenten.ItemsSource = null;
            lstIngeschrevenStudenten.ItemsSource = geselecteerdeTraining.IngeschrevenStudenten;

            lstGeaccepteerdeStudenten.ItemsSource = null;
            lstGeaccepteerdeStudenten.ItemsSource = geselecteerdeTraining.GeaccepteerdeStudenten;

            if (geselecteerdeTraining.IsVol)
            {
                txtTrainingStatus.Text = "Deze training is nu volzet.";
            }
            else
            {
                txtTrainingStatus.Text = "Student geaccepteerd.";
            }

            VernieuwTrainingenOutput();
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
                sb.AppendLine("Geaccepteerd: " + training.GeaccepteerdeStudenten.Count + "/" + training.BeschikbarePlaatsen);
                sb.AppendLine("Volzet: " + (training.IsVol ? "Ja" : "Nee"));
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
            txbStartUurTraining.Clear();
            txbEindUurTraining.Clear();
            dapDatumTraining.SelectedDate = null;
            cmbTagsTraining.SelectedIndex = 0;
            cmbNiveauTraining.SelectedIndex = 0;
        }

        private void lstTrainingen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            geselecteerdeTraining = lstTrainingen.SelectedItem as Training;
            VernieuwBeschikbareDuikers();

            if (geselecteerdeTraining != null)
            {
                lstIngeschrevenStudenten.ItemsSource = null;
                lstIngeschrevenStudenten.ItemsSource = geselecteerdeTraining.IngeschrevenStudenten;

                lstGeaccepteerdeStudenten.ItemsSource = null;
                lstGeaccepteerdeStudenten.ItemsSource = geselecteerdeTraining.GeaccepteerdeStudenten;
            }
        }

        private void VernieuwBeschikbareDuikers()
        {
            beschikbareDuikersLijst.Clear();

            if (geselecteerdeTraining == null)
            {
                foreach (var lid in ledenLijst)
                {
                    beschikbareDuikersLijst.Add(lid);
                }
                return;
            }

            var bezetteIds = sessiesLijst
                .Where(s => s.TrainingID == geselecteerdeTraining.ID)
                .SelectMany(s => new[] { s.Duiker1ID, s.Duiker2ID })
                .ToList();

            foreach (var lid in ledenLijst)
            {
                if (!bezetteIds.Contains(lid.ID))
                {
                    beschikbareDuikersLijst.Add(lid);
                }
            }
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

            if (!geldig)
            {
                txtSessieOutput.Text = "De geselecteerde duikers vormen geen geldig team voor deze training.";
                return;
            }

            TrainingsSessie sessie = new TrainingsSessie();
            sessie.TrainingID = gekozenTraining.ID;
            sessie.TrainingNaam = gekozenTraining.Naam;
            sessie.Duiker1ID = d1.ID;
            sessie.Duiker2ID = d2.ID;
            sessie.Duiker1Naam = d1.Voornaam + " " + d1.Naam;
            sessie.Duiker2Naam = d2.Voornaam + " " + d2.Naam;
            sessie.Omschrijving = gekozenTraining.Naam + " - " + sessie.Duiker1Naam + " & " + sessie.Duiker2Naam;

            sessiesLijst.Add(sessie);
            VernieuwBeschikbareDuikers();

            txtSessieOutput.Text =
                d1.Voornaam + " en " + d2.Voornaam +
                " vormen een geldig team voor training '" + gekozenTraining.Naam + "' en de sessie werd opgeslagen.";
        }

        private void MaakStartData()
        {
            ledenLijst.Add(new Lid
            {
                ID = "00000001",
                Naam = "Gaudeus",
                Voornaam = "Gregory",
                Rijksregisternummer = "92071435740",
                Straat = "Kerkstraat",
                Huisnummer = "10",
                Postcode = "9000",
                Gemeente = "Gent",
                Telefoonnummer = "0412345678",
                Email = "gregory.gaudeus@student.odisee.be",
                Certificaat = 2
            });

            ledenLijst.Add(new Lid
            {
                ID = "00000002",
                Naam = "De Smet",
                Voornaam = "Sam",
                Rijksregisternummer = "92030554321",
                Straat = "Dorpstraat",
                Huisnummer = "25",
                Postcode = "9300",
                Gemeente = "Aalst",
                Telefoonnummer = "0423456789",
                Email = "sam.desmet2@student.odisee.be",
                Certificaat = 3
            });

            ledenLijst.Add(new Lid
            {
                ID = "00000003",
                Naam = "Knops",
                Voornaam = "Wout",
                Rijksregisternummer = "88071298765",
                Straat = "Stationsstraat",
                Huisnummer = "3",
                Postcode = "1770",
                Gemeente = "Liedekerke",
                Telefoonnummer = "0434567890",
                Email = "wout.knops@student.odisee.be",
                Certificaat = 1
            });

            registratieCounter = ledenLijst.Count;

            Training training1 = new Training();
            training1.ID = "0001";
            training1.Naam = "Basis Open Water";
            training1.Titel = "Introductieduik";
            training1.Tags = "Open Water";
            training1.Inhoud = "Eerste kennismaking met duiken in open water";
            training1.Plaats = "Zilvermeer";
            training1.Niveau = "1";
            training1.Datum = DateTime.Today.AddDays(7);
            training1.BeschikbarePlaatsen = 2;
            training1.Diepte = 5;
            training1.StartUur = new TimeSpan(18, 0, 0);
            training1.EindUur = new TimeSpan(20, 0, 0);
            training1.IngeschrevenStudenten.Add("Student A");
            training1.IngeschrevenStudenten.Add("Student B");
            training1.IngeschrevenStudenten.Add("Student C");
            trainingenLijst.Add(training1);

            Training training2 = new Training();
            training2.ID = "0002";
            training2.Naam = "Nachtduik Specialisatie";
            training2.Titel = "Duiken bij duisternis";
            training2.Tags = "Nachtduik";
            training2.Inhoud = "Geavanceerde training voor nachtduiken";
            training2.Plaats = "Put van Ekeren";
            training2.Niveau = "3";
            training2.Datum = DateTime.Today.AddDays(14);
            training2.BeschikbarePlaatsen = 6;
            training2.Diepte = 25;
            training2.StartUur = new TimeSpan(19, 0, 0);
            training2.EindUur = new TimeSpan(22, 0, 0);
            training2.IngeschrevenStudenten.Add("Student X");
            training2.IngeschrevenStudenten.Add("Student Y");
            trainingenLijst.Add(training2);

            trainingenCounter = trainingenLijst.Count;
        }

        private void BerekenTellers()
        {
            lblAantalLeden.Content = ledenLijst.Count + " actieve leden";
            lblAantalTrainingen.Content = trainingenLijst.Count + " trainingen";
        }
    }
}