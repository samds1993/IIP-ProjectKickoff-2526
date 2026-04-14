using System;

namespace WpfBubbelvrienden
{
    public class Training
    {
        public string Naam { get; set; } = "";
        public string Titel { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Inhoud { get; set; } = "";
        public string Plaats { get; set; } = "";
        public int BeschikbarePlaatsen { get; set; }
        public int Niveau { get; set; }
        public int Diepte { get; set; }
        public DateTime Datum { get; set; }
    }
}