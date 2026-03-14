# DevOps 101 - Bitirme Projesi

Bu repo, DevOps 101 eğitimi için bitirme projesi olarak hazırlanmıştır. Proje, Docker Compose ve GitHub Actions kullanılarak VPS üzerine otomatik olarak dağıtılan (deploy edilen) **.NET 10** tabanlı bir Web API uygulamasını içerir.

---

## 🚀 Proje İçeriği ve Özellikler

Uygulama, temel sağlık durumunu ve ortam bilgilerini sunan iki basit endpoint barındırır:

- **`GET /health`**
  - Uygulama ayaktaysa `200 OK` döner.
- **`GET /api/info`**
  - Eğitimde istenen özel formatta; öğrenci adını, uygulamanın çalıştığı ortamı ve sunucu zamanını JSON olarak döndürür.

### 🛠 Kullanılan Teknolojiler

- **Backend:** .NET 10 (ASP.NET Core Web API, Controller tabanlı) & C#
- **Test:** xUnit
- **Containerization:** Docker & Docker Compose
- **Web Sunucusu:** NGINX (Reverse Proxy)
- **CI/CD:** GitHub Actions

---

## 💻 Geliştirme Ortamında Çalıştırma

Projeyi kendi bilgisayarınızda farklı yöntemlerle çalıştırabilirsiniz.

### 1. Docker Compose ile (Önerilen)
Web API ve Nginx servislerini tam entegre çalıştırır.

```bash
# Değişken şablonunu asıl dosyaya kopyalayın
cp .env.example .env

# Uygulamayı ayağa kaldırın
docker-compose up --build -d
```
Test için:
- `http://localhost:<APP_PORT>/health`
- `http://localhost:<APP_PORT>/api/info`

### 2. Sadece Docker ile
Nginx olmadan yalın API uygulamasını konteynerde çalıştırır.

- Varsayılan olarak **Development** ortamında çalışır.
- İsterseniz `ASPNETCORE_ENVIRONMENT_OVERRIDE` ile ortamı ezebilirsiniz (örn. `Production`).

```bash
docker build -t simpleapi .

# Geliştirme (varsayılan) için
docker run -p 8080:8080 simpleapi

# Production ortamı simüle etmek için
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT_OVERRIDE=Production simpleapi
```

### 3. Native .NET ile (Geliştirici Modu)
Docker kullanmadan, projeyi doğrudan makinenizde çalıştırır.

```bash
cd src/SimpleApi
dotnet run
```

---

## ⚙️ CI/CD ve Otomatik Dağıtım (Deployment)

Projede sürekli entegrasyon ve dağıtım (**CI/CD**) süreçleri için GitHub Actions kullanılmaktadır. 

`main` dalına (branch) yapılan her yeni **push** işleminde:
1. Kaynak kod çekilir ve .NET 10 SDK kurulur.
2. Tüm projenin derlenmesi ve **xUnit** testleri çalıştırılır. (`build-and-test`)
3. Testler başarısız olursa süreç durdurulur ve hatalı kodun sunucuya gitmesi engellenir.
4. Testler başarılıysa, **Appleboy SSH/SCP Action** kullanılarak tüm proje klasörleri ve değişkenler güvenli bir şekilde hedef VPS sunucusuna aktarılır.
5. Sunucuda bulunan eski Docker imajları silinir ve yeni kodlarla birlikte proje tekrar ayağa kaldırılır.

### 🔐 Gerekli GitHub Secrets
Bu otomatik kurulumun çalışması için, projenizi klonladıktan sonra kendi GitHub Repository ayarlarınızda (`Settings > Secrets and variables > Actions`) aşağıdaki gizli değişkenlerin (secrets) tanımlı olması gerekmektedir:

| Değişken Adı | Açıklama |
|---|---|
| `VPS_HOST` | Hedef sunucunun IP adresi |
| `VPS_USER` | Sunucu SSH kullanıcı adı (Örn: ubuntu, root) |
| `VPS_PASSWORD` | Sunucu SSH şifresi |
| `DEPLOY_PATH` | Sunucuda projenin kurulacağı tam dizin yolu (Örn: `/home/ubuntu/proje`) |
| `APP_PORT` | Uygulamanın sunucu dışına açılacağı yayın portu (Örn: `8080`) |
| `STUDENT_NAME` | Kendi adınız ve soyadınız |