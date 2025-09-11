

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stajApi.Data;
using stajApi.Models;
using System;
using System.Linq;

namespace stajApi.Controllers
{

    /// <summary>
    /// Ürünlerle ilgili işlemleri içerir.
    /// </summary>
    [Authorize]
    public class ProductApiController : BaseApiController<Product>
    {
        public ProductApiController(ApplicationDbContext context) : base(context) { }


        /// <summary>
        /// Tüm ürünleri listeler.
        /// </summary>
        /// <returns>Ürün listesi</returns>
        [HttpGet]
        public override IActionResult GetAll()
        {
            var products = _context.Products
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    ImageData = p.ImageData != null ? Convert.ToBase64String(p.ImageData) : null
                })
                .ToList();

            return Ok(products);
        }



        /// <summary>
        /// Belirli bir ürünü getirir.
        /// </summary>
        /// <param name="id">Ürün ID</param>
        /// <returns>Ürün bilgisi</returns>

        [HttpGet("{id}")]
        public override IActionResult GetById(int id)
        {
            var product = _context.Products
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Stock,
                    ImageData = p.ImageData != null ? Convert.ToBase64String(p.ImageData) : null
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return Ok(product);
        }



        /// <summary>
        /// Ürünü günceller.
        /// </summary>
        /// <param name="id">Ürün ID</param>
        /// <param name="product">Güncellenmiş ürün bilgisi</param>
        /// <returns>Durum sonucu</returns>
        [HttpPut("{id}")]
        public override IActionResult Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest("ID uyumsuzluğu");

            var existing = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existing == null)
                return NotFound();

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Stock = product.Stock;

            // Eğer gelen Product içinde ImageData null değilse, yeni resmi kaydet
            if (product.ImageData != null && product.ImageData.Length > 0)
            {
                existing.ImageData = product.ImageData;
            }
            // Eğer ImageData null geliyorsa, eski resim aynen kalacak

            _context.SaveChanges();

            return NoContent();
        }

    }
}
