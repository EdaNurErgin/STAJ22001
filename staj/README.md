## staj & stajApi — Proje Rehberi

Bu depo iki .NET 8 projesi içerir:

- **staj**: MVC tabanlı yönetim paneli (admin paneli / web arayüzü)
- **stajApi**: REST API + SignalR mesajlaşma altyapısı (arka uç servis)

Her iki proje de Entity Framework Core ile çalışır ve aynı çözüm (`staj.sln`) altında yer alır.

---

### Proje Yapısı

```
staj.sln
│
├─ staj/            # Admin paneli (MVC Views, Controllers, wwwroot)
├─ stajApi/         # API projesi (Controllers, Data, Hubs, Migrations)
└─ untitled7/       # Android istemci (mobil uygulama)
```

Not: `untitled7` ile `staj` aynı kök klasörde konumlanır. Örn:

```
C:\Users\Eda Ergin\source\repos\staj\
  staj\
  stajApi\
  untitled7\
```

---

### Teknolojiler

- .NET 8 (ASP.NET Core MVC, Web API)
- Entity Framework Core (Code-First, Migrations)
- SignalR (gerçek zamanlı mesajlaşma)
- Bootstrap, jQuery (UI)

---

### Önkoşullar

- .NET SDK 8.x
- Bir SQL veritabanı (varsayılan konfigürasyona göre ayarlayın)

---

### Kurulum

1) Depoyu klonlayın veya indirin.
2) Gerekirse `stajApi/appsettings.json` ve `staj/appsettings.json` dosyalarındaki bağlantı dizelerini (ConnectionStrings) düzenleyin.
3) Veritabanı migrasyonlarını uygulayın (API projesi üzerinden):

```bash
cd stajApi
dotnet ef database update
```

> Not: EF CLI yüklü değilse `dotnet tool install --global dotnet-ef` komutunu kullanın.

---

### Çalıştırma

İki projeyi ayrı terminallerde başlatın:

```bash
# Terminal 1: API
cd stajApi
dotnet run

# Terminal 2: Admin paneli (MVC)
cd staj
dotnet run
```

Varsayılan olarak her proje kendi `launchSettings.json` ayarlarına göre port atar. Admin paneli, API ile haberleşmek için `stajApi` adresini kullanır.

---

### staj (Admin Paneli)

- Modüller: Kullanıcı, Müşteri, Ürün, Sipariş, Raporlar, Sohbet
- Görünümler `staj/Views/` altındadır.
- Statik dosyalar `staj/wwwroot/` altındadır.

---

### stajApi (API & SignalR)

- REST uç noktaları `stajApi/Controllers/` içerisindedir (ör. `CustomerApiController`, `OrderApiController`, `ProductApiController`, `ReportApiController`).
- Kimlik/oturum yapısı için `AuthApiController` ve/veya `UserApiController` kullanılır.
- SignalR gerçek zamanlı mesajlaşma: `Hubs/MessageHub.cs` ve `Hubs/CustomUserIdProvider.cs`.
- EF Core veri erişimi: `Data/ApplicationDbContext.cs`.
- Rapor DTO’ları: `Dtos/MonthlyReportDto.cs`, `Dtos/YearlyReportDto.cs`.

Örnek çalıştırma:

```bash
cd stajApi
dotnet run
```

API çalıştığında Swagger veya HTTP istemcisi ile uç noktalar test edilebilir (varsa `stajApi.http`).

---

### Migrasyon Yönetimi (EF Core)

- Yeni migrasyon oluşturma:

```bash
cd stajApi
dotnet ef migrations add <MigrationAdi>
```

- Veritabanını güncelleme:

```bash
dotnet ef database update
```

---

### Ortam Dosyaları

- `appsettings.json` ve `appsettings.Development.json` dosyalarında ortam değişkenleri ve bağlantı bilgileri bulunur.
- Yerel geliştirmede `Development` profili kullanılır.

---

### Dağıtım İpuçları

- API ve Admin panelini ayrı servisler olarak yayınlayın.
- `ASPNETCORE_ENVIRONMENT` değişkenini ortama uygun şekilde ayarlayın (Production/Development).
- Bağlantı dizelerini ve gizli anahtarları ortam değişkenlerinden yönetin.

---

### Android (Mobil Uygulama — `untitled7`)

