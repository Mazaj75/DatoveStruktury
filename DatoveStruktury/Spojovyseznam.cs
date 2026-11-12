/// <summary>Vlastní obousměrně spojový seznam s ukazatelem na aktuální prvek.</summary>
public class SpojovySeznam<T> where T : class
{
    private class Uzel
    {
        public T Data;
        public Uzel? Predchozi, Dalsi;
        public Uzel(T data) { Data = data; }
    }

    private Uzel? prvni, posledni, aktualni;
    private int pocet = 0;

    public int Velikost => pocet;

    public T? Aktualni => aktualni?.Data;
    public T? DalsiPrvek => aktualni?.Dalsi?.Data;

    public void PrejdiNaPredchozi() { if (aktualni?.Predchozi != null) aktualni = aktualni.Predchozi; }
    public void PrejdiNaDalsi()     { if (aktualni?.Dalsi != null) aktualni = aktualni.Dalsi; }
    public void Zacatek()           { aktualni = prvni; }
    public void Konec()             { aktualni = posledni; }

    /// <summary>Vloží prvek za aktuální (do prázdného seznamu jako jediný) a nastaví ho jako aktuální.</summary>
    public void VlozZaAktualni(T data)
    {
        Uzel novy = new Uzel(data);
        if (aktualni == null)
        {
            prvni = posledni = novy;
        }
        else
        {
            novy.Predchozi = aktualni;
            novy.Dalsi = aktualni.Dalsi;
            if (aktualni.Dalsi != null) aktualni.Dalsi.Predchozi = novy;
            else posledni = novy;
            aktualni.Dalsi = novy;
        }
        aktualni = novy;
        pocet++;
    }

    /// <summary>Odebere aktuální prvek; aktuálním se stane následující, jinak předchozí.</summary>
    public void OdeberAktualni()
    {
        if (aktualni == null) return;
        Uzel? p = aktualni.Predchozi, n = aktualni.Dalsi;
        if (p != null) p.Dalsi = n; else prvni = n;
        if (n != null) n.Predchozi = p; else posledni = p;
        aktualni = n ?? p;
        pocet--;
    }
}