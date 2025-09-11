


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using staj.Models;
using stajApi.Models;


namespace staj.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController<stajApi.Models.Order>
    {
        public OrderController(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory, "api/orderapi") { }

        //  BaseApiController'dan Index kullanılacak → tüm siparişler listelenir
        // Ancak API'nin ilişkili User, Customer, OrderDetails, Product'ları döndürdüğünden emin ol!

        //  Sipariş detayları (özelleştirilmiş, BaseApiController'da yok)
        public async Task<IActionResult> Details(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Sipariş detayları getirilemedi.");
                return RedirectToAction(nameof(Index));
            }

            var json = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<stajApi.Models.Order>(json);

            if (order == null)
                return NotFound();

            return View(order);
        }

        //  Siparişi tamamlama (özelleştirilmiş, BaseApiController'da yok)
        public async Task<IActionResult> CompleteOrder(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsync($"{_apiUrl}/complete/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Sipariş tamamlanamadı.");
            }

            return RedirectToAction(nameof(Index));
        }

        // Silme işlemi BaseApiController'daki DeleteEntity ile yapılır (gerekirse override yazabilirsin)
        // Yeni sipariş oluşturma/güncelleme metotları yoksa ekleyebilirsin
         


        

    }
}



