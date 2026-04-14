using System.Text.RegularExpressions;

namespace WpfBubbelvrienden
{
    public static class ValidatieHelper
    {
        public static string ValideerLid(
            string naam,
            string voornaam,
            string rijksregisternummer,
            string adres,
            string telefoonnummer,
            string emailadres)
        {
            if (string.IsNullOrWhiteSpace(naam) ||
                string.IsNullOrWhiteSpace(voornaam) ||
                string.IsNullOrWhiteSpace(rijksregisternummer) ||
                string.IsNullOrWhiteSpace(adres) ||
                string.IsNullOrWhiteSpace(telefoonnummer) ||
                string.IsNullOrWhiteSpace(emailadres))
            {
                return "Alle velden moeten verplicht ingevuld worden.";
            }

            if (!Regex.IsMatch(rijksregisternummer, @"^\d{11}$"))
            {
                return "Een rijksregisternummer moet exact 11 cijfers bevatten.";
            }

            if (!Regex.IsMatch(telefoonnummer, @"^[0-9+\s\/.-]{8,20}$"))
            {
                return "Het telefoonnummer is ongeldig.";
            }

            if (!Regex.IsMatch(emailadres, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "Het e-mailadres is ongeldig.";
            }

            return "";
        }
    }
}