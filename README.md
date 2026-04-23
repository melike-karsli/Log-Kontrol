🚀 Log Analysis & Order Tracking System
Bu proje, karmaşık log yapılarını ve API'den gelen sipariş verilerini anlamlı verilere dönüştüren, yüksek performanslı bir izleme arayüzüdür.

📝 Proje Hakkında
Uygulama, operasyonel süreçlerdeki veri kirliliğini minimize etmek için iki temel işlev sunar:

Sipariş Yönetimi: JSON formatındaki ham sipariş verilerini ayrıştırarak (parsing) ilişkisel tablolara dönüştürür.

Hata Analiz Motoru: Karmaşık log dosyalarını anlık olarak tarar; sunucu hataları, timeout ve bağlantı kesintileri gibi kritik logları ayırt ederek analiz için görselleştirir.

✨ Temel Özellikler
Dinamik JSON Ayrıştırma: API bazlı verileri Newtonsoft.Json veya System.Text.Json ile yüksek hızda parse eder.

Kritik Hata Filtreleme: Loglar içindeki gürültüyü temizler ve sadece aksiyon alınması gereken hataları listeler.

Optimize Edilmiş Arayüz: DataGridView entegrasyonu sayesinde binlerce satırlık veride bile kasmadan akıcı listeleme ve filtreleme sağlar.

Görsel Uyarı Sistemi: Kritik hatalar için renk kodlu (Kırmızı/Yeşil) durum göstergeleri (Opsiyonel).

🛠 Teknolojiler ve Gereksinimler
Dil & Framework: C# / .NET

IDE: Visual Studio 2022+

Kütüphaneler: Newtonsoft.Json (JSON işleme için)

Veri Kaynağı: Yerel .txt log dosyaları veya API yanıtları.

📦 Kurulum ve Çalıştırma
Repoyu Klonlayın:

Bash
git clone https://github.com/melike-karsli/Ent-tyProjeUygulama.git
Bağımlılıkları Yükleyin: Visual Studio ile açtığınızda NuGet paketleri otomatik olarak yüklenecektir.

Yapılandırma: App.config veya ilgili path değişkeninden log dosyanızın yolunu belirtin.

Çalıştır: F5 ile projeyi derleyip başlatın.
