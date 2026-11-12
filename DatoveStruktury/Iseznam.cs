/// <summary>Rozhraní vlastního seznamu.</summary>
public interface ISeznam<T>
{
    void Pridej(T prvek);
    T Get(int index);
    T Odeber(int index);
    int Velikost { get; }
}