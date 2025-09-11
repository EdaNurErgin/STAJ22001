


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace staj.Controllers
{
    [Authorize]
    public class ChatController : BaseApiController<dynamic>
    {
        public ChatController(IHttpClientFactory factory)
            : base(factory, "api/UserApi")
        {
        }

        public override async Task<IActionResult> Index()
        {
            // Giriş yapan kullanıcının username bilgisi
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            AddAuthorizationHeader();

            // Kullanıcı bilgilerini API'den çek
            var response = await _httpClient.GetAsync($"{_apiUrl}/getbyusername?username={username}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kullanıcı bilgisi alınamadı.");
                return Unauthorized();
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic user = JsonConvert.DeserializeObject(json);

            // ViewBag’e bilgiler atılıyor
            ViewBag.UserId = (int)user.id;
            ViewBag.UserName = (string)user.username;

            // Kullanıcının rolünü belirle (Claim bazlı değilse burası manuel ayarlanabilir)
            ViewBag.UserRole = User.IsInRole("Customer") ? "Customer" : "User";

            // Eğer kullanıcı rolündeyse, sabit müşteri ID’si gir 
            ViewBag.ReceiverId = 5;

            return View();
        }
    }
}
