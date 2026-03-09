# DEVOPS 101 - Bitirme Projesi

## Proje Açıklaması
Bu proje, GitHub Actions ile Docker Compose kullanan .NET 10 tabanlı bir Web API uygulamasının otomatik deploy edilmesi amacıyla hazırlanmıştır. Uygulama bir VPS üzerinde Ubuntu ortamında yayınlanmaktadır.

Projede iki temel endpoint bulunmaktadır:
- `GET /health` : Uygulamanın ayakta olup olmadığını kontrol eder (200 OK döner).
- `GET /api/info` : Öğrenci adı, ortam bilgisi ve sunucunun UTC zamanını JSON formatında döner.

## Kullanılan Teknolojiler
- .NET 10 (Web API, Minimal API)
- xUnit (Birim testleri)
- Docker & Docker Compose
- NGINX (Reverse proxy)
- GitHub Actions (CI/CD)

## Local Çalıştırma Adımları
Projeyi bilgisayarınızda (Docker kullanmadan) çalıştırmak için aşağıdaki komutları kullanabilirsiniz:

```bash
cd src/SimpleApi
dotnet run
```
API varsayılan olarak `http://localhost:5000` (veya `http://localhost:8080`) portunda yayın yapacaktır.

## Docker ile Çalıştırma Adımları
Sadece uygulamayı Docker üzerinde derleyip çalıştırmak için:

```bash
docker build -t simpleapi .
docker run -p 8080:8080 simpleapi
```

## Docker Compose ile Çalıştırma Adımları
NGINX reverse proxy ve Web API'yi birlikte yapılandırarak ayağa kaldırmak için:

1. `.env.example` dosyasını baz alarak bir `.env` dosyası oluşturun:
```bash
cp .env.example .env
```
2. Docker Compose ile başlatın:
```bash
docker compose up --build -d
```
3. Tarayıcınızdan test edin:
- `http://localhost:${APP_PORT}/health`
- `http://localhost:${APP_PORT}/api/info`

## Gerekli Github Secrets Listesi
Uygulamanın GitHub Actions ile otomatik deploy edilebilmesi için GitHub repository ayarlarında şu secrets'ların tanımlanmış olması gerekir:

- `VPS_HOST`: Hedef sunucunun IP adresi
- `VPS_USER`: Sunucu SSH kullanıcı adı
- `VPS_PASSWORD`: Sunucu SSH şifresi
- `DEPLOY_PATH`: Sunucuda projenin kopyalanacağı ve çalıştırılacağı dizin
- `APP_PORT`: Sunucuda uygulamanın dışarı açılacağı host portu
- `STUDENT_NAME`: Öğrenci adı ve soyadı

## Deployment Sürecinin Nasıl Tetiklendiği
Deployment süreci `main` branch'ine yapılan her **push** işleminde tetiklenmektedir. İşleyiş sırası şöyledir:
1. `build-and-test` job'ı çalışır (.NET projelerini derler ve `dotnet test` ile testleri koşturur).
2. `deploy` job'ı çalışır (Sunucuya SCP ile dosyaları kopyalar, SSH üzerinden `docker compose up --build -d` ile uygulamayı ayağa kaldırır).