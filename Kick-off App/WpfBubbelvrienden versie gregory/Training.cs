using System;

namespace WpfBubbelvrienden
{
    public class Training
    {
        public string ID { get; set; } = "";
        public string Naam { get; set; } = "";
        public string Titel { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Inhoud { get; set; } = "";
        public string Plaats { get; set; } = "";
        public int BeschikbarePlaatsen { get; set; }
        public string Niveau { get; set; } = "";
        public int Diepte { get; set; }
        public DateTime Datum { get; set; }

        public override string ToString()
        {
            return
                "ID: " + ID + "\n" +
                "Naam: " + Naam + "\n" +
                "Titel: " + Titel + "\n" +
                "Tags: " + Tags + "\n" +
                "Inhoud: " + Inhoud + "\n" +
                "Plaats: " + Plaats + "\n" +
                "Beschikbare plaatsen: " + BeschikbarePlaatsen + "\n" +
                "Niveau: " + Niveau + "\n" +
                "Diepte: " + Diepte + "\n" +
                "Datum: " + Datum.ToString("dd/MM/yyyy");
        }
    }
}