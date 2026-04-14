namespace WpfBubbelvrienden
{
    public class MemberRecord
    {
        public int MemberId { get; set; }
        public string Naam { get; set; } = "";
        public string Voornaam { get; set; } = "";
        public string Rijksregisternummer { get; set; } = "";
        public string Adres { get; set; } = "";
        public string Telefoonnummer { get; set; } = "";
        public string Emailadres { get; set; } = "";
        public int Sterren { get; set; }
    }
}