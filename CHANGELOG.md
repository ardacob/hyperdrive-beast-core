# Değişiklik günlüğü

## v0.1.0 — 2026-10-01

İlk yayımlanan Windows geliştirme sürümü. Yerel geliştirme sırasında hazırlanan v1–v5 dosyaları bu ilk yayın altında birleştirildi; ayrı GitHub sürümleri değildir.

- 1 GiB sıralı yazma/okuma ve ayrı MB/sn sonuçları.
- Geçici test dosyası, durdurma ve temizleme.
- Windows dosya önbelleğini atlayan erişim.
- Beast Core yaratık teması, nar çiçeği/turkuaz vurgular ve yuvarlak düğmeler.
- Bahnschrift font; başlık yüksekliği düzeltilerek kesilmesi giderildi.
- C: testinin korumalı kök dizin yerine aynı sürücüde yazılabilir klasör kullanması.
- Düğme köşelerinde arka plan parçalarının görünmesi giderildi.
- Yerel Button çizimi yerine özel Control kullanılarak hover/basılı/devre dışı çizimleri birleştirildi.

Doğrulama: Windows'ta derleme; 1 GiB C: testi, test dosyası boyutu ve temizliği; üç düğmenin farklı durumlarda köşe kontrolü.

Kapsam: sıralı dosya aktarımı. Rastgele I/O, SMART ve kapasite doğrulaması mevcut değil.
