

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using stajApi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace staj.Controllers
{
    [Authorize]
    public class CustomerController : BaseApiController<Customer>
    {
        public CustomerController(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory, "api/customerapi") { }
        //endpoint olusturmak icin

        public IActionResult Create()
        {
            return View(new Customer());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);

            var success = await CreateEntity(customer);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "API'ye müşteri eklenemedi.");
            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await GetById(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);

            var success = await UpdateEntity(customer.Id, customer);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "API'de müşteri güncellenemedi.");
            return View(customer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var success = await DeleteEntity(id);
            if (!success)
                ModelState.AddModelError("", "API'de müşteri silinemedi.");

            return RedirectToAction(nameof(Index));
        }
    }
}