`untitled7` tam teşekküllü bir Android uygulamasıdır ve kullanıcı arayüzü (user UI) içerir. Uygulama, `stajApi` ile haberleşerek kullanıcı giriş işlemleri, ürün/sipariş işlemleri ve (gerekiyorsa) gerçek zamanlı sohbet gibi fonksiyonları sunar.

> Yerleşim: `untitled7/` klasörü, `staj` ve `stajApi` ile aynı köktedir. Android Studio ile doğrudan `untitled7` klasörünü açabilirsiniz.

**Öne Çıkan Özellikler (muhtemel modüller)**

- Kullanıcı oturum açma/çıkış (Auth)
- Ürün listeleme ve detay görüntüleme
- Sepet/Sipariş akışı (varsa)
- Gerçek zamanlı mesajlaşma/sohbet (SignalR ile, varsa ilgili ekran)

> Not: Modüller ve ekranlar projedeki mevcut UI’ya göre farklılık gösterebilir. Bu README, API tarafındaki controller’lara göre genel bir özet sunar.

**Önkoşullar**

- Android Studio (Hedgehog veya üstü), JDK 17
- Gradle ile Kotlin tercih edilir (Java da mümkündür)

**API Taban Adresi (Base URL) Konfigürasyonu**

- Geliştirmede `stajApi` çalıştığı makinenin IP/port’u kullanılmalı (ör: `http://10.0.2.2:5080` Emulator için, `http://<LAN_IP>:5080` gerçek cihaz için).
- Önerilen yaklaşım: `local.properties` içine ekleyin ve `BuildConfig`’e aktarın.

```properties
# local.properties
API_BASE_URL=http://10.0.2.2:5080
```

```kotlin
// build.gradle (app) — örnek
android {
    defaultConfig {
        buildConfigField("String", "API_BASE_URL", '"' + (project.findProperty("API_BASE_URL") ?: "http://10.0.2.2:5080") + '"')
    }
}
```

**HTTP İstekleri (Retrofit kullanımı)**

- JSON iletişimi için Retrofit + OkHttp + Kotlin Serialization/Moshi kullanın.
- Yetkili isteklerde `Authorization: Bearer <token>` başlığı ekleyin (giriş akışına göre alınan JWT/Token).

```kotlin
interface ProductService {
    @GET("/api/Product")
    suspend fun list(): List<ProductDto>

    @GET("/api/Product/{id}")
    suspend fun detail(@Path("id") id: Int): ProductDto
}

val retrofit = Retrofit.Builder()
    .baseUrl(BuildConfig.API_BASE_URL)
    .addConverterFactory(MoshiConverterFactory.create())
    .build()

val productService = retrofit.create(ProductService::class.java)
```

**Kimlik Doğrulama (Login/Token)**

- `AuthApiController` veya `UserApiController` uç noktaları ile giriş yapın, dönen token’ı güvenle saklayın (EncryptedSharedPreferences önerilir).
- Tüm korumalı çağrılara token başlığı ekleyin.

**Gerçek Zamanlı Sohbet (SignalR) — varsa**

- Java/Kotlin istemci: `com.microsoft.signalr:signalr` kütüphanesi.
- Hub URL: `<API_BASE_URL>/hubs/message` (projedeki `MessageHub` yolunu doğrulayın).
- Örnek akış: bağlantı kur → `on` ile mesajları dinle → `send`/`invoke` ile mesaj gönder.

**Ağ Güvenliği / HTTP**

- Geliştirmede HTTP kullanıyorsanız Android 9+ için Network Security Config ile cleartext’e izin verin veya HTTPS kullanın.

**Geliştirme Akışı**

1) `stajApi`’yi çalıştırın ve erişilebilir olduğundan emin olun.
2) `API_BASE_URL`’ü emulator/cihaz için doğru IP/port ile ayarlayın.
3) Android uygulamasında login → token al → API çağrılarını gerçekleştirin.
4) Sohbet gerekiyorsa SignalR bağlantısını kurun.

**Android Studio ile açma/derleme**

1) Android Studio → Open → `untitled7/`
2) Gradle sync tamamlandıktan sonra bir emulator/cihaz seçin → Run


---

### İletişim / Notlar

- Bu README, projeyi hızlıca ayağa kaldırmak ve genel yapıyı anlatmak için hazırlanmıştır.
- Sorular için proje sahipleriyle iletişime geçebilirsiniz.


