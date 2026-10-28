# YD CarPlay — Kurulum Kılavuzu (Installation Guide)

Bu kılavuz, YD CarPlay sisteminin bilgisayarınıza ve Euro Truck Simulator 2 (ETS2) oyununa nasıl kurulacağını adım adım açıklamaktadır.

---

## 1. Sistem Gereksinimleri

- **İşletim Sistemi:** Windows 10 veya Windows 11 (64-bit)
- **Çalışma Zamanı (Runtime):** 
  - Microsoft .NET 8.0 Desktop Runtime (x64)
  - Microsoft Edge WebView2 Runtime (YouTube ve Web Spotify için gereklidir)
- **Oyun:** Euro Truck Simulator 2 (Steam 64-bit sürümü, v1.40 - 1.52+)
- **İşlemci / Bellek:** Çift çekirdekli 2.0 GHz veya üzeri, 4 GB RAM

---

## 2. Otomatik Kurulum (Önerilen)

Projeyle birlikte gelen **YDCarPlay.Installer** aracı tüm adımları tek tıkla otomatik olarak gerçekleştirir:

1. `src/YDCarPlay.Installer/bin/Debug/net8.0-windows/YDCarPlay.Installer.exe` uygulamasını çalıştırın.
2. Kurulum aracı sisteminizdeki .NET 8 ve WebView2 çalışma zamanlarını doğrular.
3. Steam Registry kaydından ETS2 ana dizinini (`steamapps\common\Euro Truck Simulator 2`) otomatik tespit eder.
4. Oyun dizininde `bin\win_x64\plugins` klasörünü hazırlar.
5. Mevcut dosyaları tarih damgalı olarak yedekler (`scs-telemetry.dll.bak_...`).
6. **"🚀 YD CarPlay'i Kur"** butonuna basarak kurulumu tamamlayın.

---

## 3. Manuel Kurulum Adımları

Kurulumu el ile yapmak isterseniz:

1. ETS2 oyununun kurulu olduğu dizine gidin:
   ```
   C:\Program Files (x86)\Steam\steamapps\common\Euro Truck Simulator 2\bin\win_x64\
   ```
2. `win_x64` klasörü içinde `plugins` adında bir klasör yoksa oluşturun.
3. Resmî `scs-telemetry.dll` eklenti dosyasını bu `plugins` klasörünün içine kopyalayın:
   ```
   ...\Euro Truck Simulator 2\bin\win_x64\plugins\scs-telemetry.dll
   ```
4. ETS2 oyununu başlatın. Oyun açılışında **"SDK Telemetry / Third-party plugin detected. Do you want to load it?"** uyarısı çıktığında **"Yes"** (Evet) seçeneğini seçin.
5. `YDCarPlay.App.exe` uygulamasını başlatın.

---

## 4. Temiz Kaldırma (Uninstallation)

Uygulama ve oyun dosyaları kullanıcının onayı olmadan silinmez veya bozulmaz:

1. **Kurulum Aracı ile Kaldırma:** `YDCarPlay.Installer.exe` açıp sol alttaki **"Kaldır (Uninstall)"** butonuna tıklayın. Alınan orijinal yedekler geri yüklenir ve eklenti yapılandırmaları temizlenir.
2. **El ile Kaldırma:** `plugins\yd_carplay_telemetry.ini` dosyasını silmeniz yeterlidir. Oyun kayıtlarınız veya profil verileriniz hiçbir şekilde etkilenmez.
