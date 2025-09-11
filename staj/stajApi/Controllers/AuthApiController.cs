

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using stajApi.Data;
using stajApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace stajApi.Controllers
{
    /// <summary>
    /// login ile ilgili işlemleri içerir
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //veritabanı ile konuşmak için kullanılan bir sınıfın örneğini tanımlar
        private readonly IConfiguration _configuration; //appsettings.json gibi yapılandırma dosyalarındaki bilgileri okuyabilmek için kullanılır.
        //readonly demek: Bu değişken sadece constructor'da set edilebilir ama dışarıdan değiştirilemez.


        public AuthApiController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        /// <summary>
        /// Kullanıcı girişi yapar (User tablosu için).
        /// </summary>
        /// <param name="model">Kullanıcı adı ve şifre</param>
        /// <returns>JWT token</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
                return Unauthorized();

            var token = GenerateJwtToken(user.Id, user.Username ?? "UnknownUser", "User");
            return Ok(new { token, id = user.Id });
        }


        /// <summary>
        /// JWT token üretir (internal kullanım).
        /// </summary>
        /// <param name="id">Kullanıcı veya müşteri ID</param>
        /// <param name="name">Ad veya kullanıcı adı</param>
        /// <param name="role">Rol (User veya Customer)</param>
        /// <returns>JWT string token</returns>
        private string GenerateJwtToken(int id, string name, string role)
        {
            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim("Id", id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            //Bu token, HMAC SHA256 algoritmasıyla, şu gizli anahtar ile imzalandı
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],//	Token’ı kim oluşturdu?
                audience: _configuration["Jwt:Audience"],//Token'ı kimler kullanabilir?
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        /// <summary>
        /// Müşteri girişi yapar (Customer tablosu için).
        /// </summary>
        /// <param name="model">Telefon numarası ve şifre</param>
        /// <returns>JWT token ve müşteri ID</returns>
        [HttpPost("customer-login")]
        public IActionResult CustomerLogin([FromBody] CustomerLoginViewModel model)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.PhoneNumber == model.PhoneNumber);

            if (customer == null)
                return Unauthorized(new { message = "Geçersiz telefon numarası" });

            var hasher = new PasswordHasher<Customer>();
            var result = hasher.VerifyHashedPassword(customer, customer.Password, model.Password);

            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(customer.Id, customer.FullName ?? "UnknownCustomer", "Customer");
                return Ok(new { token, id = customer.Id });
            }

            return Unauthorized(new { message = "Hatalı şifre" });
        }



        /// <summary>
        /// Test amacıyla basit bir veri döner (authorization gerekmez).
        /// </summary>
        /// <returns>Ürün listesi</returns>
        //deneme
        [HttpGet]
        [AllowAnonymous] // 
        public IActionResult Deneme()
        {
            var products = new[] {
            new { Id = 1, Name = "Elma" },
            new { Id = 2, Name = "Armut" }
        };
            return Ok(products);
        }
    }
}




//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using stajApi.Data;
//using stajApi.Models;

//namespace stajApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthApiController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public AuthApiController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("login")]
//        public IActionResult Login([FromBody] LoginViewModel model)
//        {
//            var user = _context.Users
//                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

//            if (user == null)
//                return Unauthorized();

//            return Ok(user); // doğruysa kullanıcıyı döndür
//        }


//        [HttpPost("customer-login")]
//        public IActionResult CustomerLogin([FromBody] CustomerLoginViewModel model)
//        {
//            var customer = _context.Customers
//                .FirstOrDefault(c => c.PhoneNumber == model.PhoneNumber && c.Password == model.Password);

//            if (customer == null)
//                return Unauthorized();

//            return Ok(customer);
//        }


//    }

//}