# YD CarPlay — ETS2 Oyun İçi Entegrasyon & Teknik Mimari Raporu

Bu doküman, YD CarPlay projesinin Euro Truck Simulator 2 (ETS2) Prism3D oyun motoru ile olan telemetri ve görüntü aktarımı entegrasyonunun derinlemesine teknik analizini ve uygulama yöntemlerini açıklamaktadır.

---

## 1. ETS2 Prism3D Motoru ve Kokpit Ekran Mimarisi

Euro Truck Simulator 2, SCS Software'in tescilli **Prism3D** motoru üzerinde DirectX 11 API'si kullanılarak çalışır. Tır kabinlerindeki (interior) GPS ve multimedya ekranları şu katmanlardan meydana gelir:

1. **3D Model (`interior.pmg`):** Kabin içi geometrisinde GPS ekranının bulunduğu yüzey ayrı bir alt ağ (submesh) olarak tanımlanır ve UV haritası belirli bir doku koordinatına eşlenir.
2. **Malzeme Dosyası (`.mat`):** Örneğin `/material/ui/dashboard/gps.mat`. Genellikle `eut2.dif.spec.mult` veya `eut2.ui` gölgelendirici (shader) efektini kullanır.
3. **Arayüz Tanımı (`dashboard.sii` & `gps.sii`):** Oyun içi Prism3D arayüz sistemi, `.sii` dosyalarındaki layout tanımlarına göre hız, rota çizgisi ve harita katmanlarını DirectX yüzeyine çizer.

### Önemli Teknik Kısıtlama:
ETS2 motoru, oyun dışından gelen harici bir video sinyalini (WPF, HWND veya video akışı) doğrudan 3D kabin dokusuna dinamik olarak enjekte eden bir resmî API sunmaz. Bu nedenle doğrudan WPF penceresini açmak, görüntüyü kendiliğinden tırın konsolundaki dokuya aktarmaz.

---

## 2. Görüntü Aktarım Yöntemleri ve Çözüm Mimarisi

YD CarPlay, simülasyon oyuncularının farklı donanım ve kullanım senaryolarına uyum sağlamak üzere 3 temel aktarım köprüsü sunar:

### Yöntem A: Dahili Yüksek Performanslı Görüntü Sunucusu (MJPEG / HTTP Stream)
YD CarPlay Core içerisinde yer alan `ScreenStreamingServer` sınıfı, WPF arayüzünü 30-60 FPS hızında yakalayarak yerel ağ üzerinden `http://localhost:5055/stream` adresinden canlı bir MJPEG video akışı yayınlar.

- **Kullanım Senaryoları:**
  - Tır direksiyon setinin yanına veya kokpite monte edilen tablet / akıllı telefonlarda kablosuz gerçek CarPlay deneyimi.
  - İkinci bir küçük araç içi monitörde (HDMI/USB LCD display) görüntüleme.
  - OBS Studio Browser Source veya Overwolf üzerinden oyun içine HUD yerleşimi.
- **Performans:** 15-20 ms düşük gecikme, ETS2 oyun kare hızına (FPS) sıfır yük.

### Yöntem B: Çerçevesiz Pencereli Oyun İçi Sabitlenmiş Katman (Borderless HUD Overlay)
ETS2 çerçevesiz pencereli (borderless windowed) modda çalıştırıldığında, YD CarPlay Ayarlar menüsünden **"Her Zaman Üstte Tut" (Always On Top)** ve **"Pencere Şeffaflığı"** aktif edilir.
- Uygulama penceresi boyutlandırılarak tırın konsolundaki GPS ekranının tam üzerine denk getirilir.
- Kullanıcı tır sürerken doğrudan kabin içindeki ekranın yerinde canlı CarPlay arayüzünü görür ve tıklar.

### Yöntem C: DirectX 11 Hooking & Shared Texture Enjeksiyonu (İleri Seviye Modlama)
- **Teknik Yaklaşım:** `dxgi.dll` ve `d3d11.dll` fonksiyonları kancalanarak (hook), Prism3D motorunun `ID3D11DeviceContext::UpdateSubresource` çağrısı esnasında GPS dokusunun (`gps.dds`) GPU bellek işaretçisi YD CarPlay tarafından paylaşılan `DXGI Shared Handle` ile takas edilir.
- **Dikkat Edilmesi Gerekenler:** TruckersMP veya anti-cheat sistemleri harici DLL kancalamalarını hile olarak algılayabileceğinden, YD CarPlay bu işlemi oyuncunun güvenliği için varsayılan olarak izole bir HTTP/MJPEG sunucusu üzerinden çalıştırmayı tercih eder.

---

## 3. SCS Telemetri Protokolü ve Bellek Haritası (Shared Memory)

YD CarPlay, ETS2 ile çift yönlü iletişim için resmî SCS SDK Telemetry eklentisini (`scs-telemetry.dll`) kullanır. Eklenti, Windows üzerinde isimli paylaşımlı bellek (Shared Memory Map) tahsis eder:

- **Birincil Kanal:** `Local\SCSTelemetry`
- **Yedek Kanal:** `Local\SCS\ETS2`
- **Boyut:** 32.768 Bayt (32 KB)

### Okunan Temel Telemetri Verileri:

| Veri Alanı | Bellek Ofseti | Veri Tipi | Açıklama |
|---|---|---|---|
| `sdkActive` | 0 | `uint32` | Eklenti ve oyunun aktiflik durumu (0 = Kapalı) |
| `paused` | 4 | `uint32` | Oyunun duraklatılma durumu |
| `time` | 8 | `uint64` | Oyun içi mutlak zaman (dakika) |
| `speed` | 40 | `float` | Araç hızı (m/s -> km/s için × 3.6) |
| `engineRpm` | 44 | `float` | Motor devir sayısı |
| `gear` | 68 | `int32` | Fiziksel vites konumu (-1: Geri, 0: Boş, 1..16: İleri) |
| `displayedGear`| 72 | `int32` | Gösterge tablosundaki vites |
| `fuel` | 80 | `float` | Mevcut yakıt miktarı (Litre) |
| `fuelCapacity` | 84 | `float` | Depo toplam hacmi (Litre) |
| `coordinateX` | 112 | `double` | ETS2 dünya X koordinatı |
| `coordinateY` | 120 | `double` | ETS2 rakım (yükseklik) Y |
| `coordinateZ` | 128 | `double` | ETS2 dünya Z koordinatı |
| `rotationY` | 144 | `double` | Araç rotasyon açısı (Pusula yönü) |
| `blinkerLeft` | 250 | `uint8` | Sol sinyal lambası durumu |
| `blinkerRight` | 251 | `uint8` | Sağ sinyal lambası durumu |
| `lightsBeamLow`| 253 | `uint8` | Kısa far durumu |
| `lightsBeamHigh`| 254| `uint8` | Uzun far durumu |

---

## 4. Çökme Koruması ve Simülasyon Modu (Simulation Fallback)

`TelemetryService`, ETS2'nin kapalı olması, oyunun çökmesi veya telemetri eklentisinin yanıt vermemesi durumlarını otomatik olarak yönetir:

1. Oyun kapalıysa hiçbir Windows hatası vermez.
2. `SimulatedTelemetryService` devreye girerek gerçekçi Avrupa otoban fiziği (82-88 km/s hız, 1200-1450 RPM, vites geçişleri, sinyal döngüleri ve koordinat hareketleri) üretir.
3. ETS2 başlatıldığı anda sistem gecikmesiz olarak gerçek oyun verilerine geçiş yapar.
