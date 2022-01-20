using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreNortwind.Data.Dtos
{
    public class MostOrderedFiveProducts
    {
        //en çok sipariş edilen5 adet ürünü raporlarız.
        //Views olarak kullanılan classlar herhangi bir method barındırmaz key alanları ID alanları olmaz. Sadece içerisinde property barındırır.
        //bu bir entity değil yani domain object değil DTO yani data transferr objecttir.
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        //ayynı alan olsun ki db den geleni buraya basalım
    }
}
