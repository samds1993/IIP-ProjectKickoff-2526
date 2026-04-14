using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfBubbelvrienden
{
    public static class CsvHelper
    {
        public static void ExporteerLeden(string pad, List<MemberRecord> leden)
        {
            using (StreamWriter writer = new StreamWriter(pad))
            {
                writer.WriteLine("MemberId,Naam,Voornaam,Rijksregisternummer,Adres,Telefoonnummer,Emailadres,Sterren");

                foreach (MemberRecord lid in leden)
                {
                    writer.WriteLine(
                        lid.MemberId + "," +
                        Escape(lid.Naam) + "," +
                        Escape(lid.Voornaam) + "," +
                        Escape(lid.Rijksregisternummer) + "," +
                        Escape(lid.Adres) + "," +
                        Escape(lid.Telefoonnummer) + "," +
                        Escape(lid.Emailadres) + "," +
                        lid.Sterren);
                }
            }
        }

        public static List<MemberRecord> ImporteerLeden(string pad)
        {
            List<MemberRecord> resultaat = new List<MemberRecord>();
            string[] lijnen = File.ReadAllLines(pad);

            if (lijnen.Length <= 1)
            {
                return resultaat;
            }

            for (int i = 1; i < lijnen.Length; i++)
            {
                string[] delen = SplitCsv(lijnen[i]);

                if (delen.Length >= 8)
                {
                    MemberRecord lid = new MemberRecord();
                    lid.MemberId = int.Parse(delen[0]);
                    lid.Naam = delen[1];
                    lid.Voornaam = delen[2];
                    lid.Rijksregisternummer = delen[3];
                    lid.Adres = delen[4];
                    lid.Telefoonnummer = delen[5];
                    lid.Emailadres = delen[6];
                    lid.Sterren = int.Parse(delen[7]);

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