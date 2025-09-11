

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using stajApi.Data;

using stajApi.Models;

namespace stajApi.Controllers
{

    /// <summary>
    /// Siparişlerle ilgili işlemleri içerir.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderApiController : BaseApiController<Order>
    {
        public OrderApiController(ApplicationDbContext context) : base(context) { }


        /// <summary>
        /// Tüm siparişleri detaylı şekilde listeler.
        /// </summary>
        /// <returns>Sipariş listesi</returns>
        [HttpGet]
        public override IActionResult GetAll()
        {
            var orders = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.IsCompleted,
                    CustomerName = o.Customer.FullName,
                    UserName = o.User.Username,
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.ProductId,
                        Product = new
                        {
                            od.Product.Name,
                            od.Product.ImageData
                        },
                        od.Quantity
                    })
                })
                .ToList();

            return Ok(orders);
        }





        /// <summary>
        /// Belirli bir siparişin detaylarını getirir.
        /// </summary>
        /// <param name="id">Sipariş ID</param>
        /// <returns>Sipariş detayı</returns>

        [HttpGet("{id}")]
        public override IActionResult GetById(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(order, settings);
            return Content(json, "application/json");
        }


        /// <summary>
        /// Yeni sipariş oluşturur.
        /// </summary>
        /// <param name="order">Sipariş bilgisi</param>
        /// <returns>Oluşturulan sipariş</returns>
        [HttpPost]
        public override IActionResult Create([FromBody] Order order)
        {
            if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
                return BadRequest("Geçerli bir sipariş ve sipariş detayları gönderin.");

            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }



        /// <summary>
        /// Siparişi tamamlanmış olarak işaretler.
        /// </summary>
        /// <param name="id">Sipariş ID</param>
        /// <returns>Durum sonucu</returns>
        [HttpPut("complete/{id}")]
        public IActionResult CompleteOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();

            order.IsCompleted = true;
            _context.SaveChanges();
            return NoContent();
        }



        /// <summary>
        /// Belirli bir müşteriye ait siparişleri listeler.
        /// </summary>
        /// <param name="customerId">Müşteri ID</param>
        /// <returns>Sipariş listesi</returns>

        [HttpGet("customer/{customerId}")]
        public IActionResult GetOrdersByCustomer(int customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.IsCompleted,
                    CustomerName = o.Customer.FullName,
                    UserName = o.User.Username,
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.ProductId,
                        Product = new
                        {
                            od.Product.Name,
                            od.Product.ImageData
                        },
                        od.Quantity
                    })
                })
                .ToList();

            return Ok(orders);
        }


        ////Raporlama

        //[HttpGet("monthly-sales")]
        //public IActionResult GetMonthlySalesReport()
        //{
        //    var report = _context.Orders
        //        .Where(o => o.IsCompleted)
        //        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
        //        .Select(g => new {
        //            Year = g.Key.Year,
        //            Month = g.Key.Month,
        //            TotalSales = g.Count(),
        //            TotalRevenue = g.Sum(o => o.OrderDetails.Sum(od => od.Product.Price * od.Quantity))
        //        })
        //        .OrderBy(r => r.Year).ThenBy(r => r.Month)
        //        .ToList();

        //    return Ok(report);
        //}


    }
}
