using System;
using System.Collections.Generic;

namespace Library.Database.Models.Public;

public partial class country
{
    public int id { get; set; }

    public string country_name { get; set; } = null!;

    public string country_code2 { get; set; } = null!;

    public string country_code3 { get; set; } = null!;

    public string currency_code { get; set; } = null!;

    public string timezone { get; set; } = null!;
}
