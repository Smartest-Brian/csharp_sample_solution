using Library.Database.Models.Public;

namespace Library.InfoHelper;

public static class CountryCache
{
    private static List<Country> _countries = new();

    public static IReadOnlyList<Country> Countries => _countries;

    internal static void SetCountries(List<Country> countries)
    {
        _countries = countries;
    }
}
