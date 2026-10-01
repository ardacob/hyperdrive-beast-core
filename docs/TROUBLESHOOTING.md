# Sorun giderme

## C: testi erişim hatası veriyor

İlk prototip test dosyasını C: kök dizinine yazıyordu. v0.1.0, seçilen sürücüdeki geçici, kullanıcı veya uygulama klasörlerinden yazılabilir olanı kullanır. ZIP'teki güncel EXE'yi çalıştırın. Kurumsal korumalar tüm aday klasörlere yazmayı engelliyorsa uygulama bunu hata olarak gösterir.

## USB görünmüyor

Uygulama yalnızca Windows'ta hazır olan ve sürücü harfi bulunan sabit/çıkarılabilir sürücüleri listeler. USB'yi bağladıktan sonra **Yenile** düğmesine basın. Windows 0 bayt veya kullanılamaz bir sürücü gösteriyorsa önce disk/bölüm sorununu çözmek gerekir. HyperDrive diski biçimlendirmez.

## Yazma başarısız oluyor

Boş alanı, sürücü izinlerini ve fiziksel yazma korumasını kontrol edin. En az 1 GiB + 64 MiB boş alan gerekir. Testi sürdürmek için mevcut dosyalarınızı silmeyin; uygun boş alan bulunan sürücüyü seçin.

## Sonuçlar beklenenden yüksek/düşük

Diğer disk işlemlerini kapatıp tekrar deneyin. Test Windows dosya önbelleğini atlar, ancak donanım önbelleğini tamamen devre dışı bırakmaz. 1 GiB test kısa süreli sıralı aktarımı ölçer; uzun süreli sürdürülebilir hızın kanıtı değildir.

## Geçici dosya kaldı

Normal bitişte ve durdurmada dosya silinir. Uygulamayı zorla kapatmak veya sürücüyü test sırasında çıkarmak dosya bırakabilir. Dosya adı `.depolama-hiz-testi-` ile başlayan, `.tmp` uzantılı benzersiz bir addır. Uygulama kapalıyken yalnızca bu teste ait dosyayı silebilirsiniz. Temizleme hatasında uygulama tam dosya yolunu gösterir.

## Font/ekran ölçeği

Bahnschrift Windows'ta kullanılır; bulunmazsa sistem yedek font seçebilir. DPI manifesti sistem ölçeğini kullanır. Başka ölçekli ekrana geçince uygulamayı o ekranda yeniden açın.
