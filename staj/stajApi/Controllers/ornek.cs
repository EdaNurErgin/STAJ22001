using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace StajApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Örnek veri listesi 
        private static List<string> products = new List<string>
        {
            "Laptop", "Telefon", "Tablet"
        };

        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(products);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id < 0 || id >= products.Count)
                return NotFound("Ürün bulunamadı!");

            return Ok(products[id]);
        }

        // POST: api/products
        [HttpPost]
        public IActionResult Create([FromBody] string productName)
        {
            products.Add(productName);
            return Ok($"'{productName}' ürünü eklendi.");
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] string newName)
        {
            if (id < 0 || id >= products.Count)
                return NotFound("Ürün bulunamadı!");

            products[id] = newName;
            return Ok($"Ürün güncellendi: {newName}");
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0 || id >= products.Count)
                return NotFound("Ürün bulunamadı!");

            var deleted = products[id];
            products.RemoveAt(id);
            return Ok($"'{deleted}' ürünü silindi.");
        }
    }
}
