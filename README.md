# ⚓ TimeAnchor (Zaman Çıpası)

TimeAnchor, zamanı sadece takip etmekle kalmayan, ona "çapa atan" modern, esnek ve arka planda sessizce çalışan bir kişisel asistan ve hatırlatıcı (Reminder/Habit Tracker) uygulamasıdır. 

Windows Forms veya eski nesil arayüzlerin hantallığından uzak; tamamen modern Material Design prensipleriyle tasarlanmış, kullanıcı deneyimini (UX) ön planda tutan bir mimariye sahiptir.

**Geliştirici:** HarunSinevazyon

## 🚀 Projenin Amacı ve Vizyonu
Geleneksel "To-Do" (Yapılacaklar) uygulamaları genellikle görev bitirildiğinde onu yok eder. Ancak alışkanlıklar ve periyodik görevler süreklilik ister. TimeAnchor'ın temel felsefesi:
* **Silme, Arşivle:** Spor salonuna bir hafta gitmedin diye görev silinmez, "Arşiv" sekmesinde pasif olarak seni bekler.
* **Geleceğe Çapa At:** Periyodik bir görevi (örn. her Pazartesi çiçekleri sula) tamamladığında, sistem geçmişe "Başarıyla Tamamlandı" rozeti bırakırken, arka planda asıl görevi bir sonraki haftaya otomatik olarak kopyalar ve zamanlar.
* **Seni Asla Unutmaz:** Uygulamayı kapatsan bile, System Tray (Görev Çubuğu) üzerinde "Hayalet Modda" çalışmaya devam eder ve zamanı geldiğinde şık bir pop-up ile sana kendini hatırlatır.

## 🛠️ Kullanılan Teknolojiler (Tech Stack)
* **Dil:** C# (.NET 8.0)
* **Arayüz (UI):** WPF (Windows Presentation Foundation)
* **Tasarım Sistemi:** MaterialDesignInXAML
* **Veritabanı:** SQLite (Local, hafif ve hızlı veri yönetimi)

## ✨ Mevcut Özellikler (Current State)
* **Responsive (Esnek) Tasarım:** Pencere boyutu değiştiğinde veya tam ekran yapıldığında içindeki kartlar ekranı mükemmel bir şekilde dolduracak şekilde esner.
* **Pencere Hafızası:** Uygulama, kapatıldığı andaki ekran koordinatlarını ve boyutlarını hatırlar; bir sonraki açılışta tam olarak bıraktığınız yerde ve boyutta başlar.
* **Canlı Saat ve Takvim:** Ana ekranda donanımı yormayan, saniyesi saniyesine akan canlı bir saat ve şık bir açılır Material Takvim bulunur.
* **Çift Motorlu Mimari:** Arayüzdeki canlı saat ile arka planda veritabanını tarayan alarm motoru (DispatcherTimer) birbirini engellemeden asenkron çalışır.
* **System Tray (Hayalet Mod):** Pencere kapatıldığında uygulama kapanmaz; görev çubuğuna şık ikonumuzla küçülür ve kalp atışını sürdürür.
* **Özel Alarm Pop-up'ı:** Sıkıcı Windows uyarıları yerine; sürüklenebilen, köşe yuvarlatmalı, "Tamamla / 15 Dk Ertele / Yoksay" seçenekleri sunan modern bir alarm ekranı.

## 🚧 Gelecek Planları (Roadmap)
- [ ] **Gelişmiş Analitik ve İstatistikler:** Hangi görevlerin yüzde kaç oranında tamamlandığını gösteren bir "Dashboard" (Kontrol Paneli) sayfası.
- [ ] **Özel Ses Entegrasyonu:** Alarm çaldığında Windows bildirim sesi yerine, kullanıcının seçebileceği özel ses dosyalarının (.wav/.mp3) çalınması.
- [ ] **Kategori ve Renk Etiketleri:** Görevleri "İş", "Okul", "Kişisel" gibi etiketlere bölme ve bunlara göre filtreleyebilme özelliği.
- [ ] **Veritabanı Dışa Aktarma (Export/Import):** Kullanıcıların verilerini JSON veya CSV formatında yedekleyebilmesi.
- [ ] **Eşzamanlı Cihaz Entegrasyonu:** Kullanıcıların farklı cihazlarda aynı oturumlarla giriş yapıp kullanabilecek bir hesap oluşturabilmesi.
- [ ] **Mobil Aplikasyon:** Projeyi bir adım daha öteye taşıyıp mobil bir Android/IOS uygulaması geliştirmek.

---
*Zamanı yakalayamayabilirsin, ama ona çapa atabilirsin.* ⚓
