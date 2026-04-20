namespace WpfBubbelvrienden
{
    public class Lid
    {
        public string ID { get; set; } = "";
        public string Naam { get; set; } = "";
        public string Voornaam { get; set; } = "";
        public string Rijksregisternummer { get; set; } = "";
        public string Straat { get; set; } = "";
        public string Huisnummer { get; set; } = "";
        public string Postcode { get; set; } = "";
        public string Gemeente { get; set; } = "";
        public string Telefoonnummer { get; set; } = "";
        public string Email { get; set; } = "";
        public int Certificaat { get; set; }

        public override string ToString()
        {
            return
                "ID: " + ID + "\n" +
                "Naam: " + Naam + "\n" +
                "Voornaam: " + Voornaam + "\n" +
                "Rijksregisternummer: " + Rijksregisternummer + "\n" +
                "Straatnaam: " + Straat + "\n" +
                "Huisnummer: " + Huisnummer + "\n" +
                "Postcode: " + Postcode + "\n" +
                "Gemeente: " + Gemeente + "\n" +
                "Telefoonnummer: " + Telefoonnummer + "\n" +
                "Email-adres: " + Email + "\n" +
                "Certificaat: " + Certificaat;
        }
    }
}