using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreNortwind.Data
{
    public partial class Person
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
