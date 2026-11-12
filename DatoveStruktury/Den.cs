/// <summary>Jeden den deníku: datum + seznam záznamů.</summary>
public class Den
{
    public DateTime Datum { get; }
    public ISeznam<string> Zaznamy { get; } = new PoleSeznam<string>();

    public Den(DateTime datum) { Datum = datum; }
}