using EFCoreNortwind.Data;
using EFCoreNortwind.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreNortwind.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NORTHWNDContext _db;

        public HomeController(ILogger<HomeController> logger, NORTHWNDContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult ViewController()
        {
            var model = _db.MostOrderedFiveProducts.ToList(); // add tarzı şeyler yapmamaız gerekiyor zaten ondan set kapattık.
            return View(model);
        }

        /*-- 
         * verinin bir bütün halinde tutulası , tek bir işlem olarak yapılabilmesini saplar  yani işlein atomik olması için kullanırız.

            -- ürün sipariş işleminde vt order derail ve order kaydı düşer aunı zamanda ürün stok güncvellenir.

              begin transaction

             transaction commit
            --tüm işlemleri başarılı ise transection commit edilir yani ilemler kapatılır. 
             transaction rollback 
            -- bir hata durumunda ies işlemler geri alınır
              end

            --genelde transaction yapılalrı store proceddure ile lullanılır.
            --store p kodları içerisinde eğer insert. update ve delete gibi işlemler varsa ve işlemler birbiri ile alakalı işlemler ise vt bütünlüğü bozmamak için transection kullanmalıyız.*/

        public IActionResult StoreProsedure() //sp için controller açtık
        {
            /*var model = _db.PagedProduct.FromSqlRaw($"'PagedProduct' {"'Liquids'"},{100},{0},{"'CategoryName'"},{"Asc"}").ToList();*/ //bir sql komut tetikleyebiliriz : FromSqlRaw

            var searchText = new SqlParameter("@searchText","Liquids");
            var limit= new SqlParameter("@limit", 100);
            var offset= new SqlParameter("@offset", 0);
            var sortColumn= new SqlParameter("@sortColumn", "CategoryName");
            var sortOrder= new SqlParameter("@sortOrder", "Asc");

            var model = _db.PagedProduct.FromSqlRaw($"PagedProduct @searchText, @limit,@offset,@sortColumn,@sortOrder", searchText, limit, offset, sortColumn, sortOrder).ToList();
            return View(model);
        }

        /*
         -- sql trigger

--bir tabloya yapılan insert, update,delete operasyonları sonrasında otomatik olarak tetiklenen database yaposodor. after update,insert,delete tiplerinde bir tabloya tanımlanabilir. bir tablo birden fazla trigger tanımlamak işlem takibi açısından zor pşdığı be bu triggerların birbirini kilitleme durumları oldığı için tercih edilmemektedir. genelde database taradınfa loglama amaçlı kullanılan sistemlerdir.

alter trigger CategoryLog on Categories
after insert
as
begin
select CategoryName,CategoryId from inserted
print 'Kategoriye kayıt girildi.'
end
-- kendi kendine tetiklenir.

insert into Categories(CategoryName) values ('KAtegori4')
         */


        public IActionResult ChangeTrackerView()
        {

            //var product = _db.Products.AsNoTracking().FirstOrDefault(x => x.ProductId == 1);

            /*_db.ChangeTracker.Entries();*/ // tüm entity ve state değişimlerini dizi olarak verir.

            //foreach (var item in _db.ChangeTracker.Entries())
            //{
            //    if(item.State == EntityState.Added)
            //    {
            //    }
            //}

            //var state1 = _db.Entry(product).State;

            //product.UnitPrice = 32;

            //var state2 = _db.Entry(product).State;

            //_db.Products.Update(product);
            //_db.SaveChanges();

            //var state3 = _db.Entry(product).State;

            // Disconnected Remove Operation

            // 1. yöntem (Attach ile silinecek olan nesneyi db Bağlar state değiştiririz)
            //Category cat3 = new Category();
            //cat3.CategoryId = 1009;

            //_db.Attach(cat3).State = EntityState.Deleted; // EF 6 da bu şekilde Attach etmek zorundaydık.
            //_db.SaveChanges();

            // 2. yöntem remove methodu ile silme
            //Category cat4 = new Category();
            //cat4.CategoryId = 1010;
            //_db.Remove(cat4);

            // ekleme için 1 yöntem
            //var category5 = new Category();
            //category5.CategoryName = "Yeni";

            //_db.Attach(category5).State = EntityState.Added;
            //_db.SaveChanges();

            // ekleme için 2 yöntem olan yöntem ile ekleme Add method connected gibi ekleyebiliriz.
            //var category6 = new Category();
            //category6.CategoryName = "Yeni kategori 6";

            //_db.Add(category6);
            //_db.SaveChanges();

            // Günceleme işleminde ise 1. yöntem

            //var category7 = new Category();
            //category7.CategoryId = 1011;
            //category7.CategoryName = "Yeni Güncel q4";
            //_db.Attach(category7).State = EntityState.Modified;
            //_db.SaveChanges();

            // 2. yöntem ise

            var category8 = new Category();
            category8.CategoryId = 1013;
            category8.CategoryName = "dsadsad34";

            _db.Update<Category>(category8);
            _db.SaveChanges();

            // EF Core da Update methodu disconnected state çalışır bu sebeple AsNoTracking işaretlenen bir nesnenin değerini değiştirebiliriz.

            return View();

        }
        public IActionResult Index()
        {
            // sorgu1
            // en çok sipariş verilen 5 adet ürünü çekelim ve kaç adet şipariş veridiğinide gösterelim
            // iki tablo birbiri ile çoka çok ilişkili ise ara tablo üzerinden diğer tabloya ulşamak için thenInclude yaparız.
            // 1'e  çok ilişki varsa yada 1'1 ilişki varsa Include yeterlidir.

            // sql tarafından view oluşturuyoruz.

            //            use NORTHWND

            //  create view MostOrderedFiveProducts
            //  as
            //  select top(5) p.ProductName as 'ProductName',  SUM(od.Quantity) as 'Quantity'
            //  from[Order Details] od
            //  inner join Orders o
            //  on od.OrderID = o.OrderID
            //  inner join Products p
            //  on p.ProductID = od.ProductID
            //  group by od.ProductID, p.ProductName
            //  order by Quantity desc

            //select* from MostOrderedFiveProducts

            //dinamik bir tablolama işlemi varsa SP kullanıyoruz. ilk çeyreği hesapla gibi.
            //data klasörü içinde dtos açıp içine class sql ile aynı ismi verdik. propla aldık içine. Views olarak kullanılan classlaeda method key ıd olmaz. çünkü entity değildir.
            //View ler hızlı rapor çekmek için kullanılır.Sabit rapor ise kullanılır.Id si yoktur.Tabloların içindeki kayıtların yansıması yoktur kendine ait bir kaydı yoktur.Tablodaki değişiklikler view tarafından yakalanır.Select dediğimiz kısım program tarafındaki view kısmıdır.
            //Dinamik bir raporlama işlemi varsa 2011 yılının ilk çeyreğinin raporu gibi paramatre vereceksek storedProcedures kullanıyoruz onun dışında view kullanırız..
            //nortwindcontext içerisinde dbset olarak tablo gibi view tanımlıyoruz.1.
            //nortwindcontext içerisinde OnModelCreating kısmında modelBuilder ile yazdık.
            //

            //sabit raporsa view kullanılır 
            //sorgu fiziksel olarak kaydedildiğinden hızlı çalışır tablodaki dğişiklik otomatilk yakalanır
            //ıd barındırmaz
            //tabloların yansımasıdır üzerinde bir kayıt operasyonu yapılmaz
            //select ile bir tablo gibi çalışır
            //eğer dinamik bir raporlama varsa(şu yılın ilk ) storeprosedure onun dışında view
            //böyle bir view varsa arka tarafa contexte bir class yazıyoruz

            //ht
            //var pro1 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(p => p.Product).SelectMany(u => u.OrderDetails).GroupBy(c => c.ProductId).Select(y => new
            //{
            //    ProductName = y.Key, // A
            //    Quantity = y.Sum(z => z.Quantity) // 1200
            //}).OrderByDescending(c => c.Quantity).Take(5).ToList();

            /*    SQL QUERY
             * select top 5 SUM(od.Quantity) as 'adet', p.ProductName from [Order Details] od 
                    inner join Orders o 
                    on od.OrderID = o.OrderID inner join Products p on
                    p.ProductID = od.ProductID
                    group by od.ProductID, p.ProductName 
                    order by adet desc
             */

            // sorgu2
            // hangi kategoride kaç adet ürün var

            var q2 = _db.Categories.Include(x => x.Products).Select(a => new
            {
                CategoryName = a.CategoryName,
                ProductCount = a.Products.Count()
            }).ToList();

            // sorgu3
            // hangi ülkede kaç adet çalışanım var

            var q3 = _db.Employees.GroupBy(x => x.Country).Select(a => new
            {
                Country = a.Key,
                Count = a.Count()
            }).ToList();

            // sorgu4
            // tüm ürünlerin maliyeti ne kadar

            var q4 = _db.Products.Sum(x => x.UnitPrice * x.UnitsInStock);

            // sorgu5
            // şimdiye kadar ne kadar ciro yaptık

            var q5 = _db.Orders.Include(x => x.OrderDetails).SelectMany(a => a.OrderDetails).Sum(x => x.Quantity * x.UnitPrice * (decimal)(1 - x.Discount));

            // sorgu6
            // hangi müşteri hangi üründen kaç adet sipariş etti
            // çift alana göre group by işlemi

            var query6 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).Include(x => x.Customer).SelectMany(y => y.OrderDetails).GroupBy(y => new { y.Order.CustomerId, y.Product.ProductName }).Select(a => new
            {
                Product = a.Key.ProductName,
                Customer = a.Key.CustomerId,
                TotalProductQuantity = a.Sum(x => x.Quantity)
            }).OrderByDescending(x => x.TotalProductQuantity).ToList();

            // hangi müşteri hangi kaç adet ürün sipariş etti
            var query61 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).Include(x => x.Customer).SelectMany(y => y.OrderDetails).GroupBy(y => y.Order.CustomerId).Select(a => new
            {
                Customer = a.Key,
                TotalProductQuantity = a.Sum(x => x.Quantity)

            }).OrderByDescending(x => x.TotalProductQuantity).ToList();

            /*
             * select SUM(od.Quantity) 'adet',od.ProductID , c.CustomerID
                    from Orders o inner join 
                    [Order Details] od on o.OrderID = od.OrderID 
                    inner join Products p on p.ProductID = od.ProductID 
                    inner  join Customers c on c.CustomerID = o.CustomerID
                    group by  od.ProductID, c.CustomerID
             */

            // sorgu7
            // tost seven çalışanların sorgusu

            var q7 = _db.Employees.Where(x => EF.Functions.Like(x.Notes, "%toast%")).ToList();

            // sorgu 8
            // fiyatı 50 liranın üstünde olan ürünleri fiyata göre artandan azalana sıralayalım

            var q8 = _db.Products.OrderByDescending(x => x.UnitPrice >= 50).ToList();

            // sorgu9 
            // rapor veren çalışanların listesi (yani bir müdürü bulunan çalışanlar)

            var q9 = _db.Employees.Where(x => x.ReportsTo != null).ToList();

            // bütün müdürleri bul

            // select * from Employees e3 where e3.EmployeeID in (select distinct emp2.ReportsTo from Employees emp2)
            // Any ile çözüm bulamadık. 
            // Contains veya Any ile reporstto emplyeeId eşit olanlar sorgulandı
            var q10 = _db.Employees.Where(x =>
            _db.Employees.Select(y => y.ReportsTo).Distinct().Any(id => id == x.EmployeeId)).ToList();

            var q101 = _db.Employees.Where(x =>
           _db.Employees.Select(y => y.ReportsTo).Distinct().Contains(x.EmployeeId)).ToList();

            // 1 can null 
            // 2 ali 1
            // mehmet 2

            // sorgu10
            // hangi tedarikçi kaç adet ürün tedarik ediyor ?

            // sorgu 11
            // 50 yaş üzerindeki çalışanların listesi

            // sorgu12
            // hangi çalışan kaç adet sipariş almış


            //var query12 = _db.Orders
            //    .Include(x => x.Employee)
            //    .GroupBy(x => new { x.Employee.FirstName, x.Employee.LastName }).Select(a => new
            //    {
            //        EmployeeName = $"{a.Key.FirstName} {a.Key.LastName}",
            //        Count = a.Count()

            //    }).ToList();

            //var qauery14 = _db.Orders.Include(x => x.Customer).Select(a => a.Customer.ContactName).ToList();
            //var qauery13 = _db.Orders.Select(a=> a.Customer.ContactName).ToList();

            // sorgu13
            // Hangi ürün hangi kategoride hangi tedarikçi tarafından getirilmiştir. KategoryAdı,UrunAdı,Fiyatı,Stoğu,Tedarikçi bilgileri ekrana getirilecek sorgu

            var query13 = _db.Products.Include(x => x.Category).Include(x => x.Supplier).Select(a => new
            {
                ProductName = a.ProductName,
                CategoryName = a.Category.CategoryName,
                SupplierName = a.Supplier.ContactName,
                UnitPrice = a.UnitPrice,
                Stock = a.UnitsInStock

            }).AsNoTracking();

            // AsNoTracking toList veya firstOrDefault öncesinde kullanalım.
            // ToList ve firstOrDefault IQuerable olarak işaretlenmiş sorgunun Sql düşmesine yani execute edilmesine neden olur.

            // Not eğer sorgulama operasyonları yapıyorsak EF core bu sorgulananan nesnelerin her birini takip alıyor. buda performans kaybına yol açıyor. select işlemlerinde çok gereksiz bir durum. Sorgu performansını artırmak asNoTracking olarak işaretleyelim

            //var product = _db.Products.Find("1"); // ChangeTracker ile direk program tarafında attached oluyor. yani üzerinden değişiklik yapılınca direk dbye gönderilebilir. // Attached
            //product.UnitPrice = 2131; // Modified

            //_db.Products.Update(product);
            //_db.SaveChanges(); // detached

            query13.ToList();

            //sorgu14
            // en çok adet ürün sipariş edilen sipariş'in toplam tutarını bulalım

            // Sql 
            /*
             * select MaxPriceOrderTableRecord.OrderTotalPrice from (select 
                top 1
                od.OrderID as 'OrderNumber',
                SUM(od.Quantity) as 'OrderQuantity', 
                SUM(od.Quantity * od.UnitPrice * (1-od.Discount))  as 'OrderTotalPrice'
                from Orders o inner join [Order Details] od
                on o.OrderID = od.OrderID 
                group by od.OrderID
                order by OrderQuantity desc) as MaxPriceOrderTableRecord
             */

            var query14 = _db.Orders.Include(x => x.OrderDetails).SelectMany(x => x.OrderDetails).GroupBy(x => x.OrderId).Select(a => new
            {
                OrderId = a.Key,
                OrderQuantity = a.Sum(x => x.Quantity),
                OrderTotalPrice = a.Sum(x => x.Quantity * x.UnitPrice * (decimal)(1 - x.Discount))

            }).OrderByDescending(x => x.OrderQuantity).Take(1).FirstOrDefault().OrderTotalPrice;

            //sorgu15
            // en çok sipariş edilen ilk 5 ürünün toplam tutarını hesaplayalım
            // Urun Adı, Toplam Tutar şeklinde ekranda listelenecek.

            var query15 = _db.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .SelectMany(x => x.OrderDetails)
                .GroupBy(x => x.Product.ProductName)
                .Select(a => new
                {
                    ProductName = a.Key,
                    ProductCount = a.Count(),
                    ProductTotalPrice =
                a.Sum(x => x.Quantity * x.UnitPrice * (decimal)(1 - x.Discount))

                })
                .OrderByDescending(x => x.ProductCount)
                .Take(5)
                .Sum(x => x.ProductTotalPrice);

            return View();
        }

        //changeTracker ile direlt proglamla tarafından attached oluyor. yani üzerinden değişiklik yapılınca direkt dbye gönderilebilir // attached

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
