staj & stajApi & untitled7 — Proje Rehberi
Bu depo iki .NET 8 projesi ve bir Android istemcisi içerir:

staj: ASP.NET Core MVC tabanlı yönetim paneli (web arayüzü)
stajApi: ASP.NET Core Web API + SignalR (gerçek zamanlı mesajlaşma)
untitled7: Android (Kotlin/Java) istemci (opsiyonel)
Tüm .NET projeleri Entity Framework Core ile çalışır ve aynı çözüm (staj.sln) altındadır.

İçindekiler
Dizin Yapısı
Teknolojiler
Önkoşullar
Hızlı Başlangıç (Quickstart)
Ortam Değişkenleri ve Ayarlar
Veritabanı ve Migrasyonlar
Çalıştırma
API Özeti (Swagger)
Kimlik Doğrulama (JWT)
SignalR Mesajlaşma
Android Uygulaması (untitled7)
Sorun Giderme
Yararlı Komutlar
.gitignore İpuçları
Dizin Yapısı
staj.sln
│
├─ staj/            # Admin paneli (MVC: Controllers, Views, wwwroot)
├─ stajApi/         # API + SignalR (Controllers, Data, Hubs, Migrations, Dtos)
└─ untitled7/       # Android istemci (mobil uygulama)
Örnek konum: C:\Users\Eda Ergin\source\repos\staj\ altında staj/, stajApi/, untitled7/ klasörleri yan yana.

Teknolojiler
.NET 8 (ASP.NET Core MVC, Web API)
Entity Framework Core (Code‑First, Migrations)
SignalR (gerçek zamanlı iletişim)
Swagger / Swashbuckle (API dokümantasyonu)
Bootstrap, jQuery (UI)
Önkoşullar
.NET SDK 8.x
Bir SQL Server örneği (lokalde veya uzak)
(Android için) Android Studio + JDK 17
Hızlı Başlangıç (Quickstart)
# 1) Depoyu klonlayın
# (SSH veya HTTPS ile)

# 2) API projesi için bağlantı dizelerini ayarlayın
#    - stajApi/appsettings.Development.json içinde ConnectionStrings.DefaultConnection'ı düzenleyin

# 3) Migrasyonları veritabanına uygulayın
cd stajApi
 dotnet tool restore  # (varsayılan: dotnet-ef, dotnet-watch local tool ise)
 dotnet ef database update

# 4) Projeleri çalıştırın
# Terminal 1 – API
cd stajApi
 dotnet run

# Terminal 2 – MVC Admin Paneli
cd ../staj
 dotnet run

# 5) Tarayıcıdan erişin
# MVC:    http://localhost:<mvc_port>
# Swagger: http://localhost:<api_port>/swagger
Not: Portlar launchSettings.json dosyalarınıza göre atanır. Admin paneli, API adresini appsettings üzerinden veya HttpClient/service katmanında kullanır.

Ortam Değişkenleri ve Ayarlar
stajApi/appsettings.json ve appsettings.Development.json içinde:

ConnectionStrings.DefaultConnection
Jwt:Key, Jwt:Issuer, Jwt:Audience
Güvenli Geliştirme için User Secrets kullanın (anahtarları commit etmeyin):

cd stajApi
 dotnet user-secrets init
 dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=...;..."
 dotnet user-secrets set "Jwt:Key" "<güçlü_bir_anahtar>"
Üretimde gizli bilgiler ortam değişkenlerinden veya güvenli bir secret store üzerinden sağlanmalıdır.

Veritabanı ve Migrasyonlar
Yeni bir değişiklik yaptığınızda migrasyon ekleyin ve veritabanını güncelleyin:

cd stajApi
 dotnet ef migrations add <MigrationAdi>
 dotnet ef database update
Tavsiye: Migrasyon adlarını anlamlı verin (örn. AddChatMessage, UpdateOrderSchema).

Çalıştırma
İki projeyi ayrı terminallerde başlatın:

# API
cd stajApi
 dotnet run

# MVC
cd staj
 dotnet run
