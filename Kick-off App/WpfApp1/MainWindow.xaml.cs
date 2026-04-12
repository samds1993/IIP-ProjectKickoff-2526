using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfBubbelvrienden
{
    public partial class MainWindow : Window
    {
        private List<Training> trainingen = new List<Training>();
        private Training geselecteerdeTraining = null;

        public MainWindow()
        {
            InitializeComponent();

            cbxTrainingNiveau.SelectedIndex = 0;
            cbxTags.SelectedIndex = 0;

            VoegFictieveTrainingenToe();
            ToonStart();
            RefreshAlles();
        }

        // ---------------- SCHERMEN ----------------

        private void VerbergAlleSchermen()
        {
            grdStart.Visibility = Visibility.Collapsed;
            grdTrainingen.Visibility = Visibility.Collapsed;
        }

        private void ToonStart()
        {
            VerbergAlleSchermen();
            grdStart.Visibility = Visibility.Visible;
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

        private void btnMenuTrainingen_Click(object sender, RoutedEventArgs e)
        {
            ToonTrainingen();
        }

        // ---------------- REFRESH ----------------

        private void RefreshAlles()
        {
            dgTrainingen.ItemsSource = null;
            dgTrainingen.ItemsSource = trainingen;

            RefreshStudentenLijsten();
        }

        private void RefreshStudentenLijsten()
        {
            if (geselecteerdeTraining != null)
            {
                lstIngeschrevenStudenten.ItemsSource = null;
                lstIngeschrevenStudenten.ItemsSource = geselecteerdeTraining.IngeschrevenStudenten;

                lstGeaccepteerdeStudenten.ItemsSource = null;
                lstGeaccepteerdeStudenten.ItemsSource = geselecteerdeTraining.GeaccepteerdeStudenten;

                if (geselecteerdeTraining.IsVol)
                {
                    txtTrainingStatus.Text = "Deze training is vol.";
                }
                else
                {
                    txtTrainingStatus.Text = "";
                }
            }
            else
            {
                lstIngeschrevenStudenten.ItemsSource = null;
                lstGeaccepteerdeStudenten.ItemsSource = null;
            }
        }

        // ---------------- TRAININGEN ----------------

        private int GeselecteerdTrainingNiveau()
        {
            ComboBoxItem item = cbxTrainingNiveau.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                return int.Parse(item.Content.ToString());
            }

            return 1;
        }

        private string GeselecteerdeTag()
        {
            ComboBoxItem item = cbxTags.SelectedItem as ComboBoxItem;
            if (item != null)
            {
                return item.Content.ToString();
            }

            return "";
        }

        private void MaakTrainingFormulierLeeg()
        {
            txbTrainingNaam.Text = "";
            txbTrainingTitel.Text = "";
            cbxTags.SelectedIndex = 0;
            txbTrainingInhoud.Text = "";
            txbTrainingStad.Text = "";
            txbTrainingProvincie.Text = "";
            txbBeschikbarePlaatsen.Text = "";
            cbxTrainingNiveau.SelectedIndex = 0;
            txbDiepte.Text = "";
            dpTrainingDatum.SelectedDate = null;
            txbStartuur.Text = "";
            txbEinduur.Text = "";
            txtTrainingStatus.Text = "";
            dgTrainingen.SelectedItem = null;
            geselecteerdeTraining = null;
            RefreshStudentenLijsten();
        }

        private void btnTrainingToevoegen_Click(object sender, RoutedEventArgs e)
        {
            txtTrainingStatus.Text = "";

            try
            {
                if (string.IsNullOrWhiteSpace(txbTrainingNaam.Text) ||
                    string.IsNullOrWhiteSpace(txbTrainingTitel.Text) ||
                    string.IsNullOrWhiteSpace(txbTrainingInhoud.Text) ||
                    string.IsNullOrWhiteSpace(txbTrainingStad.Text) ||
                    string.IsNullOrWhiteSpace(txbTrainingProvincie.Text) ||
                    string.IsNullOrWhiteSpace(txbBeschikbarePlaatsen.Text) ||
                    string.IsNullOrWhiteSpace(txbDiepte.Text) ||
                    string.IsNullOrWhiteSpace(txbStartuur.Text) ||
                    string.IsNullOrWhiteSpace(txbEinduur.Text) ||
                    dpTrainingDatum.SelectedDate == null)
                {
                    txtTrainingStatus.Text = "Alle velden van de training moeten ingevuld worden.";
                    return;
                }

                Training training = new Training();
                training.Naam = txbTrainingNaam.Text.Trim();
                training.Titel = txbTrainingTitel.Text.Trim();
                training.Tags = GeselecteerdeTag();
                training.Inhoud = txbTrainingInhoud.Text.Trim();
                training.Stad = txbTrainingStad.Text.Trim();
                training.Provincie = txbTrainingProvincie.Text.Trim();
                training.BeschikbarePlaatsen = int.Parse(txbBeschikbarePlaatsen.Text);
                training.Niveau = GeselecteerdTrainingNiveau();
                training.Diepte = int.Parse(txbDiepte.Text);
                training.Datum = dpTrainingDatum.SelectedDate.Value;
                training.StartUur = TimeSpan.Parse(txbStartuur.Text);
                training.EindUur = TimeSpan.Parse(txbEinduur.Text);

                if (training.BeschikbarePlaatsen <= 0)
                {
                    txtTrainingStatus.Text = "Het aantal beschikbare plaatsen moet groter zijn dan 0.";
                    return;
                }

                if (training.Diepte <= 0)
                {
                    txtTrainingStatus.Text = "De diepte moet groter zijn dan 0.";
                    return;
                }

                if (training.EindUur <= training.StartUur)
                {
                    txtTrainingStatus.Text = "Het einduur moet later zijn dan het startuur.";
                    return;
                }

                // Fictieve studenten
                training.IngeschrevenStudenten.Add("Jan Peeters");
                training.IngeschrevenStudenten.Add("Sara Claes");
                training.IngeschrevenStudenten.Add("Tom Vermeiren");

                trainingen.Add(training);

                RefreshAlles();
                MaakTrainingFormulierLeeg();

                MessageBox.Show("Training succesvol toegevoegd.");
            }
            catch
            {
                txtTrainingStatus.Text = "Controleer de invoer van de training. Gebruik getallen en uren zoals 18:00.";
            }
        }

        private void btnTrainingVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            Training training = dgTrainingen.SelectedItem as Training;

            if (training == null)
            {
                txtTrainingStatus.Text = "Selecteer eerst een training.";
                return;
            }

            trainingen.Remove(training);

            RefreshAlles();
            MaakTrainingFormulierLeeg();

            MessageBox.Show("Training verwijderd.");
        }

        private void btnTrainingLeegmaken_Click(object sender, RoutedEventArgs e)
        {
            MaakTrainingFormulierLeeg();
        }

        private void dgTrainingen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Training training = dgTrainingen.SelectedItem as Training;

            if (training != null)
            {
                geselecteerdeTraining = training;
                RefreshStudentenLijsten();
            }
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
                txtTrainingStatus.Text = "Selecteer eerst een student.";
                return;
            }

            if (geselecteerdeTraining.IsVol)
            {
                txtTrainingStatus.Text = "Deze training is vol.";
                return;
            }

            string student = lstIngeschrevenStudenten.SelectedItem.ToString();

            geselecteerdeTraining.IngeschrevenStudenten.Remove(student);
            geselecteerdeTraining.GeaccepteerdeStudenten.Add(student);

            RefreshAlles();

            if (geselecteerdeTraining.IsVol)
            {
                txtTrainingStatus.Text = "Deze training is nu vol.";
            }
            else
            {
                txtTrainingStatus.Text = "Student geaccepteerd.";
            }
        }

        // ---------------- FICTIEVE DATA ----------------

        private void VoegFictieveTrainingenToe()
        {
            Training training = new Training();
            training.Naam = "Zwembadtraining";
            training.Titel = "Basisvaardigheden";
            training.Tags = "Zwembad";
            training.Inhoud = "Basistraining voor nieuwe duikers.";
            training.Stad = "Antwerpen";
            training.Provincie = "Antwerpen";
            training.BeschikbarePlaatsen = 2;
            training.Niveau = 1;
            training.Diepte = 10;
            training.Datum = DateTime.Today.AddDays(5);
            training.StartUur = new TimeSpan(18, 0, 0);
            training.EindUur = new TimeSpan(20, 0, 0);

            training.IngeschrevenStudenten.Add("Lisa Peeters");
            training.IngeschrevenStudenten.Add("Arne Jansen");
            training.IngeschrevenStudenten.Add("Mila Verhoeven");

            trainingen.Add(training);
        }
    }
}