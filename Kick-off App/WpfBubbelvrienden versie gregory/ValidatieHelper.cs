using System.Text.RegularExpressions;

namespace WpfBubbelvrienden
{
    public static class ValidatieHelper
    {
        public static string ValideerLid(
            string naam,
            string voornaam,
            string rijksregisternummer,
            string straat,
            string huisnummer,
            string postcode,
            string gemeente,
            string telefoonnummer,
            string email)
        {
            if (string.IsNullOrWhiteSpace(naam) ||
                string.IsNullOrWhiteSpace(voornaam) ||
                string.IsNullOrWhiteSpace(rijksregisternummer) ||
                string.IsNullOrWhiteSpace(straat) ||
                string.IsNullOrWhiteSpace(huisnummer) ||
                string.IsNullOrWhiteSpace(postcode) ||
                string.IsNullOrWhiteSpace(gemeente) ||
                string.IsNullOrWhiteSpace(telefoonnummer) ||
                string.IsNullOrWhiteSpace(email))
            {
                return "Alle velden moeten verplicht ingevuld worden.";
            }

            if (!Regex.IsMatch(postcode, @"^\d{4}$"))
            {
                return "De postcode moet uit exact 4 cijfers bestaan.";
            }

            if (!Regex.IsMatch(rijksregisternummer, @"^\d{11}$"))
            {
                return "Een rijksregisternummer moet uit exact 11 cijfers bestaan.";
            }

            if (!IsGeldigRijksregister(rijksregisternummer))
            {
                return "Het rijksregisternummer is ongeldig.";
            }

            if (!Regex.IsMatch(telefoonnummer, @"^(?:\+324\d{8}|04\d{8}|0\d{8,9})$"))
            {
                return "Het telefoonnummer is ongeldig.";
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "Het e-mailadres is ongeldig.";
            }

            return "";
        }

        private static bool IsGeldigRijksregister(string rrn)
        {
            if (!Regex.IsMatch(rrn, @"^\d{11}$"))
            {
                return false;
            }

            string baseNumber = rrn.Substring(0, 9);
            string controlDigits = rrn.Substring(9, 2);

            long baseNum;
            int control;

            if (!long.TryParse(baseNumber, out baseNum) ||
                !int.TryParse(controlDigits, out control))
            {
                return false;
            }

            int expectedControl = 97 - (int)(baseNum % 97);

            if (expectedControl == control)
            {
                return true;
            }

            expectedControl = 97 - (int)((2000000000L + baseNum) % 97);

            return expectedControl == control;
        }
    }
}