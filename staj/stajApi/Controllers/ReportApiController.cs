using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stajApi.Data; // DbContext için
using System.Linq;


namespace stajApi.Controllers
{

    /// <summary>
    /// Satış raporlarıyla ilgili işlemleri içerir.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportApiController(ApplicationDbContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Aylık satış raporunu getirir.
        /// </summary>
        /// <returns>Yıl ve aya göre toplam satış adedi ve toplam gelir</returns>

        // Aylık Satış Raporu
        [HttpGet("monthly-sales")]
        public IActionResult GetMonthlySalesReport()
        {
            var report = _context.Orders
                .Where(o => o.IsCompleted)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSales = g.Sum(o => o.OrderDetails.Sum(od => od.Quantity)),
                    TotalRevenue = g.Sum(o => o.OrderDetails.Sum(od => od.Product.Price * od.Quantity))

                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToList();

            return Ok(report);
        }


        /// <summary>
        /// Belirli bir yıl için aylık satış raporu verir.
        /// </summary>
        /// <param name="year">Yıl (örn: 2025)</param>
        /// <returns>12 ay boyunca toplam satış ve gelir bilgisi</returns>

        // Yıllık Satış Raporu - 12 ayın verisini verir
        [HttpGet("yearly-sales")]
        public IActionResult GetYearlySalesReport([FromQuery] int year)
        {
            var report = _context.Orders
                .Where(o => o.IsCompleted && o.OrderDate.Year == year)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalSales = g.Sum(o => o.OrderDetails.Sum(od => od.Quantity)), // 🔁 ürün adedi
                    TotalRevenue = g.Sum(o => o.OrderDetails.Sum(od => od.Product.Price * od.Quantity))
                })
                .OrderBy(r => r.Month)
                .ToList();

            return Ok(report);
        }



    }
}
