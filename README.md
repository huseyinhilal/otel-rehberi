# Otel Rehberi Uygulamasi

Bu proje, birbirleriyle iletisim kuran iki mikroservis kullanarak otel rehberi uygulamasi olusturmaktadir. Uygulama, otel bilgilerini ve iletisim bilgilerini yonetir, ayrica otellerin konumlarina gore istatistiksel raporlar olusturur. Uygulama, asenkron mesajlasma ve RabbitMQ gibi teknolojileri kullanarak raporlama sureclerini yonetir.

## Proje ozeti

- **Otel Servisi**: Otel olusturma, otel iletisim bilgileri ekleme/kaldirma, otellerin yetkili kisilerini listeleme ve konum bazli otel sorgulama islevlerini barindirir.
- **Rapor Servisi**: Otellerin konumlarina gore istatistiksel raporlar olusturur. Raporlar asenkron olarak RabbitMQ kuyrugu uzerinden tetiklenir.

## Kullanilan Teknolojiler

- .NET Core
- Entity Framework Core (MySQL)
- RabbitMQ
- Docker
- Swagger (API dokumantasyonu)
- xUnit (Unit Testleri)
- Git

## ozellikler

- Otel bilgisi olusturma, guncelleme, silme
- Otellere sinirsiz sayida iletisim bilgisi ekleme/kaldirma
- Otellerin konumlarina gore istatistiksel rapor olusturma
- Asenkron raporlama sureci (RabbitMQ ile)
- JWT Authentication (Otel Kayıt islemleri icin)
- Lazy Loading ve Pagination ile performans optimizasyonlari

## Gereksinimler

Bu projeyi calistirmak icin asagidaki yazilimlarin sisteminizde kurulu olmasi gerekmektedir:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [MySQL](https://dev.mysql.com/downloads/)
- [RabbitMQ](https://www.rabbitmq.com/download.html)
- [Docker](https://www.docker.com/get-started) (RabbitMQ ve MySQL icin kullanilabilir)

## Projeyi calistirmak

### 1. Kod Deposu Kopyalama

Proje dosyalarini yerel bilgisayariniza kopyalayin:

```bash
git clone https://github.com/huseyinhilal/otel-rehberi.git
cd otel-rehberi
```

### 2. Veritabani Kurulumu

MySQL'i yukledikten sonra yeni bir veritabani olusturun:

``` sql
CREATE DATABASE hotel_db;
CREATE DATABASE report_db;
```

MySQL sunucunuza baglanabilmek icin appsettings.json dosyasini yapilandirin (hem HotelService hem de ReportService icin):

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=hotel_db;User=root;Password=yourpassword;"
}

---

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=report_db;User=root;Password=yourpassword;"
}

```
### 3. RabbitMQ Kurulumu

RabbitMQ'yu Docker ile ayaga kaldirmak icin su komutu kullanin:

```bash
docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### 4. Proje Ayarlari

``` json
"RabbitMQ": {
  "HostName": "localhost",
  "UserName": "guest",
  "Password": "guest"
}
```

### 5. Migration ve Veritabani Guncellemeleri

Entity Framework Core kullanarak veritabani tablolarini olusturun. Her iki mikroservis icin su adimlari takip edin:

HotelService:

```bash
cd hotel-service/HotelService
dotnet ef database update
```

ReportService:

```bash
cd report-service/ReportService
dotnet ef database update
```

### 6. Servisleri calistirma

HotelService

```bash
cd hotel-service/HotelService
dotnet run
```

ReportService

(ReportService calistirmadan once RabbitMQ'nun ayakta oldugundan emin olun)
```bash
cd report-service/ReportService
dotnet run
```

### 7. Swagger ile Test Etme
Her iki servisin API'larini test etmek icin Swagger arayuzunu kullanabilirsiniz. Uygulamalar basariyla calistiktan sonra, tarayicida asagidaki adreslere gidin (Bu adreslerdeki portlari projenin Properties klasorunun altinda launchSettings.json dosyasinda applicationUrl alanindaki portlari guncelleyerek degistirebilirsiniz): 

HotelService: https://localhost:5000/swagger
ReportService: https://localhost:7127/swagger

### 8. JWT ile Authentication
Otel CRUD islemleri icin JWT token kullanmaniz gerekiyor. Swagger uzerinden Login endpointi ile token olusturabilir ve bu token'i Authorize butonuna tiklayarak kullanabilirsiniz.

### 9. Unit Testleri calistirma
Projede xUnit testleri bulunmaktadir. Testleri calistirmak icin su komutu kullanabilirsiniz:

```bash
dotnet test
```

### 10. Lazy Loading
Lazy Loading, otel verilerini cekerken iletisim bilgileri gibi iliskili verilerin sadece gerektiginde getirilmesini saglar. Bu, performansi iyilestirir ve gereksiz veri yuklemelerini onler. Bu proje Lazy Loading'i kullanarak, otellerin iletisim bilgilerini ihtiyac aninda yukler. Bu ozelligi HotelDbContext uzerinde yapilandirildi.

### 11. Pagination
Proje buyuk veri kumelerinde performans sorunlarini onlemek icin Pagination kullanir. Pagination, buyuk veri kumelerini sayfalar halinde dondurerek sadece istenen veriyi kullaniciya iletir. Bu, otel listeleri ve raporlar gibi buyuk veri kumesi islemlerinde onemlidir.

### 12. Iletisim Bilgisi Ekleme ve Kaldirma
Otellere sinirsiz sayida iletisim bilgisi eklenebilir veya kaldirilabilir. Iletisim bilgileri, otellerle iliskilendirilen ayri bir tabloda tutulur. Bu islem asagidaki endpointler ile gerceklestirilir:

* POST /api/Hotel/{hotelId}/communication: Iletisim bilgisi ekleme
* DELETE /api/Hotel/{hotelId}/communication/{communicationId}: Iletisim bilgisi silme

### 13. Iletisim Bilgisi Listeleme (Lazy Load ile)
Otellerin iletisim bilgileri lazy load ile iliskilendirilmistir. Iletisim bilgileri sadece gerektiginde yuklenir ve performans iyilestirilir.

### API Endpoints

HotelService API

* POST /api/Hotel: Yeni otel olusturma
* GET /api/Hotel: Tum otelleri listeleme
* GET /api/Hotel/bylocation: Konum bazli otel arama
* POST /api/Hotel/{hotelId}/communication: Iletisim bilgisi ekleme
* DELETE /api/Hotel/{hotelId}/communication/{communicationId}: Iletisim bilgisi silme

ReportService API

* POST /api/Report: Rapor olusturma (RabbitMQ uzerinden calisir)
* GET /api/Report: Tum raporlari listeleme
* GET /api/Report/{id}: Belirli bir raporu goruntuleme
