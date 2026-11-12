/// <summary>
/// Vlastní seznam nad polem pevné velikosti. Při zaplnění se vytvoří
/// větší pole (2x) a prvky se zkopírují.
/// </summary>
public class PoleSeznam<T> : ISeznam<T>
{
    private const int PocatecniKapacita = 4;
    private T[] pole = new T[PocatecniKapacita];
    private int pocet = 0;

    public int Velikost => pocet;

    public void Pridej(T prvek)
    {
        if (pocet == pole.Length)
        {
            T[] vetsi = new T[pole.Length * 2];
            for (int i = 0; i < pocet; i++) vetsi[i] = pole[i];
            pole = vetsi;
        }
        pole[pocet++] = prvek;
    }

    public T Get(int index)
    {
        Zkontroluj(index);
        return pole[index];
    }

    public T Odeber(int index)
    {
        Zkontroluj(index);
        T odebrany = pole[index];
        for (int i = index; i < pocet - 1; i++) pole[i] = pole[i + 1];
        pole[--pocet] = default!;
        return odebrany;
    }

    private void Zkontroluj(int index)
    {
        if (index < 0 || index >= pocet)
            throw new IndexOutOfRangeException($"Index {index}, velikost {pocet}");
    }
}