


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using stajApi.Data;
using stajApi.Models;

namespace stajApi.Controllers
{

    /// <summary>
    /// Müşteriyle alakalı işlemleri içerir
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerApiController(ApplicationDbContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Yeni müşteri ekler
        /// </summary>
        /// <param name="customer">Müşteri bilgileri</param>
        /// <returns>Oluşturulan müşteri</returns>

        [HttpPost]
        public IActionResult AddCustomer([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Geçersiz veri");

            // 🔐 Şifreyi hashle
            var hasher = new PasswordHasher<Customer>();
            customer.Password = hasher.HashPassword(customer, customer.Password);

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }



        /// <summary>
        /// Belirli bir müşteriyi getirir
        /// </summary>
        /// <param name="id">Müşteri ID</param>
        /// <returns>Müşteri detayları</returns>
        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            return customer == null ? NotFound() : Ok(customer);
        }




        /// <summary>
        /// Müşteri bilgilerini günceller
        /// </summary>
        /// <param name="id">Müşteri ID</param>
        /// <param name="customer">Yeni müşteri bilgileri</param>
        /// <returns>Durum sonucu</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            var existingCustomer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (existingCustomer == null)
                return NotFound();

            existingCustomer.FullName = customer.FullName;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.ShippingAddress = customer.ShippingAddress;
            existingCustomer.BillingAddress = customer.BillingAddress;

            // 🔁 Şifre değişmişse hashle
            if (!string.IsNullOrWhiteSpace(customer.Password))
            {
                var hasher = new PasswordHasher<Customer>();
                existingCustomer.Password = hasher.HashPassword(customer, customer.Password);
            }

            _context.SaveChanges();

            return NoContent();
        }




        /// <summary>
        /// Tüm müşterileri listeler
        /// </summary>
        /// <returns>Müşteri listesi</returns>
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            return Ok(_context.Customers.ToList());
        }




        /// <summary>
        /// Müşteri siler
        /// </summary>
        /// <param name="id">Silinecek müşteri ID</param>
        /// <returns>Durum sonucu</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
