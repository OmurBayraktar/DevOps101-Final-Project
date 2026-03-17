# DevOps 101 — Bitirme Projesi

**.NET 10** tabanlı bir Web API uygulaması; Docker, Docker Compose ve GitHub Actions ile konteynerleştirme, test ve VPS’e otomatik dağıtım örneklerini içerir.

---

## İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Ön Gereksinimler](#-ön-gereksinimler)
- [API Referansı](#-api-referansı)
- [Ortam Değişkenleri](#-ortam-değişkenleri)
- [Proje Yapısı](#-proje-yapısı)
- [Yerel Çalıştırma](#-yerel-çalıştırma)
- [CI/CD Pipeline](#-cicd-pipeline)
- [GitHub Secrets](#-github-secrets)

---

## 🚀 Özellikler

- Sağlık kontrolü ve ortam bilgisi sunan REST endpoint’leri
- xUnit ile birim testleri
- Çok aşamalı (multi-stage) Docker build
- Docker Compose ile API + NGINX reverse proxy
- GitHub Actions ile build, test, imaj yayını ve VPS’e SSH/SCP dağıtımı

---

## 🛠️ Teknolojiler

| Katman        | Teknoloji |
|---------------|-----------|
| Backend       | .NET 10, ASP.NET Core Web API (controller tabanlı), C# |
| Test          | xUnit |
| Konteyner     | Docker, Docker Compose |
| Reverse proxy| NGINX (Alpine) |
| CI/CD         | GitHub Actions (checkout, setup-dotnet, docker/build-push, appleboy/scp-action, appleboy/ssh-action) |

---

## 📋 Ön Gereksinimler

- **Yerel geliştirme:** [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **Docker ile çalıştırma:** [Docker](https://docs.docker.com/get-docker/) ve (isteğe bağlı) [Docker Compose](https://docs.docker.com/compose/install/)
- **CI/CD kullanımı:** GitHub repo, VPS (SSH erişimi) ve gerekli [GitHub Secrets](#-github-secrets)

---

## 🔗 API Referansı

| Method | Endpoint     | Açıklama |
|--------|--------------|----------|
| `GET`  | `/`          | Ana sayfa (HTML). |
| `GET`  | `/health`    | Sağlık kontrolü. Başarılı yanıt: `200 OK` (metin). |
| `GET`  | `/api/info`  | Ortam bilgisi (JSON). Öğrenci adı `STUDENT_NAME` ortam değişkeninden okunur. |

**`GET /api/info` örnek yanıt:**

```json
{
  "student": "Ömür Bayraktar",
  "environment": "Development",
  "serverTimeUtc": "2025-03-14T12:00:00Z"
}
```

`STUDENT_NAME` tanımlı değilse `"Unknown Student"` döner. Development ortamında OpenAPI arayüzü (`MapOpenApi`) etkindir.

---

## 🔑 Ortam Değişkenleri

| Değişken                  | Açıklama | Varsayılan / Kullanım |
|---------------------------|----------|------------------------|
| `APP_PORT`                | NGINX’in dinlediği host portu (Docker Compose). | `.env.example`: `8080` |
| `ASPNETCORE_ENVIRONMENT`  | Çalışma ortamı (`Development`, `Production` vb.). | Yerel compose: `Development`; VPS deploy: `Production` |
| `STUDENT_NAME`            | `/api/info` yanıtında görünen öğrenci adı. | Örnek: `Ömür Bayraktar` |
| `IMAGE_URL`               | Kullanılacak API imajı (Docker Compose). | Yerel: build kullanılır; deploy: `ghcr.io/<repo>:latest` |

`.env` oluşturmak için: `cp .env.example .env` ve değerleri düzenleyin.

---

## 📁 Proje Yapısı

```
├── .github/workflows/
│   └── deploy.yml              # CI/CD: build-test, publish image, deploy
├── nginx/
│   └── default.conf            # NGINX reverse proxy (API:8080 → 80)
├── src/
│   ├── SimpleApi/              # Web API
│   │   ├── Controllers/
│   │   │   └── SystemInfoController.cs
│   │   ├── Program.cs
│   │   ├── index.html
│   │   └── ...
│   └── SimpleApi.Tests/         # xUnit testleri
│       ├── UnitTest1.cs
│       └── SimpleApi.Tests.csproj
├── .env.example
├── .gitignore
├── .dockerignore
├── Dockerfile                  # Multi-stage: build → runtime
├── docker-compose.yml          # api + nginx servisleri
└── README.md
```

---

## 💻 Yerel Çalıştırma

### 1. Docker Compose (önerilen)

API ve NGINX birlikte çalışır. Port, `.env` içindeki `APP_PORT` ile belirlenir (varsayılan `8080`).

```bash
# Docker Compose başlatmak için:
docker compose --env-file .env up --build -d

# Docker Compose durdurmak için:
docker compose down
```

Erişim:
- `http://localhost:8080/health`
- `http://localhost:8080/api/info`
- `http://localhost:8080/` (ana sayfa)

`APP_PORT` farklı ise `8080` yerine o portu kullanın.

### 2. Sadece Docker (NGINX olmadan)

```bash
# Api`yi docker da başlatmak için:
docker build -t simpleapi . && docker run -d --name api --rm -p 8080:8080 --env-file .env simpleapi

# Api`yi durdurmak için:
docker stop api
```

Erişim: `http://localhost:8080/health`, `http://localhost:8080/api/info`.

### 3. .NET ile (Docker kullanmadan)

Varsayılan profil `launchSettings.json` ile **Development** ortamını kullanır.

```bash
cd src/SimpleApi
dotnet run
```

Uygulama `http://localhost:5024` (veya launchSettings’te tanımlı URL) üzerinde çalışır. `STUDENT_NAME` için ortam değişkenini shell’de tanımlayın veya `Properties/launchSettings.json` içine ekleyin.

---

## 🔄 CI/CD Pipeline

`main` dalına her **push** tetiklenir.

| Aşama | Açıklama |
|--------|----------|
| **build-and-test** | Checkout, .NET 10 SDK kurulumu, `SimpleApi.Tests` için restore → build → test. Test başarısızsa pipeline durur. |
| **publish-image** | GitHub Container Registry’ye giriş, Docker imajı build edilir ve `ghcr.io/<repository>:latest` olarak push edilir. |
| **deploy** | `docker-compose.yml` ve `nginx/` klasörü SCP ile VPS’teki `DEPLOY_PATH`’e kopyalanır. SSH ile sunucuda `.env` oluşturulur (`APP_PORT`, `ASPNETCORE_ENVIRONMENT=Production`, `STUDENT_NAME`, `IMAGE_URL=ghcr.io/...`) ve `docker-compose pull` + `docker-compose up -d` çalıştırılır; kullanılmayan imajlar prune edilir. |

---

## 🛡️ GitHub Secrets

Otomatik dağıtım için repository’de **Settings → Secrets and variables → Actions** altında aşağıdaki secret’lar tanımlanmalıdır:

| Secret          | Açıklama |
|-----------------|----------|
| `VPS_HOST`      | VPS IP adresi |
| `VPS_USER`      | SSH kullanıcı adı (örn. `ubuntu`, `root`) |
| `VPS_PASSWORD`  | SSH parolası |
| `DEPLOY_PATH`   | Sunucuda proje dizini (örn. `/home/ubuntu/devops101`) |
| `APP_PORT`      | Dışarıya açılacak port (örn. `8080`) |
| `STUDENT_NAME`  | `/api/info` çıktısında görünecek ad soyadı |

---

*DevOps 101 eğitim projesi — .NET 10, Docker ve GitHub Actions ile örnek CI/CD akışı.*
