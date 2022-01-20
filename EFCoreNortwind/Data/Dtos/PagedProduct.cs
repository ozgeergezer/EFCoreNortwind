using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreNortwind.Data.Dtos
{
    public class PagedProduct
    {
        public int ProductId { get; set; } 
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string SupplierCompany { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
        //contexte db set olarak tanımla
    }
}