Swagger ile API uçlarını test edin (örn. http://localhost:<api_port>/swagger).
MVC tarafı, API ile haberleşmek için API Base URL’i kullanır.
API Özeti (Swagger)
Swagger UI, çalışırken şu adres altında yayınlanır:

http://localhost:<api_port>/swagger
Başlıca controller’lar:

CustomerApiController
OrderApiController
ProductApiController
ReportApiController
AuthApiController / UserApiController
İsteğe bağlı: stajApi.http veya Postman koleksiyonu ekleyin.

Kimlik Doğrulama (JWT)
Giriş uç noktası (AuthApiController veya UserApiController): token döner.
Korunan uç noktalara Authorization: Bearer <token> başlığı ile erişilir.
Örnek curl (temsilî):
curl -X POST \
  http://localhost:<api_port>/api/AuthApi/login \
  -H 'Content-Type: application/json' \
  -d '{"username":"admin","password":"P@ssw0rd"}'
Yerel geliştirme için örnek kullanıcı/seed eklemek işinizi kolaylaştırır.

SignalR Mesajlaşma
Hub: Hubs/MessageHub.cs

Kullanıcı kimliği çözümleyici: Hubs/CustomUserIdProvider.cs

Varsayılan hub yolu (örnek): /hubs/message

İstemci olayları:

ReceiveMessage (sunucudan gelen mesajların dinlenmesi)
SendMessage/Invoke (istemciden sunucuya)
CORS ve WebSockets ayarlarının dev/ürün ortamlarında uygun olduğundan emin olun.

Android Uygulaması (untitled7)
Base URL (Emulator): http://10.0.2.2:<api_port> Gerçek cihaz: http://<LAN_IP>:<api_port>

local.properties örneği:
API_BASE_URL=http://10.0.2.2:5080
build.gradle (app) örneği:
android {
  defaultConfig {
    buildConfigField("String", "API_BASE_URL", '"' + (project.findProperty("API_BASE_URL") ?: "http://10.0.2.2:5080") + '"')
  }
}
Retrofit ile örnek kullanım:
interface ProductService {
  @GET("/api/Product")
  suspend fun list(): List<ProductDto>

  @GET("/api/Product/{id}")
  suspend fun detail(@Path("id") id: Int): ProductDto
}
Cleartext HTTP gerekiyorsa Network Security Config ekleyin (Android 9+):
<!-- app/src/main/res/xml/network_security_config.xml -->
<network-security-config>
  <domain-config cleartextTrafficPermitted="true">
    <domain includeSubdomains="true">10.0.2.2</domain>
    <domain includeSubdomains="true">192.168.1.0/24</domain>
  </domain-config>
</network-security-config>
<!-- AndroidManifest.xml -->
<application
  android:networkSecurityConfig="@xml/network_security_config" ...>
</application>
Sorun Giderme
Swagger açılmıyor: API portu/URL’yi kontrol edin; launchSettings.json profilinizde HTTPS/HTTP farkına dikkat edin.
JWT çalışmıyor: Jwt:Key/Issuer/Audience uyuşmalı; sistem saatleri senkron olmalı.
EF hataları: Bağlantı dizesini ve migrasyonların uygulandığını doğrulayın (dotnet ef database update).
SignalR bağlantı hatası: CORS, hub yolu ve transport ayarlarını kontrol edin; tarayıcı konsolunu inceleyin.
Android API erişemiyor: Emulator için 10.0.2.2, cihaz için LAN IP kullanın; aynı ağda olduğundan emin olun.
Yararlı Komutlar
# EF Core
cd stajApi
 dotnet ef migrations add <MigrationAdi>
 dotnet ef database update

# Hot reload
 dotnet watch run
.gitignore İpuçları
Kök klasörde aşağıdaki dosya/klasörleri ignore edin:
# .NET
bin/
obj/

# Kullanıcı/IDE
.vscode/
*.user
*.suo

# Gizli
**/appsettings.*.local.json
**/secrets.json

# Android
untitled7/.gradle/
untitled7/.idea/
untitled7/build/
untitled7/local.properties
Güvenlik: appsettings.json içinde gerçek Jwt:Key veya üretim bağlantı dizelerini commit etmeyin. Development için User Secrets kullanın.
