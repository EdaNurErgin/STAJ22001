


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stajApi.Models;

namespace staj.Controllers
{
    [Authorize]
    public class ProductController : BaseApiController<Product>
    {
        public ProductController(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory, "api/productapi") { }

        //  Yeni ürün ekleme formu
        public IActionResult Create()
        {
            return View(new Product());
        }

        //  Yeni ürün ekleme (resim dahil)
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? Image)
        {
            if (!ModelState.IsValid)
                return View(product);

            if (Image != null && Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await Image.CopyToAsync(ms);
                product.ImageData = ms.ToArray();
            }

            var success = await CreateEntity(product);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "API'ye ürün eklenemedi.");
            return View(product);
        }

        // ✔️ Ürün düzenleme formu
        public async Task<IActionResult> Edit(int id)
        {
            var product = await GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // ✔️ Ürün düzenleme (resim dahil)
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? Image)
        {

            if (!ModelState.IsValid)
                return View(product);

            if (Image != null && Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await Image.CopyToAsync(ms);
                product.ImageData = ms.ToArray();
            }

            var success = await UpdateEntity(product.Id, product);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "API'de ürün güncellenemedi.");
            return View(product);
        }

        // ✔️ Ürün silme
        public async Task<IActionResult> Delete(int id)
        {
            var success = await DeleteEntity(id);
            if (!success)
                ModelState.AddModelError("", "API'de ürün silinemedi.");

            return RedirectToAction(nameof(Index));
        }
    }
}






