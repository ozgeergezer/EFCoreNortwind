using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreNortwind.Data
{
    public partial class VwNancy
    {
        public string LastName { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
    }
}
