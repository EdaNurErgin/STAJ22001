using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using stajApi.Models.Dtos;
public class ReportController : Controller
{
    private readonly HttpClient _httpClient;

    public ReportController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5255"); // API projenin adresi
    }



    public async Task<IActionResult> MonthlySales(int? month, int? year)
    {
        // DTO tipinde liste
        var monthlyData = new List<MonthlyReportDto>();

        var monthlyResponse = await _httpClient.GetAsync("/api/ReportApi/monthly-sales");
        if (monthlyResponse.IsSuccessStatusCode)
        {
            var json = await monthlyResponse.Content.ReadAsStringAsync();
            monthlyData = JsonConvert.DeserializeObject<List<MonthlyReportDto>>(json);

            if (month.HasValue && year.HasValue)
            {
                monthlyData = monthlyData
                    .Where(d => d.Month == month && d.Year == year)
                    .ToList();
            }
        }

        // DTO tipinde yıllık liste
        var yearlyData = new List<YearlyReportDto>();
        if (year.HasValue)
        {
            var yearlyResponse = await _httpClient.GetAsync($"/api/ReportApi/yearly-sales?year={year}");
            if (yearlyResponse.IsSuccessStatusCode)
            {
                var json = await yearlyResponse.Content.ReadAsStringAsync();
                yearlyData = JsonConvert.DeserializeObject<List<YearlyReportDto>>(json);
            }
        }

        ViewBag.SelectedMonth = month?.ToString() ?? "";
        ViewBag.SelectedYear = year?.ToString() ?? "";
        ViewBag.YearlyData = JsonConvert.SerializeObject(yearlyData);

        return View(monthlyData);
    }


}
