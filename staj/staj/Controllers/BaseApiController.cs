//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using System.Net.Http;
//using System.Net.Http.Json;

//public class BaseApiController<T> : Controller where T : class
//{
//    protected readonly HttpClient _httpClient;
//    protected readonly string _apiUrl;  // private değil, protected

//    public BaseApiController(IHttpClientFactory httpClientFactory, string apiUrl)
//    {
//        _httpClient = httpClientFactory.CreateClient();
//        _httpClient.BaseAddress = new Uri("http://localhost:5255/");
//        _apiUrl = apiUrl;
//    }

//    // Listeleme
//    public async Task<IActionResult> Index()
//    {
//        try
//        {
//            var response = await _httpClient.GetAsync(_apiUrl);
//            if (!response.IsSuccessStatusCode)
//                throw new Exception("API'den geçerli yanıt alınamadı.");

//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<T>>(json);

//            return View(list);
//        }
//        catch (Exception ex)
//        {
//            ModelState.AddModelError("", ex.Message);
//            return View(new List<T>());
//        }
//    }

//    // Detay
//    public async Task<T> GetById(int id)
//    {
//        var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
//        var json = await response.Content.ReadAsStringAsync();
//        return JsonConvert.DeserializeObject<T>(json);
//    }

//    // Ekle
//    public async Task<bool> CreateEntity(T entity)
//    {
//        var response = await _httpClient.PostAsJsonAsync(_apiUrl, entity);
//        return response.IsSuccessStatusCode;
//    }

//    // Güncelle
//    public async Task<bool> UpdateEntity(int id, T entity)
//    {
//        var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{id}", entity);
//        return response.IsSuccessStatusCode;
//    }

//    // Sil
//    public async Task<bool> DeleteEntity(int id)
//    {
//        var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
//        return response.IsSuccessStatusCode;
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

public class BaseApiController<T> : Controller where T : class
{
    protected readonly HttpClient _httpClient;
    protected readonly string _apiUrl;

    public BaseApiController(IHttpClientFactory httpClientFactory, string apiUrl)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5255/");
        _apiUrl = apiUrl;
    }

    // Token'ı header'a ekleyen yardımcı
    public void AddAuthorizationHeader()
    {
        var token = User.FindFirst("AccessToken")?.Value;
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public virtual async Task<IActionResult> Index()
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(_apiUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception("API'den geçerli yanıt alınamadı.");

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<T>>(json);
            return View(list);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(new List<T>());
        }
    }

    public async Task<T> GetById(int id)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task<bool> CreateEntity(T entity)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync(_apiUrl, entity);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEntity(int id, T entity)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{id}", entity);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEntity(int id)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
        return response.IsSuccessStatusCode;
    }
}
