<p align="center">
  <img src="assets/beast-core.png" alt="HyperDrive Beast Core" width="100%">
</p>

<p align="center"><strong>Türkçe</strong> · <a href="README_EN.md">English</a></p>

<p align="center">
  <img alt="Windows" src="https://img.shields.io/badge/platform-Windows-113643?logo=windows&logoColor=white">
  <img alt="CSharp" src="https://img.shields.io/badge/C%23-WinForms-ff4f41">
  <img alt="Test size" src="https://img.shields.io/badge/test-1_GiB-4dedd7">
  <img alt="Release" src="https://img.shields.io/badge/release-v0.1.0-ff4f41">
</p>

# HyperDrive · Beast Core

HyperDrive, Arda Çobanoğlu tarafından Codex desteğiyle geliştirilen, **Windows için dahili ve harici depolama hız testi uygulamasıdır**. Seçilen sürücüye 1 GiB geçici veri yazar, ardından aynı veriyi okuyup sonuçları ayrı ayrı **MB/sn** olarak gösterir.

CS2 Hyper Beast estetiğinden esinlenen yaratık görseli, nar çiçeği ve turkuaz vurgular, Bahnschrift yazı tipi ve tamamen yuvarlak uçlu düğmeler arayüzü oluşturur. Bu depo ilk yayımlanan **v0.1.0 geliştirme sürümünü** içerir.

## İndir ve çalıştır

**[Microsoft Store’dan ücretsiz indir](https://apps.microsoft.com/store/detail/9NCF9BDTM310?cid=DevShareMCLPCS)**

Microsoft Store sürümü yayımlandı. Store üzerinden kurulum ve güncellemeler kullanılabilir.

### Taşınabilir sürüm

1. [v0.1.0 sürümünü açın](https://github.com/ardacob/hyperdrive-beast-core/releases/tag/v0.1.0).
2. `HyperDrive-Windows-v0.1.0.zip` dosyasını indirip çıkarın.
3. `HyperDrive.exe` dosyasını çalıştırın; sürücüyü seçip **Testi başlat** düğmesine basın.

Kurulum gerekmez. Windows ve .NET Framework 4.x gerekir; güncel Windows 10/11 sistemlerinde mevcut çerçeve kullanılır. GitHub’daki taşınabilir EXE Authenticode ile imzalanmamıştır; Store sürümü için yukarıdaki bağlantıyı kullanın.

## Mevcut özellikler

- Dahili diskler, USB bellekler ve Windows'ta sürücü harfi bulunan harici diskler.
- Sabit **1 GiB (1.024 MiB)** sıralı yazma ve okuma testi.
- Ayrı yazma ve okuma sonuçları; **1 MB/sn = 1.000.000 bayt/sn**.
- Windows dosya önbelleğini atlayan erişim ve yazmalar için write-through.
- Testi durdurma, ilerleme göstergesi ve geçici dosyanın işlem sonunda temizlenmesi.
- C: kök dizini yerine aynı sürücüde yazılabilir bir klasör seçimi; normal C: testi için yönetici izni gerekmez.
- DPI desteği; hover, basılı ve devre dışı durumları özel çizilen yuvarlak düğmeler.

## Arayüz

<p align="center"><img src="docs/preview.png" alt="HyperDrive Windows arayüz önizlemesi" width="95%"></p>

Bu geliştirme önizlemesinde düğmelerin test sonundaki ilerleme durumuyla çizimi kontrol edilmiştir; gösterilen ekran bir performans sonucu değildir.

## Testin kapsamı

| Ölçülen | Bu sürümde bulunmayan |
| --- | --- |
| 1 GiB sıralı dosya yazma ve okuma | Rastgele I/O, IOPS ve gecikme |
| Dosya sistemi üzerinden MB/sn | Ham disk veya bölüm erişimi |
| Seçilen sürücüde geçici test dosyası | SMART, sağlık ve gerçek kapasite doğrulama |

En az **1 GiB + 64 MiB boş alan** gerekir. Mevcut dosyalar değiştirilmez. Donanım önbelleği, arka plandaki disk işlemleri, USB bağlantısı ve sıcaklık sonuçları etkiler. Arayüzdeki “1 GB” ve “1.024 MB” test boyutu etiketleri ikili birimlerde 1 GiB ve 1.024 MiB anlamındadır; hız sonucu ondalık MB/sn'dir.

## Kaynaktan derleme

```powershell
git clone https://github.com/ardacob/hyperdrive-beast-core.git
cd hyperdrive-beast-core
powershell -ExecutionPolicy Bypass -File .\scripts\build.ps1
```

Çıktı `dist/HyperDrive.exe` olur. Ek NuGet bağımlılığı yoktur; Windows'taki .NET Framework C# derleyicisi kullanılır. Tema görseli ve DPI manifesti EXE içine gömülür.

## Depo düzeni

```text
assets/                 Yaratık tema görseli
docs/                   Arayüz önizlemesi ve sorun giderme
scripts/build.ps1       Windows derleme betiği
windows/                C# kaynak ve DPI manifesti
CHANGELOG.md            Doğrulanmış değişiklikler
README_EN.md            İngilizce açıklama
```

## Gizlilik ve dosya temizliği

Test yerel olarak çalışır; uygulama dosya veya sonuçları bir sunucuya yüklemez. Benzersiz `.depolama-hiz-testi-*.tmp` dosyası kullanılır. Test bitince veya durdurulunca dosya silinir. Uygulama zorla kapatılırsa kalan dosyayı elle silmeniz gerekebilir; normal temizleme başarısız olursa uygulama dosyanın yolunu gösterir.

## Doğrulama ve sorun giderme

Geliştirme sırasında C: üzerinde 1 GiB yazma/okuma, dosya boyutu ve dosyanın silinmesi doğrulandı. Üç düğmenin hover, basılı ve devre dışı durumlarında köşe arka planları kontrol edildi. Her donanım ve ekran ölçeği kombinasyonu test edilmemiştir.

[Sorun giderme](docs/TROUBLESHOOTING.md) · [Değişiklik günlüğü](CHANGELOG.md) · [Sürümler](https://github.com/ardacob/hyperdrive-beast-core/releases)

## Tasarım kaynağı

Yaratık görseli AI ile üretilmiştir; resmi oyun görseli değildir. Tasarım CS2 Hyper Beast renk ve yaratık estetiğinden esinlenir. Uygulamanın Valve veya Counter-Strike ile resmi bağlantısı yoktur.
