using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using staj.Models;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;


//login/logout
//herkes tarafından erisilebilir
[AllowAnonymous] 
public class UserController : Controller
{
    private readonly HttpClient _httpClient;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5255/");
    }

    [HttpGet]
    public IActionResult Login() => View();

    //[HttpPost]
    //public async Task<IActionResult> Login(LoginViewModel model)
    //{
    //    //var response = await _httpClient.GetAsync("api/userapi");
    //    //var json = await response.Content.ReadAsStringAsync();

    //    //var users = JsonConvert.DeserializeObject<List<Customer>>(json);



    //    //var users = await _httpClient.GetFromJsonAsync<List<User>>("api/userapi");

    //    var response = await _httpClient.GetAsync("api/AuthApi");
    //    var json = await response.Content.ReadAsStringAsync();


    //    if (string.IsNullOrWhiteSpace(json))
    //    {
    //        // logla veya hata göster
    //        ModelState.AddModelError("", "API'den kullanıcı verisi alınamadı.");
    //        return View("Login");
    //    }


    //    var users = JsonConvert.DeserializeObject<List<User>>(json);



    //    var user = users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

    //    if (user != null)
    //    {
    //        var claims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.Name, user.Username)
    //        };

    //       // Cookie ile oturum başlatır
    //        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    //        var principal = new ClaimsPrincipal(identity);

    //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    //        return RedirectToAction("Index", "Home");
    //    }

    //    ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
    //    return View(model);
    //}



    //[HttpPost]
    //public async Task<IActionResult> Login(LoginViewModel model)
    //{
    //    var response = await _httpClient.PostAsJsonAsync("api/AuthApi/login", model);
    //    var json = await response.Content.ReadAsStringAsync();

    //    if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(json))
    //    {
    //        ModelState.AddModelError("", "API'den kullanıcı verisi alınamadı.");
    //        return View("Login");
    //    }

    //    var user = JsonConvert.DeserializeObject<User>(json);

    //    var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.Name, user.Username)
    //};

    //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    //    var principal = new ClaimsPrincipal(identity);

    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    //    return RedirectToAction("Index", "Home");
    //}


    //[HttpPost]
    //public async Task<IActionResult> Login(LoginViewModel model)
    //{
    //    var response = await _httpClient.PostAsJsonAsync("api/AuthApi/login", model);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
    //        return View(model);
    //    }

    //    var json = await response.Content.ReadAsStringAsync();
    //    var user = JsonConvert.DeserializeObject<User>(json);

    //    var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.Name, user.Username)
    //};

    //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    //    var principal = new ClaimsPrincipal(identity);

    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    //    TempData["Message"] = $"Hoş geldiniz, {user.Username}!";

    //    return RedirectToAction("Index", "Home");
    //}



    //[HttpPost]
    //public async Task<IActionResult> Login(LoginViewModel model)
    //{
    //    var response = await _httpClient.PostAsJsonAsync("api/AuthApi/login", model);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
    //        return View(model);
    //    }

    //    var json = await response.Content.ReadAsStringAsync();
    //    var result = JsonConvert.DeserializeObject<dynamic>(json);
    //    string token = result.token;

    //    var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.Name, model.Username ?? "UnknownUser"),
    //    new Claim("AccessToken", token)  // 🔑 TOKEN CLAIM OLARAK EKLENDİ
    //};

    //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    //    var principal = new ClaimsPrincipal(identity);

    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    //    TempData["Message"] = $"Hoş geldiniz, {model.Username}!";

    //    return RedirectToAction("Index", "Home");
    //}

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/AuthApi/login", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
            return View(model);
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(json);

        string token = result.token;
        int userId = result.id; // 🟡 SignalR için id çekiyoruz

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, model.Username ?? "UnknownUser"),
        new Claim("AccessToken", token),
        new Claim("Id", userId.ToString()) // ✅ SignalR bağlanınca burayı UserIdentifier olarak kullanacak
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        TempData["Message"] = $"Hoş geldiniz, {model.Username}!";

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        TempData["Message"] = "Başarıyla çıkış yaptınız.";
        return RedirectToAction("Login");
    }
}
