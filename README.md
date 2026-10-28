# YD CarPlay — ETS2 İçin Gelişmiş CarPlay Multimedya Sistemi

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/UI-WPF%20%2F%20XAML-indigo.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![ETS2 Telemetry](https://img.shields.io/badge/ETS2-SCS%20SDK%20v1.11--1.14-green.svg)](https://scssoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Euro Truck Simulator 2 (ETS2) için C# ve .NET 8 WPF kullanılarak geliştirilmiş profesyonel, modern ve ultra gerçekçi bir **YD CarPlay** sistemidir.

Tır kabinlerindeki GPS ve multimedya ekranlarının yerini almak üzere Apple CarPlay tasarım diline sadık kalınarak otomotiv standartlarında tasarlanmıştır.

---

## 📸 Arayüz Tasarımı ve Uygulamalar

Arayüz, kullanıcının talep ettiği modern koyu temalı, cam efektli (glassmorphism) CarPlay görünümüne tam uyumlu 10 uygulamadan oluşur:

1. **Telefon:** CarPlay tuş takımı, kişiler listesi, arama geçmişi ve Bluetooth bağlantı durumu.
2. **Müzik:** Yerel müzik ve radyo çalar, çalma listesi, albüm kapağı ve süre ilerleme çubuğu.
3. **Harita:** ETS2 dünya koordinatlarıyla gerçek zamanlı senkronize GPS pusulası, hız limitleri ve tahmini varış süresi (ETA).
4. **Mesajlar:** Lojistik yük bildirimleri, garaj bakım raporları ve Siri sesli yanıt simülasyonu.
5. **Şimdi Çalınan:** Dinamik ses dalgası ekolayzır göstergesi ve medya kontrolleri.
6. **YouTube:** Microsoft WebView2 destekli video arama, trendler ve güvenli sürüş uyarı sistemi.
7. **Spotify:** Spotify Web Player arayüzü ve yerel oynatma kontrolleri.
8. **Podcast'ler:** Tır şoförü sohbetleri ve uzun yol podcast kanalları.
9. **App Store:** Dijital takograf asistanı, yakıt hesaplayıcı ve CB telsiz araçları.
10. **Ayarlar:** Çözünürlük, tam ekran (F11), şeffaf HUD modu, dil (TR/EN) ve telemetri yapılandırması.

Ek olarak:
- **Sayfa 2 (Kokpit Paneli):** Büyük dijital hız göstergesi, motor devir saati (RPM), vites konumu (`D12`, `N`, `R1`), yakıt göstergesi ve sefer detayları.
- **Sayfa 3 (Canlı Yayın Köprüsü):** Tır kabinine tablet veya telefon bağlamak için düşük gecikmeli canlı MJPEG HTTP yayın sunucusu (`http://localhost:5055/stream`).

---

## 🛠️ Mimari ve Klasör Yapısı

```
YDCarPlay/
├── YDCarPlay.sln
├── src/
│   ├── YDCarPlay.App/               # WPF Ana CarPlay Uygulaması
│   │   ├── Controls/                # AppIconCard vb. özel CarPlay kontrolleri
│   │   ├── Resources/               # Vektörel XAML simgeler ve stiller
│   │   ├── Services/                # MediaService (Ses motoru)
│   │   ├── ViewModels/              # MVVM ViewModel katmanı
│   │   └── Views/                   # Home, Nav, YouTube, Spotify, Phone vb.
│   ├── YDCarPlay.Core/              # Çekirdek modeller ve servisler
│   │   ├── Models/                  # AppSettings, TruckTelemetryData vb.
│   │   ├── Interfaces/              # ITelemetryService, ISettingsService vb.
│   │   └── Services/                # SettingsService, ScreenStreamingServer
│   ├── YDCarPlay.Telemetry/         # ETS2 SCS Telemetri motoru
│   │   ├── ScsSharedMemoryReader.cs # Local\SCSTelemetry bellek haritası okuyucu
│   │   ├── SimulatedTelemetryService.cs # Oyun kapalıyken gerçekçi simülatör
│   │   └── TelemetryService.cs      # Birleşik telemetri koordinatörü
│   └── YDCarPlay.Installer/         # Otomatik Windows Kurulum Aracı
├── tests/
│   └── YDCarPlay.Tests/             # xUnit Birim Testleri (9/9 Başarılı)
├── docs/
│   ├── INSTALLATION.md              # Adım adım kurulum rehberi
│   ├── ETS2_INTEGRATION.md          # 3D ekran ve teknik doku analizi
│   └── TROUBLESHOOTING.md           # Hata ve çözüm kılavuzu
└── README.md
```

---

## 🚀 Hızlı Başlangıç

### Derleme:
```powershell
dotnet build
```

### Testleri Çalıştırma:
```powershell
dotnet test
```

### CarPlay Uygulamasını Başlatma:
```powershell
dotnet run --project src/YDCarPlay.App/YDCarPlay.App.csproj
```

### Otomatik Kurulum Asistanını Başlatma:
```powershell
dotnet run --project src/YDCarPlay.Installer/YDCarPlay.Installer.csproj
```

---

## 🎮 ETS2 Oyun İçi Kullanım

1. **Simülasyon Modu:** ETS2 açık değilken bile uygulama hız, devir, vites ve harita hareketlerini otomatik olarak simüle eder.
2. **Canlı ETS2 Bağlantısı:** ETS2'yi başlattığınızda telemetri servisi otomatik olarak gerçek tır verilerine kilitlenir.
3. **Ekran Aktarımı:** Tarayıcınızdan `http://localhost:5055` adresini açarak telefonunuzda veya tabletinizde kablosuz CarPlay olarak kullanabilirsiniz.
