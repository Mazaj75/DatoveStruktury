using System.Globalization;
using System.Text;

public static class Program
{
    const string Format = "dd.MM.yyyy";
    static SpojovySeznam<Den> dny = new SpojovySeznam<Den>();

    static string Cti() => Console.ReadLine() ?? "";

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            Vypis();
            string prikaz = Cti().Trim().ToLowerInvariant();
            switch (prikaz)
            {
                case "predchozi": dny.PrejdiNaPredchozi(); break;
                case "dalsi": dny.PrejdiNaDalsi(); break;
                case "zacatek": dny.Zacatek(); break;
                case "konec": dny.Konec(); break;
                case "novy": Novy(); break;
                case "smaz": Smaz(); break;
                case "uloz": Uloz(); break;
                case "nacti": Nacti(); break;
                case "zavri": return;
                default: Console.WriteLine("Neznámá operace."); break;
            }
        }
    }

    static void Vypis()
    {
        Console.WriteLine("\n==============================");
        Den? d = dny.Aktualni;
        if (d == null)
        {
            Console.WriteLine("Deník je prázdný.");
        }
        else
        {
            Console.WriteLine("Den: " + d.Datum.ToString(Format, CultureInfo.InvariantCulture));
            for (int i = 0; i < d.Zaznamy.Velikost; i++)
                Console.WriteLine($"{i + 1}. " + d.Zaznamy.Get(i).Replace("\n", "\n   "));
        }
        Console.WriteLine("------------------------------");
        Console.WriteLine("Počet dnů: " + dny.Velikost
            + (d == null ? "" : " | Záznamů v dni: " + d.Zaznamy.Velikost));
        Console.WriteLine("Operace: predchozi, dalsi, zacatek, konec, novy, smaz, uloz, nacti, zavri");
        Console.Write("> ");
    }

    static DateTime NactiDatum()
    {
        while (true)
        {
            Console.Write("Datum (dd.MM.yyyy): ");
            if (DateTime.TryParseExact(Cti().Trim(), Format, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime datum))
                return datum;
            Console.WriteLine("Chybný formát data, zkuste to znovu.");
        }
    }

    static string NactiText()
    {
        Console.WriteLine("Text záznamu (konec = prázdný řádek):");
        var sb = new StringBuilder();
        string radek;
        while ((radek = Cti()) != "")
        {
            if (sb.Length > 0) sb.Append('\n');
            sb.Append(radek);
        }
        return sb.ToString();
    }

    static void Novy()
    {
        DateTime datum = NactiDatum();
        Den? aktualni = dny.Aktualni;
        bool novyDen;
        if (aktualni == null)
        {
            novyDen = true; // první záznam do prázdného deníku
        }
        else if (datum == aktualni.Datum)
        {
            novyDen = false;
        }
        else
        {
            Den? dalsi = dny.DalsiPrvek;
            bool zaAktualnim = datum > aktualni.Datum;
            bool predDalsim = dalsi == null || datum < dalsi.Datum;
            if (zaAktualnim && predDalsim) novyDen = true;
            else
            {
                Console.WriteLine("Zápis odmítnut: datum neodpovídá aktuálnímu dni ani místu hned za ním.");
                return;
            }
        }

        string text = NactiText();
        if (text.Length == 0) { Console.WriteLine("Prázdný záznam se neukládá."); return; }

        if (novyDen) dny.VlozZaAktualni(new Den(datum));
        dny.Aktualni!.Zaznamy.Pridej(text);
    }

    static void Smaz()
    {
        Den? d = dny.Aktualni;
        if (d == null) { Console.WriteLine("Není co mazat."); return; }

        Console.Write("Číslo záznamu ke smazání: ");
        if (!int.TryParse(Cti().Trim(), out int cislo)) { Console.WriteLine("Neplatné číslo."); return; }
        if (cislo < 1 || cislo > d.Zaznamy.Velikost) { Console.WriteLine("Záznam neexistuje."); return; }

        Console.WriteLine($"{cislo}. " + d.Zaznamy.Get(cislo - 1));
        Console.Write("Opravdu smazat? (a/n): ");
        if (!Cti().Trim().Equals("a", StringComparison.OrdinalIgnoreCase)) { Console.WriteLine("Zrušeno."); return; }

        d.Zaznamy.Odeber(cislo - 1);
        if (d.Zaznamy.Velikost == 0) dny.OdeberAktualni();
    }

    static string NactiNazevSouboru()
    {
        Console.Write("Název souboru [denik.txt]: ");
        string nazev = Cti().Trim();
        return nazev == "" ? "denik.txt" : nazev;
    }

    // Formát: "DEN dd.MM.yyyy <početZáznamů>", pak pro každý záznam "ZAZNAM <početŘádků>" + řádky
    static void Uloz()
    {
        string nazev = NactiNazevSouboru();
        try
        {
            Den? puvodni = dny.Aktualni;
            using (var sw = new StreamWriter(nazev, false, new UTF8Encoding(false)))
            {
                dny.Zacatek();
                for (int i = 0; i < dny.Velikost; i++)
                {
                    Den d = dny.Aktualni!;
                    sw.WriteLine($"DEN {d.Datum.ToString(Format, CultureInfo.InvariantCulture)} {d.Zaznamy.Velikost}");
                    for (int j = 0; j < d.Zaznamy.Velikost; j++)
                    {
                        string[] radky = d.Zaznamy.Get(j).Split('\n');
                        sw.WriteLine($"ZAZNAM {radky.Length}");
                        foreach (string r in radky) sw.WriteLine(r);
                    }
                    dny.PrejdiNaDalsi();
                }
            }
            // návrat na původní den
            dny.Zacatek();
            while (dny.Aktualni != puvodni && dny.DalsiPrvek != null) dny.PrejdiNaDalsi();
            Console.WriteLine("Uloženo do " + nazev);
        }
        catch (IOException e) { Console.WriteLine("Chyba při ukládání: " + e.Message); }
        catch (UnauthorizedAccessException e) { Console.WriteLine("Chyba při ukládání: " + e.Message); }
    }

    static void Nacti()
    {
        string nazev = NactiNazevSouboru();
        var novy = new SpojovySeznam<Den>();
        try
        {
            using var sr = new StreamReader(nazev, Encoding.UTF8);
            string? radek;
            DateTime? predchozi = null;
            while ((radek = sr.ReadLine()) != null)
            {
                if (radek == "") continue;
                string[] c = radek.Split(' ');
                if (c.Length != 3 || c[0] != "DEN") throw new FormatException("Očekáván řádek DEN: " + radek);
                if (!DateTime.TryParseExact(c[1], Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datum))
                    throw new FormatException("Chybné datum: " + c[1]);
                if (predchozi != null && datum <= predchozi) throw new FormatException("Dny nejsou vzestupně seřazeny.");
                predchozi = datum;

                int pocet = int.Parse(c[2]);
                if (pocet < 1) throw new FormatException("Den bez záznamů.");
                var den = new Den(datum);
                for (int i = 0; i < pocet; i++)
                {
                    string? hlavicka = sr.ReadLine();
                    if (hlavicka == null || !hlavicka.StartsWith("ZAZNAM ")) throw new FormatException("Očekáván řádek ZAZNAM.");
                    int radku = int.Parse(hlavicka.Substring(7).Trim());
                    var sb = new StringBuilder();
                    for (int r = 0; r < radku; r++)
                    {
                        string? t = sr.ReadLine() ?? throw new FormatException("Neočekávaný konec souboru.");
                        if (r > 0) sb.Append('\n');
                        sb.Append(t);
                    }
                    den.Zaznamy.Pridej(sb.ToString());
                }
                novy.VlozZaAktualni(den);
            }
            novy.Zacatek();
            dny = novy;
            Console.WriteLine("Načteno z " + nazev);
        }
        catch (Exception e) when (e is IOException || e is FormatException || e is UnauthorizedAccessException)
        {
            Console.WriteLine("Chyba při načítání (deník zůstává beze změny): " + e.Message);
        }
    }
}