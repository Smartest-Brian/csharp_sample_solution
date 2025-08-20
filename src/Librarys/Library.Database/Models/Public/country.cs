using System;
using System.Collections.Generic;

namespace Library.Database.Models.Public;

public partial class Country
{
    public int Id { get; set; }

    public string CountryName { get; set; } = null!;

    public string CountryCode2 { get; set; } = null!;

    public string CountryCode3 { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public string Timezone { get; set; } = null!;
}
