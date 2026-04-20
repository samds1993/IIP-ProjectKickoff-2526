using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfBubbelvrienden
{
    public static class CsvHelper
    {
        public static void ExporteerLeden(string pad, List<Lid> leden)
        {
            using (StreamWriter writer = new StreamWriter(pad))
            {
                writer.WriteLine("ID,Naam,Voornaam,Rijksregisternummer,Straat,Huisnummer,Postcode,Gemeente,Telefoonnummer,Email,Certificaat");

                foreach (Lid lid in leden)
                {
                    writer.WriteLine(
                        Escape(lid.ID) + "," +
                        Escape(lid.Naam) + "," +
                        Escape(lid.Voornaam) + "," +
                        Escape(lid.Rijksregisternummer) + "," +
                        Escape(lid.Straat) + "," +
                        Escape(lid.Huisnummer) + "," +
                        Escape(lid.Postcode) + "," +
                        Escape(lid.Gemeente) + "," +
                        Escape(lid.Telefoonnummer) + "," +
                        Escape(lid.Email) + "," +
                        lid.Certificaat);
                }
            }
        }

        public static List<Lid> ImporteerLeden(string pad)
        {
            List<Lid> resultaat = new List<Lid>();

            string[] lijnen = File.ReadAllLines(pad);

            if (lijnen.Length <= 1)
            {
                return resultaat;
            }

            for (int i = 1; i < lijnen.Length; i++)
            {
                string[] delen = SplitCsv(lijnen[i]);

                if (delen.Length >= 11)
                {
                    Lid lid = new Lid();
                    lid.ID = delen[0];
                    lid.Naam = delen[1];
                    lid.Voornaam = delen[2];
                    lid.Rijksregisternummer = delen[3];
                    lid.Straat = delen[4];
                    lid.Huisnummer = delen[5];
                    lid.Postcode = delen[6];
                    lid.Gemeente = delen[7];
                    lid.Telefoonnummer = delen[8];
                    lid.Email = delen[9];
                    lid.Certificaat = int.Parse(delen[10]);

                    resultaat.Add(lid);
                }
            }

            return resultaat;
        }

        private static string Escape(string tekst)
        {
            if (tekst.Contains(",") || tekst.Contains("\""))
            {
                tekst = tekst.Replace("\"", "\"\"");
                return "\"" + tekst + "\"";
            }

            return tekst;
        }

        private static string[] SplitCsv(string lijn)
        {
            List<string> velden = new List<string>();
            bool inQuotes = false;
            string huidig = "";

            foreach (char c in lijn)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    velden.Add(huidig);
                    huidig = "";
                }
                else
                {
                    huidig += c;
                }
            }

            velden.Add(huidig);

            return velden.Select(v => v.Replace("\"\"", "\"").Trim('"')).ToArray();
        }
    }
}