# YD CarPlay — Sorun Giderme ve Çözüm Rehberi (Troubleshooting)

Bu belgede, YD CarPlay kullanımı esnasında karşılaşılabilecek olası durumlar ve çözüm yöntemleri listelenmiştir.

---

## 1. "Oyun Açık Ama Ekranda 'Simülasyon Modu' Yazıyor"

- **Neden:** ETS2 başlatıldığında üçüncü taraf telemetri eklentisinin yüklenmesine izin verilmemiş olabilir veya eklenti 32-bit dizinine atılmış olabilir.
- **Çözüm:**
  1. Oyun açılışında çıkan sarı bilgi kutucuğunda ("SDK eklentisi algılandı") **"Evet" (Yes)** seçeneğini işaretlediğinizden emin olun.
  2. Dosyanın mutlaka `bin\win_x64\plugins\` klasöründe olduğundan emin olun (`bin\win_x86` artık modern ETS2 sürümlerinde desteklenmemektedir).
  3. ETS2 belgelerim klasöründeki `game.log.txt` dosyasını açıp `[sdk] plugin ... loaded successfully` satırını arayın.

---

## 2. "YouTube veya Spotify Ekranı Boş / Beyaz Görünüyor"

- **Neden:** Windows işletim sisteminizde Microsoft Edge WebView2 Runtime bileşeni eksik veya güncel değil.
- **Çözüm:**
  1. Microsoft'un resmî web sitesinden [WebView2 Evergreen Bootstrapper](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) indirip kurun.
  2. Bilgisayarınızın internet bağlantısının aktif olduğunu doğrulayın.

---

## 3. "Canlı Görüntü Akışı (Port 5055) Açılmıyor"

- **Neden:** 5055 numaralı port başka bir Windows uygulaması tarafından kullanılıyor olabilir.
- **Çözüm:**
  1. YD CarPlay ana ekranından **Ayarlar** uygulamasına gidin.
  2. **Görüntü Aktarım Sunucusu** bölümünden portu `5056` veya `8088` gibi boş bir porta değiştirin.
  3. Tarayıcınızdan `http://localhost:5056/stream` adresine giderek akışı test edin.

---

## 4. "ETS2 Çerçevesiz Pencereli Modda CarPlay Penceresi Arkada Kalıyor"

- **Neden:** Oyun tam ekran (exclusive fullscreen) modunda çalışıyor olabilir.
- **Çözüm:**
  1. ETS2 Grafik Seçenekleri'nden ekran modunu **"Çerçevesiz Pencere" (Borderless Windowed)** olarak ayarlayın.
  2. YD CarPlay Ayarlar menüsünden **"Her Zaman Üstte Tut" (Always On Top)** seçeneğini aktif edin.
  3. Pencere şeffaflık çubuğunu isteğinize göre %90 - %100 arasına ayarlayın.
