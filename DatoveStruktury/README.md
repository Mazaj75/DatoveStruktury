# Deník (spojový seznam dnů + seznam záznamů nad polem)

- **Jazyk:** C# 12, .NET 8 SDK, konzolová aplikace
- **Struktury:** `SpojovySeznam<T>` (dny, obousměrný), `PoleSeznam<T>` (záznamy, pole se při zaplnění zdvojnásobí)

## Spuštění
```
dotnet run
```

## Ovládání
Operace se píší do konzole: `predchozi`, `dalsi`, `zacatek`, `konec`, `novy`, `smaz`, `uloz`, `nacti`, `zavri`.
Text záznamu se ukončuje prázdným řádkem.