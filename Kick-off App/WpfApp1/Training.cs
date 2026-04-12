using System;
using System.Collections.Generic;

namespace WpfBubbelvrienden
{
    public class Training
    {
        public string Naam { get; set; }
        public string Titel { get; set; }
        public string Tags { get; set; }
        public string Inhoud { get; set; }
        public string Stad { get; set; }
        public string Provincie { get; set; }
        public int BeschikbarePlaatsen { get; set; }
        public int Niveau { get; set; }
        public int Diepte { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan StartUur { get; set; }
        public TimeSpan EindUur { get; set; }

        public List<string> IngeschrevenStudenten { get; set; }
        public List<string> GeaccepteerdeStudenten { get; set; }

        public Training()
        {
            IngeschrevenStudenten = new List<string>();
            GeaccepteerdeStudenten = new List<string>();
        }

        public bool IsVol
        {
            get { return GeaccepteerdeStudenten.Count >= BeschikbarePlaatsen; }
        }

        public string VolledigePlaats
        {
            get { return Stad + ", " + Provincie; }
        }

        public string VolTekst
        {
            get
            {
                if (IsVol)
                {
                    return "Ja";
                }
                return "Nee";
            }
        }
    }
}