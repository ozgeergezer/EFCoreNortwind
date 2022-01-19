using EFCoreNortwind.Data;
using EFCoreNortwind.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            //sorgu1
            //en çok sipariş verilen 5 adet ürünü çek
            // var pro= _db.OrderDetails.OrderByDescending(x=>x.Quantity).Take(5).ToList();


            var pro1 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(p => p.Product).SelectMany(u => u.OrderDetails).GroupBy(c => c.Product.ProductName).Select(y => new
            {
                ProductName = y.Key,
                Quantity = y.Sum(z => z.Quantity)
            }).OrderByDescending(c => c.Quantity).Take(5).ToList();

            //sorgu2
            //hangi kategoride kaç adet ürün var
            var query2 = _db.Categories.Include(x => x.Products).Select(a => new
            {
                CategoryName = a.CategoryName,
                TotalProduct = a.Products.Count()
            }).ToList();

            //sorgu3
            //hangi ülkede kaç adet çalışanım var
            var pro3 = _db.Employees.GroupBy(x => x.Country).Select(a => new
            {
                Count = a.Count(),
                Name = a.Key
            }).ToList();

            //sorgu4
            //tüm ürünlerin maaliyeti ne kadar
            var pro4 = _db.Products.Sum(x => x.UnitPrice * x.UnitsInStock);

            //sorgu5
            //şimdiye kadar ne kadar ciro yaptık
            var pro5 = _db.OrderDetails.Sum(x => x.UnitPrice * x.Quantity - (x.UnitPrice * (decimal)x.Discount));

            //orta seviye

            //sorgu6
            //hangi müşteri hangi üründen kaç ader ürün sipariş etti
            var pro6 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).Include(x => x.Customer).SelectMany(y => y.OrderDetails).GroupBy(a => new { a.Order.CustomerId, a.Product.ProductName }).Select(a => new
            {
                Product = a.Key.ProductName,
                Customer = a.Key.CustomerId,
                total = a.Sum((g => g.Quantity)),
            }).ToList();

            // hangü müşteri kaç adet ürün sipariş etti
            var query61 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).Include(x => x.Customer).SelectMany(y => y.OrderDetails).GroupBy(y => y.Order.CustomerId).Select(a => new
            {
                Customer = a.Key,
                TotalProductQuantity = a.Sum(x => x.Quantity)
            }).OrderByDescending(x => x.TotalProductQuantity).ToList();

            //sorgu7
            //tost seven çalışanların sorgusu
            var pro7 = _db.Employees.Where(x => EF.Functions.Like(x.Notes, "%toast%")).ToList();

            //sorgu8
            //fiyatı >50 olan ürünlerin fiyata göre artandan azalana sıralayalım
            var pro8 = _db.Products.OrderByDescending(x => x.UnitPrice > 50).ToList();

            //sorgu9
            //rapor veren çalışanalrın listesi ( yani bir müdürü bulunan çalışanlar)
            var pro9 = _db.Employees.Select(x => new
            {
                emp = x.FirstName,
                rapor = x.ReportsTo
            }).ToList();

            //sorgu10
            //hangi tedarikçi kaç adet ürünü tedarik eder

            //iyi

            //sorgu11
            //yaşı>50 olanların listesi
            var pro11 = _db.Employees.Where(x => (DateTime.Now.Year - x.BirthDate.Value.Year) > 50).ToList();

            //sorgu12
            //hangi çalışan kaç adet sipariş almış
            var pro12 = _db.Orders
                .Include(x => x.Employee)
                .GroupBy(f => new { f.Employee.FirstName, f.Employee.LastName }).Select(k => new
                {
                    EmployeeName = $"{(k.Key.FirstName) } {k.Key.LastName }",
                    Count = k.Count()
                }).ToList();

            //sorgu13
            //hangi ürün hangi kategoride hangi tedarikçi tarafından getirilmiştir.
            //kategoriAdı, UrunAdı, Diyatı, Stogu, Tedarikçi, bilgilerini ekrana getirecek sorgu
            var pro13 = _db.Products.Include(x => x.Category).Include(x => x.Supplier).Select(a => new
            {
                ProductName = a.ProductName,
                CategoryName = a.Category.CategoryName,
                Supplier = a.Supplier.ContactName,
                stock = a.UnitsInStock
            }).AsNoTracking().ToList();
            //not: eğer sorgulama operasyonları yapıyorsak EF score bu sorgulanan nesnelerin eher birini takip alıyor. buda performans kaybına yol açıyor. select işlemlerinde çok gereksiz bir durum. sorhu performansını arttımak için asnoTracking olarak işaretleyelim.


            //var p = _db.Products.Find("1");
            ////changeTracker ile direlt proglamla tarafından attached oluyor. yani üzerinden değişiklik yapılınca direkt dvye gönderilebilir // attached
            //p.UnitPrice = 2131; //modified
            //_db.Products.Update(p);
            //_db.SaveChanges();

            var product = _db.Products.AsNoTracking().FirstOrDefault(x=>x.ProductId==1);
            var state1 = _db.Entry(product).State;
            product.UnitPrice = 24;
            var state2 = _db.Entry(product).State;
            _db.Products.Update(product);
            _db.SaveChanges();
            var state3 = _db.Entry(product).State;


            //sorgu14
            //en çok adette sipariş edilen sapirişin toplam tutarını bulalım
            var pro14 = _db.Orders.Include(x => x.OrderDetails).SelectMany(x => x.OrderDetails).GroupBy(x => x.OrderId).Select(a => new
            {
                OrderId = a.Key,
                orderQuantity = a.Sum(a => a.Quantity),
                orderTotalPrice = a.Sum(a => a.Quantity * a.UnitPrice * (decimal)a.Discount)
      
            }).OrderByDescending(x=>x.orderTotalPrice).Take(1).ToList();

            //sorgu15
            //en çok sipariş verilen ilk 5 ürünün toplam tutarını hesaplayalım
            //Ur unAdı, ToplamTutar şeklinde ekrana basılacak
            var pro15 = _db.Orders.Include(x => x.OrderDetails).ThenInclude(p => p.Product).SelectMany(u => u.OrderDetails).GroupBy(c => c.Product.ProductName).Select(y => new
            {
                ProductName = y.Key,
                fiyat = y.Sum(x => x.Quantity* x.UnitPrice* (decimal)(1-x.Discount)),
                total = y.Count()
            }).OrderByDescending(x => x.total).Take(5).Sum(x=>x.fiyat);


            //çok iyi

            return View();
        }

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
