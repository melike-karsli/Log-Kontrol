using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Newtonsoft.Json kütüphanesi ile JSON verileri işlenebilir.



namespace RestoPOSSiparisTakip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            BasliklariRenklendir();
            KopyalamayiAyarla();
            UstKontrolleriEsitle();
            DuzeniKur();
            EskiLoglariSil();
            DosyalariYukle();
            tarihsec.ValueChanged += (s, e) => DosyalariYukle();
        }

        // Tarih, kaynak ve LOG YAZDIR aynı en ve boyda dursun.
        // Tarih seçicinin yüksekliği fonttan gelir; ComboBox yüksekliği ancak kendi çizimiyle (OwnerDrawFixed) ayarlanabildiği için
        // liste "sadece seçim" moduna alınıp öğeleri elle çiziliyor.
        private void UstKontrolleriEsitle()
        {
            int yukseklik = tarihsec.Height;

            kaynak_combo.DropDownStyle = ComboBoxStyle.DropDownList;
            kaynak_combo.DrawMode = DrawMode.OwnerDrawFixed;
            kaynak_combo.ItemHeight = yukseklik - 6;
            kaynak_combo.ItemHeight += yukseklik - kaynak_combo.Height;   // kenarlık payı sisteme göre değişebiliyor
            kaynak_combo.DrawItem += (s, e) =>
            {
                e.DrawBackground();
                if (e.Index >= 0)
                    TextRenderer.DrawText(e.Graphics, kaynak_combo.Items[e.Index].ToString(), e.Font, e.Bounds, e.ForeColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                else if (kaynak_combo.Items.Count == 0)   // seçilen günde hiç log dosyası yok
                    TextRenderer.DrawText(e.Graphics, "Bu tarihte log yok", e.Font, e.Bounds, SystemColors.GrayText,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                e.DrawFocusRectangle();
            };

            button1.Height = yukseklik;
            kaynak_combo.Width = button1.Width = tarihsec.Width;
            kaynak_combo.Top = button1.Top = tarihsec.Top;
        }

        // Tablolar satır seçse de (özette satıra tıklayınca detay açılıyor) tek hücre kopyalanabilsin:
        //   tek satır seçiliyken Ctrl+C → sadece tıklanan hücre (ör. Sipariş No); birden çok satır seçiliyken → satırların tamamı
        //   sağ tık → "Hücreyi kopyala" / "Satırı kopyala"
        private void KopyalamayiAyarla()
        {
            foreach (var tablo in Controls.OfType<DataGridView>())
            {
                tablo.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

                tablo.KeyDown += (s, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.C && tablo.CurrentCell != null && tablo.SelectedRows.Count <= 1)
                    {
                        HucreyiKopyala(tablo);
                        e.Handled = true;
                    }
                };

                var menu = new ContextMenuStrip();
                menu.Items.Add("Hücreyi kopyala", null, (s, e) => HucreyiKopyala(tablo));
                menu.Items.Add("Satırı kopyala", null, (s, e) => SatiriKopyala(tablo));
                tablo.ContextMenuStrip = menu;

                // Sağ tıklanan hücre seçilsin ki menü o hücreyi kopyalasın
                tablo.CellMouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                        tablo.CurrentCell = tablo.Rows[e.RowIndex].Cells[e.ColumnIndex];
                };
            }
        }

        private static void HucreyiKopyala(DataGridView tablo)
        {
            string deger = tablo.CurrentCell?.FormattedValue?.ToString();
            if (!string.IsNullOrEmpty(deger))
                Clipboard.SetText(deger);
        }

        private static void SatiriKopyala(DataGridView tablo)
        {
            var satir = tablo.CurrentRow;
            if (satir == null) return;

            string metin = string.Join("\t", satir.Cells.Cast<DataGridViewCell>()
                .Where(c => c.Visible)
                .OrderBy(c => c.OwningColumn.DisplayIndex)
                .Select(c => c.FormattedValue?.ToString() ?? ""));
            if (metin.Trim() != "")
                Clipboard.SetText(metin);
        }

        // Tüm tabloların başlık satırı açık mavi, kalın ve tablo yazısından 1,5 punto büyük; seçili kolonda da aynı renk kalır (mavi olmaz)
        private void BasliklariRenklendir()
        {
            var acikMavi = Color.FromArgb(221, 235, 247);

            foreach (var tablo in Controls.OfType<DataGridView>())
            {
                tablo.EnableHeadersVisualStyles = false;   // kapalı değilse Windows teması rengi ezer
                tablo.ColumnHeadersDefaultCellStyle.BackColor = acikMavi;
                tablo.ColumnHeadersDefaultCellStyle.SelectionBackColor = acikMavi;
                tablo.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                tablo.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
                tablo.ColumnHeadersDefaultCellStyle.Font = new Font(tablo.Font.FontFamily, tablo.Font.Size + 1.5f, FontStyle.Bold);
                tablo.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;   // "Satış kanalı" iki satıra bölünmesin
            }
        }


        // Log dosyalarının arandığı klasörler: RestoPOS kurulumunda C:/D:\RestoPOS\MY\LOG, bazı müşterilerde C:/D:\RestoSEPET\MY\LOG
        private static readonly string[] LogKlasorleri =
        {
            @"C:\RestoPOS\MY\LOG\",
            @"D:\RestoPOS\MY\LOG\",
            @"C:\RestoSEPET\MY\LOG\",
            @"D:\RestoSEPET\MY\LOG\"
        };

        private const string LogKlasoruYokMesaji = "LOG klasörü bulunamadı.\n\nAranan klasörler:\n" +
            @"C:\RestoPOS\MY\LOG" + "\n" + @"D:\RestoPOS\MY\LOG" + "\n" + @"C:\RestoSEPET\MY\LOG" + "\n" + @"D:\RestoSEPET\MY\LOG";

        // Var olan ilk LOG klasörü (yoksa null). İçinde .txt olan klasör öncelikli: boş bir RestoPOS\MY\LOG,
        // RestoSEPET\MY\LOG içindeki asıl logların önüne geçmesin.
        private static string LogKlasoru()
        {
            var mevcutKlasorler = LogKlasorleri.Where(Directory.Exists).ToList();
            return mevcutKlasorler.FirstOrDefault(k => Directory.EnumerateFiles(k, "*.txt").Any()) ?? mevcutKlasorler.FirstOrDefault();
        }

        // Log dosyasını, logu yazan programı (RestoSepet, RCGuard, ...) engellemeden okur.
        // File.ReadAllLines dosyayı okurken başkasının yazmasını yasakladığı için RestoSepet "Cannot open file ... başka bir işlem
        // tarafından kullanıldığından" hatası veriyordu. Burada yazmaya izin verilerek açılır, tüm içerik tek seferde belleğe alınıp
        // dosya hemen kapatılır; satırlara ayırma dosya kapandıktan sonra yapılır.
        private static string[] LogDosyasiniOku(string dosyaYolu)
        {
            byte[] icerik = null;

            // Yazan program dosyayı o an kilitli tutuyorsa birkaç kez kısa aralıklarla tekrar denenir
            for (int deneme = 1; icerik == null; deneme++)
            {
                try
                {
                    using (var dosya = new FileStream(dosyaYolu, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (var bellek = new MemoryStream())
                    {
                        dosya.CopyTo(bellek);
                        icerik = bellek.ToArray();
                    }
                }
                catch (IOException) when (deneme < 5)
                {
                    System.Threading.Thread.Sleep(200);
                }
            }

            var satirlar = new List<string>();
            using (var okuyucu = new StringReader(Encoding.GetEncoding("windows-1254").GetString(icerik)))
            {
                string satir;
                while ((satir = okuyucu.ReadLine()) != null)
                    satirlar.Add(satir);
            }
            return satirlar.ToArray();
        }

        // Program her açılışta log klasörlerindeki 5 aydan eski .txt dosyalarını kalıcı olarak siler.
        // Dosyanın tarihi adındaki _yyyyMMdd kısmından alınır (RCGuard_20251031.txt → 31.10.2025); Windows'un değiştirilme
        // tarihi kopyalama/yedeklemede değişebildiği için kullanılmaz. Adında tarih olmayan dosyaya dokunulmaz.
        private static void EskiLoglariSil()
        {
            DateTime sinir = DateTime.Today.AddMonths(-5);

            foreach (string klasor in LogKlasorleri)
            {
                if (!Directory.Exists(klasor)) continue;

                foreach (string dosya in Directory.GetFiles(klasor, "*.txt", SearchOption.TopDirectoryOnly))
                {
                    var tarihParcasi = Regex.Match(Path.GetFileName(dosya), @"_(\d{8})");
                    if (!tarihParcasi.Success) continue;

                    if (!DateTime.TryParseExact(tarihParcasi.Groups[1].Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dosyaTarihi))
                        continue;

                    if (dosyaTarihi >= sinir) continue;

                    try
                    {
                        File.Delete(dosya);
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        // Kullanımda ya da yetki yok: bir sonraki açılışta tekrar denenir
                    }
                }
            }
        }

        // Kaynak listesinde müşteriye anlaşılır ad gösterilir, log dosyası ise dosya adının başındaki kısımla bulunur
        // ("KURYE SIPARISLERI" seçilince my_kurye_20260930.txt okunur)
        private class KaynakSecenegi
        {
            private static readonly Dictionary<string, string> GorunenAdlar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "my_kurye", "KURYE SIPARISLERI" },
                { "RCGuard", "WEB-MOBIL SIPARISLERI" },
                { "RestoSepet", "PAKET SIPARISLERI" },
                { "TsmLclSrv", "OKC BILGILERI" },
            };

            public string Dosya { get; }
            public string Ad { get; }

            public KaynakSecenegi(string dosya)
            {
                Dosya = dosya;
                Ad = GorunenAdlar.TryGetValue(dosya, out var ad) ? ad : dosya;   // tanımsız kaynak dosya adıyla görünür
            }

            public override string ToString() => Ad;
        }

        private string SeciliKaynakDosyasi()
        {
            return (kaynak_combo.SelectedItem as KaynakSecenegi)?.Dosya;
        }

        // Bu adla başlayan log dosyaları kaynak listesinde gösterilmez
        private static readonly string[] GizlenenKaynaklar = { "DataKontrol", "RSKontrol" };

        // Kaynak listesi seçilen tarihe göre doldurulur: sadece o gün için dosyası olan kaynaklar görünür
        // (LOG YAZDIR "<kaynak>_<yyyyMMdd>.txt" dosyasını açtığı için yalnızca bu tam adla eşleşen dosyalar sayılır).
        // Tarih değişince liste yenilenir; önceki seçim yeni günde de varsa seçili kalır.
        private void DosyalariYukle()
        {
            string oncekiSecim = SeciliKaynakDosyasi();
            string gun = tarihsec.Value.ToString("yyyyMMdd");

            kaynak_combo.BeginUpdate();
            kaynak_combo.Items.Clear();

            string klasorYolu = LogKlasoru();

            if (klasorYolu != null)
            {
                foreach (string dosyaYolu in Directory.GetFiles(klasorYolu, $"*_{gun}.txt").OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
                {
                    // "my_kurye_20260928.txt" → "my_kurye"
                    var eslesme = Regex.Match(Path.GetFileName(dosyaYolu), $@"^(.+)_{gun}\.txt$", RegexOptions.IgnoreCase);
                    if (!eslesme.Success) continue;

                    string kaynakAdi = eslesme.Groups[1].Value;

                    // DataKontrolSrvc ve RSKontrol logları sipariş/fiş içermediği için listelenmez
                    if (GizlenenKaynaklar.Any(g => kaynakAdi.StartsWith(g, StringComparison.OrdinalIgnoreCase))) continue;

                    if (!kaynak_combo.Items.Cast<KaynakSecenegi>().Any(k => k.Dosya.Equals(kaynakAdi, StringComparison.OrdinalIgnoreCase)))
                        kaynak_combo.Items.Add(new KaynakSecenegi(kaynakAdi));
                }
            }

            // Açılır liste en uzun kaynak adını kesmeden gösterecek kadar geniş olsun
            foreach (object item in kaynak_combo.Items)
            {
                int genislik = TextRenderer.MeasureText(item.ToString(), kaynak_combo.Font).Width + SystemInformation.VerticalScrollBarWidth;
                kaynak_combo.DropDownWidth = Math.Max(kaynak_combo.DropDownWidth, genislik);
            }

            var korunan = kaynak_combo.Items.Cast<KaynakSecenegi>().FirstOrDefault(k => k.Dosya.Equals(oncekiSecim, StringComparison.OrdinalIgnoreCase));
            if (korunan != null)
                kaynak_combo.SelectedItem = korunan;

            kaynak_combo.EndUpdate();
            kaynak_combo.Invalidate();   // boş listede "Bu tarihte log yok" yazısı çizilsin

            button1.Enabled = kaynak_combo.Items.Count > 0;
        }

        string[] satirlar;

        // Listede sadece seçilen gün için dosyası olan kaynaklar bulunduğundan (bkz. DosyalariYukle) seçimde ayrıca dosya kontrolü gerekmiyor
        private void kaynak_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }


        public string FncIngenicoOdemeTip(string tip)
        {
            if (tip == "1")
                return "NAKIT";
            else if (tip == "4")
                return "BANKAKARTI";
            else if (tip == "8")
                return "YC_YEMEKCEKI";
            else if (tip == "16")
                return "MOBIL";
            else if (tip == "32")
                return "HEDIYE_CEKI";
            else if (tip == "512")
                return "PUAN";
            else if (tip == "2048")
                return "BANKA_TRANSFERI";
            else if (tip == "16384")
                return "DIGER";
            else if (tip == "4503599627370496")
                return "TR_KAREKOD_CARD";
            else if (tip == "18014398509481984")
                return "TR_KAREKOD_MOBIL";
            else if (tip == "36028797018963968")
                return "TR_KAREKOD_DIGER";
            else if (tip == "9007199254740992")
                return "TR_KAREKOD_FAST";
            else
                return ""; // veya "BILINMEYEN TIP"
        }

        public string fnc_IngenicoBankaKod(string tip)
        {
            if (tip == "0")
                return "YC_YEMEKCEKI";
            else if (tip == "46")
                return "AKBANK";
            else if (tip == "64")
                return "ISBANK";
            else if (tip == "62")
                return "GARANTI";
            else if (tip == "67")
                return "YAPIKREDI";
            else if (tip == "12")
                return "HALKBANK";
            else if (tip == "32")
                return "TEB";
            else if (tip == "10")
                return "ZIRAATBANK";
            else if (tip == "134")
                return "DENIZBANK";
            else if (tip == "111")
                return "FINANSBANK";
            else if (tip == "59")
                return "SEKERBANK";
            else if (tip == "15")
                return "VAKIFBANK";
            else if (tip == "124")
                return "ALTERNATIFBANK"; // Not: AKBANK ile çakışıyordu, sonuncusu kaldı
            else if (tip == "143")
                return "AKTIFBANK";
            else if (tip == "203")
                return "ALBARAKA";
            else if (tip == "92")
                return "CITIBANK";
            else if (tip == "71")
                return "FORTISBANK";
            else if (tip == "123")
                return "HSBCBANK";
            else if (tip == "9009")
                return "SEKERSIRKETBANK";
            else if (tip == "206")
                return "TURKIYEFINANSBANK";
            else if (tip == "205")
                return "KUVEYTTURK";
            else if (tip == "17")
                return "TKALKINMABANK";
            else if (tip == "135")
                return "ANADOLUBANK";
            else if (tip == "22970")
                return "YC_TICKET";
            else if (tip == "-12917")
                return "YC_YEMEKCEKI";
            else if (tip == "-12902")
                return "YC_METROPOL";
            else if (tip == "-12923")
                return "YC_MULTINET";
            else if (tip == "22973")
                return "YC_SODEXO";
            else if (tip == "-12882")
                return "YC_PAYE";
            else if (tip == "-12887")
                return "YC_SETCARD";
            else if (tip == "-12875")
                return "PAYCELLQR";
            else if (tip == "8083")
                return "ODEAL";
            else if (tip == "52657")
                return "TELIUM";
            else
                return "ODEME";
        }

        string fnc_IngenicoOdemeTip(string paymentTypeEx)  // PaymentTypeEx'e göre ödeme açıklaması döndürür.
        {
            if (paymentTypeEx == "1")
                return "Nakit";
            else if (paymentTypeEx == "512")
                return "Puan";
            else if (paymentTypeEx == "2048")
                return "Banka Transferi";
            else
                return "Diğer";
        }


        //TSM (Ingenico)
        // "Ingenico Gelen Veri :{...Receipt...}" satırı kesilen fişi taşır; Receipt'siz olanlar durum sorgusu olduğu için atlanır.
        // SİPARİŞ ÖZET'te her fiş tek satır, SİPARİŞ DETAY'da fişin ödemeleri gösterilir.
        private Dictionary<string, poscihazi.IngenicoData> tsmFisleri = new Dictionary<string, poscihazi.IngenicoData>();

        private void TsmTabloHazirla()
        {
            if (datagridrcguard.Columns.Count != 0) return;

            datagridrcguard.Columns.Add("saat", "Saat");
            datagridrcguard.Columns.Add("adisyonNo", "Adisyon No");
            datagridrcguard.Columns.Add("zNo", "Z No");
            datagridrcguard.Columns.Add("fisNo", "Fiş No");
            datagridrcguard.Columns.Add("ekuNo", "EKU No");
            datagridrcguard.Columns.Add("seriNo", "Seri No");
            datagridrcguard.Columns.Add("tutar", "Tutar");
            datagridrcguard.Columns.Add("odeme", "Ödeme");

            // Seçilen satırın detayını bulmak için anahtar (seri no + Z no + fiş no), kullanıcıya gösterilmez
            datagridrcguard.Columns.Add("id", "Anahtar");
            datagridrcguard.Columns["id"].Visible = false;
        }

        // Ingenico tutarları kuruş cinsinden gönderiyor (106500 = 1065,00 TL)
        private static string KurusTutar(double kurus)
        {
            return (kurus / 100).ToString("0.##");
        }

        private string IngenicoOdemeTipi(poscihazi.Payment odeme)
        {
            string tip = FncIngenicoOdemeTip(odeme?.PaymentTypeEx?.ToString() ?? "");
            return tip == "" ? "DIGER" : tip;
        }

        // BankBKMID 0 ise (nakit, banka transferi) banka yok
        private string IngenicoBanka(poscihazi.Payment odeme)
        {
            return (odeme?.BankBKMID ?? 0) == 0 ? "" : fnc_IngenicoBankaKod(odeme.BankBKMID.ToString());
        }

        // Özet tablo için: "BANKAKARTI (ISBANK)", "NAKIT" gibi
        private string IngenicoOdemeAdi(poscihazi.Payment odeme)
        {
            string banka = IngenicoBanka(odeme);
            return banka == "" ? IngenicoOdemeTipi(odeme) : $"{IngenicoOdemeTipi(odeme)} ({banka})";
        }


        private void LogTSM(string json, string saat)
        {
            if (!json.Contains("\"Receipt\"")) return;

            poscihazi.IngenicoData veri;
            try
            {
                veri = JsonConvert.DeserializeObject<poscihazi.IngenicoData>(json);
            }
            catch (JsonException)
            {
                return;
            }

            var fis = veri?.Receipt;
            if (fis == null) return;

            TsmTabloHazirla();

            string seriNo = (veri.SerialNo ?? "").Trim();
            string anahtar = $"{seriNo}_{fis.ZNo}_{fis.ReceiptNo}";

            // Aynı fiş logda birden fazla kez gelebilir
            if (tsmFisleri.ContainsKey(anahtar)) return;
            tsmFisleri[anahtar] = veri;

            var odemeler = fis.PaymentList ?? new List<poscihazi.Payment>();

            datagridrcguard.Rows.Add(
                saat ?? "",
                veri.AdisyonNo,
                fis.ZNo,
                fis.ReceiptNo,
                fis.EkuNo,
                seriNo,
                KurusTutar(fis.PaidAmount),
                string.Join(", ", odemeler.Select(IngenicoOdemeAdi).Distinct()),
                anahtar
            );
        }

        private void TsmDetayGoster(poscihazi.IngenicoData veri)
        {
            datagridrcguarddetay.Rows.Clear();
            datagridrcguarddetay.Columns.Clear();
            datagridrcguarddetay.Columns.Add("odemeTipi", "Ödeme Tipi");
            datagridrcguarddetay.Columns.Add("banka", "Banka");
            datagridrcguarddetay.Columns.Add("tutar", "Tutar");
            datagridrcguarddetay.Columns.Add("odemeZamani", "Ödeme Zamanı");

            // Merchantid / Referencenumber / Authorizationcode logda değer değil {"ValueKind":3} olarak geldiği için gösterilmez
            foreach (var odeme in veri.Receipt?.PaymentList ?? new List<poscihazi.Payment>())
            {
                string odemeZamani = DateTime.TryParse(odeme?.PaymentDateTime, out var zaman) ? zaman.ToString("dd.MM.yyyy HH:mm:ss") : odeme?.PaymentDateTime ?? "";

                datagridrcguarddetay.Rows.Add(
                    IngenicoOdemeTipi(odeme),
                    IngenicoBanka(odeme),
                    KurusTutar(odeme?.PaymentAmount ?? 0),
                    odemeZamani
                );
            }
        }

        // SİPARİŞ ÖZET / SİPARİŞ DETAY tabloları birden çok platform (Trendyol, Yemeksepeti, ...)
        // arasında paylaşılıyor: kanal adı ayrı bir sütunda, sipariş no ise platformlar arası
        // benzersiz olacak şekilde tutuluyor.
        private HashSet<string> islenenUrunSatirlari = new HashSet<string>();

        private class SiparisUrun
        {
            public int Adet = 1;
            public string Name;
            public string ModifierNamesStr;
            public string BirimFiyat;
            public string Not;
        }

        private class SiparisDetay
        {
            public string CallCenterPhone;
            public string PaymentType;
            public string TotalIndirim;
            public string Not;
            public string Adres;
            public List<SiparisUrun> Urunler = new List<SiparisUrun>();
        }

        private Dictionary<string, SiparisDetay> siparisDetaylari = new Dictionary<string, SiparisDetay>();

        // İletişim, ödeme tipi ve adres özet tabloya sığmadığı için SİPARİŞ DETAY tablosunda gösterilir.
        // Kurye loglarında kurye firması ayrı kolonda gösterilir, indirim bilgisi olmadığı için İndirim kolonu açılmaz
        private void OzetTabloHazirla(bool kuryeLogu = false)
        {
            if (datagridrcguard.Columns.Count != 0) return;

            datagridrcguard.Rows.Clear();
            datagridrcguard.Columns.Clear();
            datagridrcguard.Columns.Add("saat", "Saat");
            if (kuryeLogu)
                datagridrcguard.Columns.Add("kurye", "Kurye Firması");
            datagridrcguard.Columns.Add("kanal", "Satış kanalı");
            datagridrcguard.Columns.Add("id", "Sipariş No");
            datagridrcguard.Columns.Add("musteri", "Müşteri");
            datagridrcguard.Columns.Add("totalPrice", "Toplam Tutar");
            if (!kuryeLogu)
                datagridrcguard.Columns.Add("indirim", "İndirim");
            datagridrcguard.Columns.Add("not", "Not");

            // Not kolonu tek satırda kalır, uzun notlar yatay kaydırma çubuğuyla okunur (bkz. KolonuBoslugaYay)
            // (Fill modunda kolon tabloya sığmak için daraldığından yatay kaydırma çubuğu hiç çıkmıyordu)
            var notKolonu = datagridrcguard.Columns["not"];
            notKolonu.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            notKolonu.MinimumWidth = 300;
            notKolonu.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        // Sadece "İstemiyorum" yazan seçenekler bilgi taşımıyor; "Promosyon Baget Et Döner İstemiyorum" gibiler gösterilmeye devam eder.
        private static bool SadeceIstemiyorum(string secenek)
        {
            return string.Compare((secenek ?? "").Trim(), "İstemiyorum", new CultureInfo("tr-TR"), CompareOptions.IgnoreCase) == 0;
        }

        private static string SecenekleriBirlestir(IEnumerable<string> secenekler)
        {
            return string.Join(", ", secenekler.Where(n => !string.IsNullOrWhiteSpace(n) && !SadeceIstemiyorum(n)));
        }

        // Özet tablodaki Not: sipariş notu + ürünlere yazılmış notlar (hangi ürüne ait olduğuyla birlikte)
        private static string NotMetni(SiparisDetay detay)
        {
            var notlar = new List<string>();

            string siparisNotu = StandartNotuTemizle(detay.Not);
            if (siparisNotu != "")
                notlar.Add(siparisNotu);

            notlar.AddRange(detay.Urunler
                .Select(u => new { u.Name, Not = StandartNotuTemizle(u.Not) })
                .Where(u => u.Not != "")
                .Select(u => $"{u.Name}: {u.Not}"));

            return string.Join(" | ", notlar);
        }

        // Yemeksepeti her siparişe "- ** ÇATAL-BIÇAK GÖNDERMEYİN <ödeme tipi>" ya da "+ ** LÜTFEN ÇATAL-BIÇAK GÖNDERİN <ödeme tipi>"
        // ekliyor; standart yazı olduğu ve ödeme tipi detayda ayrıca gösterildiği için nottan çıkarılır.
        private static readonly Regex StandartCatalBicakRegex = new Regex(
            @"[-+]?\s*\*\*\s*(?:L[ÜU]TFEN\s+)?[ÇC]ATAL-B[Iİ][ÇC]AK\s+G[ÖO]NDER(?:MEY)?[İI]N.*?(?=\s*[-+]\s*\*\*|\s*[|;]|$)", RegexOptions.IgnoreCase);

        private static string StandartNotuTemizle(string not)
        {
            if (string.IsNullOrWhiteSpace(not)) return "";

            string temiz = StandartCatalBicakRegex.Replace(not, "");

            // "1 | gelince ara; " gibi sonda/başta kalan ayraçlar ve boş parçalar atılır
            var parcalar = temiz.Split(new[] { '|', ';' })
                .Select(p => p.Trim(' ', '-'))
                .Where(p => p != "");
            return string.Join(" | ", parcalar);
        }

        // Log satırı " : 29.09.2026 00:13:30" (RCGuard'da "27.9.2026 00:14:50" ya da "31/10/2025 00:10:44") ile biter; özet tabloya sadece saat yazılır.
        private static readonly Regex LogZamaniRegex = new Regex(@":\s*(\d{1,2}[./]\d{1,2}[./]\d{4} \d{1,2}:\d{2}:\d{2})\s*$");

        private static string LogSaati(string satir)
        {
            var m = LogZamaniRegex.Match(satir ?? "");
            if (!m.Success) return null;

            return DateTime.TryParseExact(m.Groups[1].Value.Replace('/', '.'), "d.M.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var zaman)
                ? zaman.ToString("HH:mm:ss")
                : null;
        }

        private static string FiyatMetni(decimal fiyat)
        {
            return fiyat.ToString("0.##");
        }

        private static string AdresMetni(params string[] parcalar)
        {
            return string.Join(", ", parcalar.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()));
        }

        private static string Onekli(string onek, string deger)
        {
            return string.IsNullOrWhiteSpace(deger) ? null : $"{onek} {deger.Trim()}";
        }

        //TRENDYOL
        private void RestoSepetTrendyol(string json, string saat)
        {
            // SİPARİŞ ÖZET: her sipariş için tek satır (saat, kanal, sipariş no, müşteri, tutar, not).
            // Ürün/indirim/iletişim gibi ayrıntılar satır tıklanınca SİPARİŞ DETAY tablosunda gösterilir.
            OzetTabloHazirla();

            var veri1 = JsonConvert.DeserializeObject<trendyol.trendyollog>(json); //json isimli stringi, sizin tanımladığınız trendyollog class’ına çeviriyor.

            if (veri1?.content == null) return; //Eğer veri1 boşsa veya content listesi yoksa, fonksiyon duruyor.


            foreach (var contentItem in veri1.content) //JSON içindeki content listesi üzerinde tek tek dönüyor. //contentItem oluşturduk listeyi buraya atadık
            {
                if (string.IsNullOrEmpty(contentItem?.id)) continue;

                // Sipariş detayını (ürünler, iletişim, ödeme tipi) sözlükte biriktir.
                if (!siparisDetaylari.TryGetValue(contentItem.id, out var detay))
                {
                    detay = new SiparisDetay();
                    siparisDetaylari[contentItem.id] = detay;
                }

                detay.CallCenterPhone = contentItem.callCenterPhone ?? detay.CallCenterPhone;
                detay.PaymentType = contentItem.payment?.paymentType ?? detay.PaymentType;
                detay.TotalIndirim = contentItem.promotions?.FirstOrDefault()?.totalSellerAmount.ToString() ?? detay.TotalIndirim ?? "0";
                detay.Not = string.IsNullOrWhiteSpace(contentItem.customerNote) ? detay.Not : contentItem.customerNote;

                var adres = contentItem.address;
                if (adres != null)
                    detay.Adres = AdresMetni(adres.neighborhood, adres.apartmentNumber, Onekli("Kat", adres.floor), Onekli("Daire", adres.doorNumber), adres.district, adres.city);

                var lines = contentItem.lines ?? new List<trendyol.lines>();  //Her contentItem'ın lines adlı bir listesi var.
                                                                                            //Eğer lines null ise boş bir liste (new List<...>()) oluşturuyor.
                                                                                            //Böylece lines null olsa da foreach patlamıyor.

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];

                    // Aynı siparişin aynı ürün satırı daha önce eklendiyse atla.
                    // (id bazlı kontrol tek başına yeterli değil: aynı sipariş birden fazla
                    // ürün içerdiğinde ilk üründen sonrakiler "zaten var" sanılıp atlanıyordu.)
                    string satirAnahtari = $"TR_{contentItem.id}_{i}_{line?.name}";
                    if (!islenenUrunSatirlari.Add(satirAnahtari)) continue;

                    // Modifier ürünler // entity framework kullanarak yazdık
                    var modifierNames = line?.modifierProducts? //line değişkenin varsa (null değilse) → onun içindeki modifierProducts listesini al.
                                            .Select(m => m?.name ?? "") //modifierProducts listesindeki her bir m için:Eğer m null değilse → m.name al.Eğer m ya da m.name null ise → boş string "" döndür.
                                            .ToList() ?? new List<string>(); // .ToList() ?? new List<string>()Select sonucu bir listeye(List<string>) çevriliyor.Eğer yukarıdaki işlemlerin tamamı null dönerse, boş bir liste(new List<string>()) oluştur.

                    string modifierNamesStr = SecenekleriBirlestir(modifierNames); //Listeyi "Peynir, Zeytin, Domates" gibi string'e çevirir, sadece "İstemiyorum" olanları atlar.

                    detay.Urunler.Add(new SiparisUrun
                    {
                        Adet = Math.Max(line?.items?.Count ?? 1, 1),
                        Name = line?.name ?? "",
                        ModifierNamesStr = modifierNamesStr,
                        BirimFiyat = FiyatMetni((decimal)(line?.unitSellingPrice ?? 0))
                    });
                }

                // Özet tabloda sipariş zaten varsa tekrar satır ekleme
                bool zatenVar = datagridrcguard.Rows
              .Cast<DataGridViewRow>()
              .Any(r => r.Cells["id"].Value?.ToString() == contentItem.id);

                if (zatenVar) continue;

                datagridrcguard.Rows.Add(

                    saat ?? "",

                    "Trendyol",  // Sabit değer

                    contentItem.id,

                    $"{contentItem.customer?.firstName ?? ""} {contentItem.customer?.lastName ?? ""}".Trim(),

                    contentItem.totalPrice.ToString("F0"),

                    detay.TotalIndirim ?? "0",

                    NotMetni(detay)
                );
            }
        }

        //YEMEKSEPETİ
        private void RestosepetYemeksepeti(string json, string saat)
        {
            OzetTabloHazirla();

            var veri3 = JsonConvert.DeserializeObject<yemeksepeti.yemeksepetilog>(json);
            if (string.IsNullOrEmpty(veri3?.code)) return;

            string siparisNo = veri3.code;

            if (!siparisDetaylari.TryGetValue(siparisNo, out var detay))
            {
                detay = new SiparisDetay();
                siparisDetaylari[siparisNo] = detay;
            }

            detay.CallCenterPhone = veri3.customer?.mobilePhone ?? detay.CallCenterPhone;
            detay.PaymentType = veri3.payment?.type ?? detay.PaymentType;
            detay.TotalIndirim = veri3.price?.discountAmountTotal ?? detay.TotalIndirim ?? "0";
            detay.Not = string.IsNullOrWhiteSpace(veri3.comments?.customerComment) ? detay.Not : veri3.comments.customerComment;

            var adres = veri3.delivery?.address;
            if (adres != null)
                detay.Adres = AdresMetni($"{adres.street} {adres.number}", adres.building, Onekli("Kat", adres.floor), adres.deliveryMainArea);

            var products = veri3.products ?? new List<yemeksepeti.products>();

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];

                string satirAnahtari = $"YS_{siparisNo}_{i}_{product?.name}";
                if (!islenenUrunSatirlari.Add(satirAnahtari)) continue;

                string toppings = product?.selectedToppings != null
                    ? SecenekleriBirlestir(product.selectedToppings.Select(t => t?.name ?? ""))
                    : "";

                detay.Urunler.Add(new SiparisUrun
                {
                    Adet = Math.Max(product?.quantity ?? 1, 1),
                    Name = product?.name ?? "",
                    ModifierNamesStr = toppings,
                    BirimFiyat = product?.unitPrice ?? "",
                    Not = product?.comment ?? ""
                });
            }

            bool zatenVar = datagridrcguard.Rows
          .Cast<DataGridViewRow>()
          .Any(r => r.Cells["id"].Value?.ToString() == siparisNo);

            if (zatenVar) return;

            datagridrcguard.Rows.Add(

                saat ?? "",

                "Yemeksepeti",  // Sabit değer

                siparisNo,

                $"{veri3.customer?.firstname ?? ""} {veri3.customer?.lastname ?? ""}".Trim(),

                veri3.price?.grandTotal ?? "0",

                detay.TotalIndirim ?? "0",

                NotMetni(detay)
            );
        }

        // Kolon içeriği kadar genişler ve tek satırda kalır; uzun içerik yatay kaydırma çubuğuyla okunur.
        // İçerik kısaysa tablonun sağında gri boşluk kalmasın diye kalan genişliği bu kolon doldurur.
        private static void KolonuBoslugaYay(DataGridView tablo, string kolonAdi)
        {
            if (!tablo.Columns.Contains(kolonAdi)) return;

            var kolon = tablo.Columns[kolonAdi];
            kolon.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            kolon.MinimumWidth = 300;
            kolon.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            int dolu = tablo.Columns.GetColumnsWidth(DataGridViewElementStates.Visible)
                     + (tablo.RowHeadersVisible ? tablo.RowHeadersWidth : 0);
            int dikeyKaydirma = tablo.Controls.OfType<VScrollBar>().Any(v => v.Visible) ? SystemInformation.VerticalScrollBarWidth : 0;
            int bosluk = tablo.ClientSize.Width - dolu - dikeyKaydirma - 2;

            if (bosluk > 0)
            {
                int genislik = kolon.Width + bosluk;
                kolon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                kolon.Width = genislik;
            }
        }

        // Kolonlar tablonun tamamını doldurur (sağda gri boşluk kalmaz); ağırlıklar kolonların göreli genişliği
        private static void KolonlariTabloyaYay(DataGridView tablo, params float[] agirliklar)
        {
            for (int i = 0; i < tablo.Columns.Count && i < agirliklar.Length; i++)
            {
                tablo.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                tablo.Columns[i].FillWeight = agirliklar[i] * 100;
            }
        }

        // Ekran boyutu müşteriden müşteriye değiştiği için kontroller sabit koordinatla değil oranlı bir ızgarayla yerleşir:
        //   [Tarih] [Kaynak] [LOG YAZDIR]       içerik kadar, "SİPARİŞ ÖZET" yazısının bittiği yerden başlar
        //   SİPARİŞ ÖZET                         yüksekliğin %52'si
        //   SİPARİŞ DETAY  | SİPARİŞ ADET        %25 (Adet 300 piksel)
        //   HATA RAPORU    | HATA SAYISI         %23 (%65 / %35)
        // Form küçülüp büyüdükçe tablolar da küçülüp büyür; sığmayan içerik tabloların kendi kaydırma çubuklarıyla okunur.
        private void DuzeniKur()
        {
            SuspendLayout();

            AutoScroll = false;                              // tablolar formun dışına taşmaz, alt çizgileri her zaman görünür
            FormBorderStyle = FormBorderStyle.Sizable;       // müşteri pencereyi kenarından sürükleyip boyutlandırabilsin
            MinimumSize = new Size(1000, 650);

            var ustSatir = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Anchor = AnchorStyles.Left,                  // tablolarla aynı sol hizadan başlar
                Margin = new Padding(0, 0, 0, 6)
            };
            foreach (Control c in new Control[] { tarihsec, kaynak_combo, button1 })
            {
                c.Anchor = AnchorStyles.None;
                c.Margin = new Padding(0, 0, 16, 0);
                ustSatir.Controls.Add(c);
            }

            var ortaBolum = IkiliBolum(label2, datagridrcguarddetay, label6, datagridsiparisadet,
                new ColumnStyle(SizeType.Percent, 100), new ColumnStyle(SizeType.Absolute, 300));
            var altBolum = IkiliBolum(label5, datagridhataraporu, label3, dataGridToplamHatalarıGoster,
                new ColumnStyle(SizeType.Percent, 65), new ColumnStyle(SizeType.Percent, 35));

            var kok = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(12, 8, 12, 12) };
            kok.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            kok.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            kok.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            kok.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            kok.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            kok.RowStyles.Add(new RowStyle(SizeType.Percent, 23));

            BaslikHazirla(label1);
            TabloHazirla(datagridrcguard, new Padding(0));

            // Üst satır "SİPARİŞ ÖZET" yazısının bittiği yerden başlar
            ustSatir.Margin = new Padding(label1.PreferredWidth, 0, 0, 6);

            kok.Controls.Add(ustSatir, 0, 0);
            kok.Controls.Add(label1, 0, 1);
            kok.Controls.Add(datagridrcguard, 0, 2);
            kok.Controls.Add(ortaBolum, 0, 3);
            kok.Controls.Add(altBolum, 0, 4);
            Controls.Add(kok);

            // Not / Adres kolonu tablonun yeni genişliğine göre boşluğu doldursun
            datagridrcguard.SizeChanged += (s, e) => KolonuBoslugaYay(datagridrcguard, "not");
            datagridrcguarddetay.SizeChanged += (s, e) => KolonuBoslugaYay(datagridrcguarddetay, "adres");

            ResumeLayout(true);
        }

        // Başlık + tablo çiftini yan yana koyan iki kolonlu bölüm (Detay|Adet, Hata Raporu|Hata Sayısı)
        private static TableLayoutPanel IkiliBolum(Label solBaslik, DataGridView solTablo, Label sagBaslik, DataGridView sagTablo,
            ColumnStyle solKolon, ColumnStyle sagKolon)
        {
            var bolum = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = new Padding(0, 8, 0, 0) };
            bolum.ColumnStyles.Add(solKolon);
            bolum.ColumnStyles.Add(sagKolon);
            bolum.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            bolum.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            BaslikHazirla(solBaslik);
            BaslikHazirla(sagBaslik);
            sagBaslik.Margin = new Padding(12, 0, 0, 3);
            TabloHazirla(solTablo, new Padding(0, 0, 6, 0));
            TabloHazirla(sagTablo, new Padding(6, 0, 0, 0));

            bolum.Controls.Add(solBaslik, 0, 0);
            bolum.Controls.Add(sagBaslik, 1, 0);
            bolum.Controls.Add(solTablo, 0, 1);
            bolum.Controls.Add(sagTablo, 1, 1);
            return bolum;
        }

        private static void BaslikHazirla(Label baslik)
        {
            baslik.AutoSize = true;
            baslik.Dock = DockStyle.None;
            baslik.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            baslik.Margin = new Padding(0, 0, 0, 3);
        }

        private static void TabloHazirla(DataGridView tablo, Padding kenar)
        {
            tablo.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            tablo.Dock = DockStyle.Fill;
            tablo.Margin = kenar;
        }

        private void SiparisDetayGoster(string siparisNo)
        {
            datagridrcguarddetay.Rows.Clear();
            datagridrcguarddetay.Columns.Clear();

            if (!siparisDetaylari.TryGetValue(siparisNo, out var detay)) return;

            // Kurye loglarında ürün listesi yok; ürün kolonları açılmadan sadece sipariş bilgileri gösterilir
            if (detay.Urunler.Count == 0)
            {
                datagridrcguarddetay.Columns.Add("tel", "İletişim No");
                datagridrcguarddetay.Columns.Add("odeme", "Ödeme Tipi");
                datagridrcguarddetay.Columns.Add("adres", "Adres");
                datagridrcguarddetay.Rows.Add(detay.CallCenterPhone, detay.PaymentType, detay.Adres);
                KolonuBoslugaYay(datagridrcguarddetay, "adres");
                return;
            }

            datagridrcguarddetay.Columns.Add("adet", "Adet");
            datagridrcguarddetay.Columns.Add("name", "Ana Ürün");
            datagridrcguarddetay.Columns.Add("modifierNamesStr", "İçindekiler");
            datagridrcguarddetay.Columns.Add("birimFiyat", "Birim Fiyat");
            datagridrcguarddetay.Columns.Add("tel", "İletişim No");
            datagridrcguarddetay.Columns.Add("odeme", "Ödeme Tipi");
            datagridrcguarddetay.Columns.Add("adres", "Adres");

            // Sipariş bilgileri her ürün satırında tekrar etmesin diye sadece ilk satıra yazılır
            bool ilkSatir = true;
            foreach (var urun in detay.Urunler)
            {
                datagridrcguarddetay.Rows.Add(
                    urun.Adet, urun.Name, urun.ModifierNamesStr, urun.BirimFiyat,
                    ilkSatir ? detay.CallCenterPhone : "",
                    ilkSatir ? detay.PaymentType : "",
                    ilkSatir ? detay.Adres : "");
                ilkSatir = false;
            }

            KolonuBoslugaYay(datagridrcguarddetay, "adres");
        }


        //GETIR
        private void Restosepetgetir(string json)
        {
            if (datagridrcguard.Columns.Count == 0)
            {
                datagridrcguard.Rows.Clear();
                datagridrcguard.Columns.Clear();
                datagridrcguard.Columns.Add("kanal", "Satış kanalı");
                datagridrcguard.Columns.Add("id", "Sipariş No");
                datagridrcguard.Columns.Add("name", "Musteriadı");
                datagridrcguard.Columns.Add("clientPhoneNumber", "İletişim No");
                datagridrcguard.Columns.Add("product", "Urunler");
                datagridrcguard.Columns.Add("totalPrice", "toplamtutar");

            }
            var veri2 = JsonConvert.DeserializeObject<getir.getirlog>(json);

            // Eğer products null ise çık
            if (veri2?.products == null) return;

            // JSON içindeki products listesi üzerinde dön
            foreach (var product in veri2.products)
            {
                // Modifier ürünler (optionCategories içindeki options isimleri)
                var modifierNames = product.optionCategories?
                                        .SelectMany(oc => oc.options)
                                        .Select(o => o.name?.tr ?? "")
                                        .ToList() ?? new List<string>();

                string modifierNamesStr = string.Join(", ", modifierNames);


                // Satırı DataGridView'e ekle
                datagridrcguard.Rows.Add(

                    "Getir",  // Sabit değer
                    veri2.id ?? "",                          // sipariş id


                    veri2.products.FirstOrDefault()?.totalPrice, // toplam fiyat

                    product.optionCategories?
                           .SelectMany(oc => oc.options)
                           .Sum(o => o.price)                // toplam indirim
                           .ToString("F0") ?? "0",

                    veri2.client?.clientPhoneNumber ?? "", // veri2.client?.location?.FirstOrDefault()?.clientPhoneNumber ?? "",List olmadıgı ıcın boyle yazılmaz

                    veri2.client?.name ?? "",                // müşteri adı

                    veri2.paymentMethodText?.tr ?? "",

                    //    $"{veri2.client?.location?.lat ?? ""}/{ veri2.client?.location?.lon ?? "" }", // adres

                    product.name?.tr ?? "",                  // ürün adı (TR)

                    modifierNamesStr                         // opsiyonlar
                );
            }

        }



        //MİGROS
        private void Restosepetmigros(string json, string saat)
        {
            OzetTabloHazirla();

            var veri4 = JsonConvert.DeserializeObject<migros.migroslog>(json);
            if (veri4?.data == null) return;

            foreach (var store in veri4.data)
            {
                if (store?.pendingOrderDetailsDTOS == null) continue;

                foreach (var order in store.pendingOrderDetailsDTOS)
                {
                    if (order == null || order.id == 0) continue;

                    string siparisNo = order.id.ToString();

                    // Migros tutarları kuruş cinsinden gönderiyor (60000 = 600,00 TL)
                    decimal toplamTutar = order.totalPrice / 100m;
                    decimal indirim = order.discountedPrice > 0 ? (order.totalPrice - order.discountedPrice) / 100m : 0m;

                    if (!siparisDetaylari.TryGetValue(siparisNo, out var detay))
                    {
                        detay = new SiparisDetay();
                        siparisDetaylari[siparisNo] = detay;
                    }

                    detay.CallCenterPhone = order.phoneNumber ?? detay.CallCenterPhone;
                    detay.PaymentType = order.paymentTypeDescription ?? order.paymentType ?? detay.PaymentType;
                    detay.TotalIndirim = indirim.ToString("0.##");
                    detay.Not = string.IsNullOrWhiteSpace(order.orderNote) ? detay.Not : order.orderNote;
                    detay.Adres = string.IsNullOrWhiteSpace(order.address) ? detay.Adres : order.address;

                    var products = order.products ?? new List<migros.products>();

                    for (int i = 0; i < products.Count; i++)
                    {
                        var product = products[i];

                        string satirAnahtari = $"MG_{siparisNo}_{i}_{product?.name}";
                        if (!islenenUrunSatirlari.Add(satirAnahtari)) continue;

                        string icindekiler = SecenekleriBirlestir(product?.options?.Select(o => o?.itemNames ?? "") ?? Enumerable.Empty<string>());

                        detay.Urunler.Add(new SiparisUrun
                        {
                            Adet = Math.Max(product?.amount ?? 1, 1),
                            Name = product?.name ?? "",
                            ModifierNamesStr = icindekiler,
                            BirimFiyat = FiyatMetni((product?.price ?? 0) / 100m),
                            Not = product?.note ?? ""
                        });
                    }

                    // Migros bekleyen siparişleri her sorguda tekrar gönderdiği için aynı sipariş tek satır olmalı
                    bool zatenVar = datagridrcguard.Rows
                  .Cast<DataGridViewRow>()
                  .Any(r => r.Cells["id"].Value?.ToString() == siparisNo);

                    if (zatenVar) continue;

                    datagridrcguard.Rows.Add(

                        saat ?? "",

                        "Migros",  // Sabit değer

                        siparisNo,

                        order.customerFullName ?? "",

                        toplamTutar.ToString("0.##"),

                        detay.TotalIndirim ?? "0",

                        NotMetni(detay)
                    );
                }
            }
        }

        //KURYE (my_kurye): Fiyuu, Diğer Kurye, ...
        private void KuryeSiparis(string json, string saat, string kuryeAdi)
        {
            kurye.kuryesiparis veri;
            try
            {
                veri = JsonConvert.DeserializeObject<kurye.kuryesiparis>(json);
            }
            catch (JsonException)
            {
                return;
            }

            if (string.IsNullOrEmpty(veri?.orderId)) return;

            OzetTabloHazirla(kuryeLogu: true);

            string siparisNo = veri.orderId;

            if (!siparisDetaylari.TryGetValue(siparisNo, out var detay))
            {
                detay = new SiparisDetay();
                siparisDetaylari[siparisNo] = detay;
            }

            string odemeTipi = veri.payment?.paymentTypeSId;

            detay.CallCenterPhone = veri.customer?.customerPhone ?? detay.CallCenterPhone;
            detay.PaymentType = KuryeOdemeTipi(odemeTipi) ?? detay.PaymentType;
            detay.Adres = string.IsNullOrWhiteSpace(veri.address?.fullAddress) ? detay.Adres : veri.address.fullAddress;
            detay.Not = string.IsNullOrWhiteSpace(veri.address?.addressDirection) ? detay.Not : veri.address.addressDirection;

            // Aynı sipariş kuryeye tekrar gönderilirse ilk satır kalır
            bool zatenVar = datagridrcguard.Rows
          .Cast<DataGridViewRow>()
          .Any(r => r.Cells["id"].Value?.ToString() == siparisNo);

            if (zatenVar) return;

            string kanal = veri.salesChannelSId ?? "";
            if (veri.isTestOrder) kanal += " - TEST";

            datagridrcguard.Rows.Add(

                saat ?? "",

                kuryeAdi,

                kanal,

                siparisNo,

                $"{veri.customer?.firstName ?? ""} {veri.customer?.lastName ?? ""}".Trim(),

                // Kurye tutarı kuruş cinsinden gönderiyor (71740 = 717,40 TL)
                ((veri.payment?.totalPrice ?? 0) / 100m).ToString("0.##"),

                NotMetni(detay)
            );
        }

        // Tanınmayan ödeme tipi olduğu gibi gösterilir
        private static string KuryeOdemeTipi(string tip)
        {
            switch ((tip ?? "").ToLowerInvariant())
            {
                case "": return null;
                case "cash": return "Nakit";
                case "online-credit-card": return "Online Kredi Kartı";
                case "offline-credit-card": return "Kapıda Kredi Kartı";
                case "offline-metropol": return "Kapıda Metropol";
                default: return tip;
            }
        }

        //DİĞER KURYE (my_kurye)
        private void DigerKuryeSiparis(string json, string saat, string kuryeAdi)
        {
            kurye.digerkuryesiparis veri;
            try
            {
                veri = JsonConvert.DeserializeObject<kurye.digerkuryesiparis>(json);
            }
            catch (JsonException)
            {
                return;
            }

            if (string.IsNullOrEmpty(veri?.orderId)) return;

            OzetTabloHazirla(kuryeLogu: true);

            string siparisNo = veri.orderId;

            if (!siparisDetaylari.TryGetValue(siparisNo, out var detay))
            {
                detay = new SiparisDetay();
                siparisDetaylari[siparisNo] = detay;
            }

            detay.CallCenterPhone = string.IsNullOrWhiteSpace(veri.phoneNumber) ? detay.CallCenterPhone : veri.phoneNumber;
            detay.PaymentType = KuryeOdemeTipi(veri.paymentMethod) ?? detay.PaymentType;
            detay.Adres = string.IsNullOrWhiteSpace(veri.address) ? detay.Adres : veri.address;

            // addressDetail "ilçe/il Tarif: <müşteri notu>" biçiminde; not olarak sadece tarif kısmı alınır ("-" boş demek)
            var tarif = Regex.Match(veri.addressDetail ?? "", @"Tarif:\s*(.*)$");
            string not = tarif.Success ? tarif.Groups[1].Value.Trim() : "";
            detay.Not = not == "" || not == "-" ? detay.Not : not;

            bool zatenVar = datagridrcguard.Rows
          .Cast<DataGridViewRow>()
          .Any(r => r.Cells["id"].Value?.ToString() == siparisNo);

            if (zatenVar) return;

            datagridrcguard.Rows.Add(

                saat ?? "",

                kuryeAdi,

                veri.salesChannel ?? "",

                siparisNo,

                veri.nameSurname ?? "",

                veri.totalAmount.ToString("0.##"),

                NotMetni(detay)
            );
        }

        //RESTOWAY (my_kurye)
        private void RestoWaySiparis(string json, string saat, string kuryeAdi)
        {
            kurye.restowaysiparis veri;
            try
            {
                veri = JsonConvert.DeserializeObject<kurye.restowaysiparis>(json);
            }
            catch (JsonException)
            {
                return;
            }

            if (veri == null || veri.RestoOrderId == 0) return;

            OzetTabloHazirla(kuryeLogu: true);

            // Platformun sipariş numarası (Yemeksepeti/Trendyol) daha anlamlı; yoksa RestoWay numarası
            string siparisNo = string.IsNullOrWhiteSpace(veri.MarketOrderNo) ? veri.RestoOrderId.ToString() : veri.MarketOrderNo;

            if (!siparisDetaylari.TryGetValue(siparisNo, out var detay))
            {
                detay = new SiparisDetay();
                siparisDetaylari[siparisNo] = detay;
            }

            detay.CallCenterPhone = string.IsNullOrWhiteSpace(veri.MobilePhone) ? detay.CallCenterPhone : veri.MobilePhone;
            detay.PaymentType = string.IsNullOrWhiteSpace(veri.PaymentMethod) ? detay.PaymentType : veri.PaymentMethod;
            detay.Adres = string.IsNullOrWhiteSpace(veri.Address) ? detay.Adres : veri.Address;
            detay.Not = !string.IsNullOrWhiteSpace(veri.AddressDescription) ? veri.AddressDescription
                      : !string.IsNullOrWhiteSpace(veri.OrderNote) ? veri.OrderNote
                      : detay.Not;

            bool zatenVar = datagridrcguard.Rows
          .Cast<DataGridViewRow>()
          .Any(r => r.Cells["id"].Value?.ToString() == siparisNo);

            if (zatenVar) return;

            datagridrcguard.Rows.Add(

                saat ?? "",

                kuryeAdi,

                RestoWayMarket(veri.MarketId),

                siparisNo,

                $"{veri.FirstName ?? ""} {veri.LastName ?? ""}".Trim(),

                veri.Amount.ToString("0.##"),

                NotMetni(detay)
            );
        }

        // MarketId sipariş numarası biçimlerinden çıkarıldı (1: "o477-2638-ynh6", 3: 64 karakterlik hash); bilinmeyenler numarasıyla gösterilir
        private static string RestoWayMarket(int marketId)
        {
            switch (marketId)
            {
                case 1: return "Yemeksepeti";
                case 3: return "Trendyol";
                default: return $"Market {marketId}";
            }
        }

        // Kurye adı müşteriye göre değişiyor: "Fiyuu Json :", "RestoWay  Json :", "Diğer Kurye Json :" ...
        private static readonly Regex KuryeSatiriRegex = new Regex(@"^([^:{]+?)\s+Json :");

        // Form seviyesinde (global)
        private List<rcguard.rcguardlog> veriListesi = new List<rcguard.rcguardlog>();
        private List<yemeksepeti.yemeksepetilog> veriys = new List<yemeksepeti.yemeksepetilog>();









        //RCGUARD
        // "Gelen Veri : [...]" satırı: dizi içindeki her eleman bir sipariş. Boş dizi ([]) sipariş yok demek.
        private void RcguardSiparis(string json, string saat)
        {
            List<rcguard.rcguardlog> siparisler;
            try
            {
                siparisler = JsonConvert.DeserializeObject<List<rcguard.rcguardlog>>(json);
            }
            catch (JsonException)
            {
                return; // "Gelen Veri" satırı sipariş listesi değilse atla
            }

            if (siparisler == null || siparisler.Count == 0) return;

            OzetTabloHazirla();

            foreach (var s in siparisler)
            {
                if (string.IsNullOrEmpty(s?.orderid)) continue;

                string siparisNo = s.orderid;

                if (!siparisDetaylari.TryGetValue(siparisNo, out var detay))
                {
                    detay = new SiparisDetay();
                    siparisDetaylari[siparisNo] = detay;
                }

                // Puan kullanımı gibi indirimler negatif tutarla geliyor
                decimal indirim = Math.Abs(s.discount?.Sum(d => d?.amount ?? 0) ?? 0);

                detay.CallCenterPhone = s.client?.clientPhoneNumber ?? detay.CallCenterPhone;
                detay.PaymentType = s.payments != null && s.payments.Count > 0
                    ? string.Join(", ", s.payments.Select(p => p?.paymentMethodText ?? ""))
                    : detay.PaymentType;
                detay.TotalIndirim = indirim.ToString("0.##");
                detay.Not = RcguardNot(s) ?? detay.Not;

                // Tekil ürünler
                var products = s.products ?? new List<rcguard.products>();
                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];

                    string satirAnahtari = $"RC_{siparisNo}_P{i}_{product?.name}";
                    if (!islenenUrunSatirlari.Add(satirAnahtari)) continue;

                    string icindekiler = SecenekleriBirlestir(product?.options?.Select(o => o?.name ?? "") ?? Enumerable.Empty<string>());

                    detay.Urunler.Add(new SiparisUrun
                    {
                        Adet = Math.Max(product?.quantity ?? 1, 1),
                        Name = product?.name ?? "",
                        ModifierNamesStr = icindekiler,
                        BirimFiyat = FiyatMetni(product?.price ?? 0),
                        Not = product?.note ?? ""
                    });
                }

                // Menüler: alt ürünlerin ilki menünün kendisi olduğu için atlanır, aynı ürünler "5 x Acısız Lahmacun" gibi gruplanır
                var menus = s.menus ?? new List<rcguard.menus>();
                for (int i = 0; i < menus.Count; i++)
                {
                    var menu = menus[i];

                    string satirAnahtari = $"RC_{siparisNo}_M{i}_{menu?.name}";
                    if (!islenenUrunSatirlari.Add(satirAnahtari)) continue;

                    var altUrunler = (menu?.products ?? new List<rcguard.products>())
                        .Where(p => p != null && p.name != menu.name)
                        .SelectMany(p => new[] { p.name }.Concat(p.options?.Select(o => o?.name) ?? Enumerable.Empty<string>()))
                        .Where(n => !string.IsNullOrWhiteSpace(n) && !SadeceIstemiyorum(n))
                        .GroupBy(n => n)
                        .Select(g => AdetliAd(g.Count(), g.Key));

                    detay.Urunler.Add(new SiparisUrun
                    {
                        Adet = Math.Max(menu?.quantity ?? 1, 1),
                        Name = menu?.name ?? "",
                        ModifierNamesStr = string.Join(", ", altUrunler),
                        BirimFiyat = FiyatMetni(menu?.price ?? 0),
                        Not = menu?.note ?? ""
                    });
                }

                bool zatenVar = datagridrcguard.Rows
              .Cast<DataGridViewRow>()
              .Any(r => r.Cells["id"].Value?.ToString() == siparisNo);

                if (zatenVar) continue;

                datagridrcguard.Rows.Add(

                    saat ?? "",

                    s.orderTypeText ?? "",   // "Takeaway", "Dining", "Gel Al", "Adrese Teslim"

                    siparisNo,

                    s.client?.name ?? "",

                    s.totalAmount.ToString("0.##"),

                    detay.TotalIndirim ?? "0",

                    NotMetni(detay)
                );
            }
        }

        private static string AdetliAd(int adet, string ad)
        {
            return adet > 1 ? $"{adet} x {ad}" : ad ?? "";
        }

        // clientNote: "ad - Telefon : ... - Sipariş Notu : salata bol olsun - Sipariş zamanı : Hemen - ..."
        // İçinden sadece müşterinin yazdığı notu, ileri tarihli siparişte de sipariş zamanını alır.
        private static string RcguardNot(rcguard.rcguardlog s)
        {
            var notlar = new List<string>();

            var m = Regex.Match(s.clientNote ?? "", @"Sipariş Notu\s*:\s*(.*?)\s*-\s*Sipariş zamanı", RegexOptions.IgnoreCase);
            if (m.Success && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                notlar.Add(m.Groups[1].Value.Trim());

            string zaman = (s.orderNote ?? "").Trim();
            if (zaman != "" && !zaman.EndsWith("Hemen", StringComparison.OrdinalIgnoreCase))
                notlar.Add(zaman);

            return notlar.Count > 0 ? string.Join(" - ", notlar) : null;
        }

        // Form seviyesinde (class içine yazılacak)
        private HashSet<string> islenmisJsonlar = new HashSet<string>();

        public void button1_Click_1(object sender, EventArgs e)
        {


            // DataGridView ayarları
            datagridrcguarddetay.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            datagridrcguarddetay.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;




            datagridrcguard.RowHeadersVisible = false;
            datagridrcguarddetay.RowHeadersVisible = false;


            // Önce DataGridView temizle
            datagridrcguarddetay.Rows.Clear();
            datagridrcguard.Rows.Clear();
            datagridrcguard.Columns.Clear();   // TSM ile sipariş kaynakları farklı kolonlar kullanıyor
            datagridsiparisadet.Rows.Clear();
            datagridhataraporu.Rows.Clear();
            dataGridToplamHatalarıGoster.Rows.Clear();
            islenenUrunSatirlari.Clear();
            siparisDetaylari.Clear();
            tsmFisleri.Clear();



            string klasorYolu = LogKlasoru();

            if (klasorYolu == null)
            {
                MessageBox.Show(LogKlasoruYokMesaji);
                return;
            }







            string secilen = SeciliKaynakDosyasi();

            if (string.IsNullOrEmpty(secilen))
            {
                MessageBox.Show("Lütfen bir kaynak seçin.");
                return;
            }

            // DateTimePicker’dan seçilen tarihi al
            string dosyaTarihi = tarihsec.Value.ToString("yyyyMMdd");


            // Dosya yolunu oluştur(örn: Yemeksepeti_20250925.txt)
            string dosyaYolu = Path.Combine(klasorYolu, $"{secilen}_{dosyaTarihi}.txt");

            if (!File.Exists(dosyaYolu))
            {
                MessageBox.Show("Seçilen tarihe ait log bulunamadı.");
                return;
            }

            //Dosyayı satır satır oku

            string[] satirlar = LogDosyasiniOku(dosyaYolu);

            // Trendyol ("Trendyol :"), Yemeksepeti ("OrderData :"), Migros ("Migros :") ve RCGuard ("Gelen Veri : [")
            // siparişleri ortak SİPARİŞ ÖZET / SİPARİŞ DETAY tablolarına yazılıyor.
            // TsmLclSrv'deki Ingenico fişleri ("Ingenico Gelen Veri :") aynı tablolara kendi kolonlarıyla yazılıyor.
            for (int i = 0; i < satirlar.Length; i++)
            {
                string satir = satirlar[i];

                // Trendyol'da JSON'dan sonra satır kırılıyor, zaman bir sonraki satırda " : 27.09.2026 12:13:47" olarak geliyor
                string saat = LogSaati(satir);
                if (saat == null && i + 1 < satirlar.Length && satirlar[i + 1].TrimStart().StartsWith(":"))
                    saat = LogSaati(satirlar[i + 1]);

                if (satir.StartsWith("Trendyol :"))
                {
                    string json = ExtractJson(satir);
                    if (json == null) continue;

                    RestoSepetTrendyol(json, saat);
                }
                else if (satir.StartsWith("OrderData :"))
                {
                    string json = ExtractJson(satir);
                    if (json == null) continue;

                    RestosepetYemeksepeti(json, saat);
                }
                else if (satir.StartsWith("Migros :"))
                {
                    string json = ExtractJson(satir);
                    if (json == null) continue;

                    Restosepetmigros(json, saat);
                }
                else if (satir.StartsWith("Gelen Veri : [{")) // RCGuard siparişleri (boş liste "[]" atlanır)
                {
                    string json = ExtractJson(satir);
                    if (json == null) continue;

                    RcguardSiparis(json, saat);
                }
                else if (KuryeSatiriRegex.IsMatch(satir)) // my_kurye: "<Kurye adı> Json :{...}"
                {
                    // Kurye firmaya göre JSON yapısı değişiyor; sadece sipariş taşıyan satırlar okunur, durum sorguları atlanır
                    // Fiyuu: "orderId" + "customer" nesnesi, Diğer Kurye: "orderId" + düz "nameSurname", RestoWay: "RestoOrderId"
                    bool restoWaySiparisi = satir.Contains("\"RestoOrderId\":");
                    bool digerKuryeSiparisi = !restoWaySiparisi && satir.Contains("\"orderId\":") && satir.Contains("\"nameSurname\":");
                    bool fiyuuSiparisi = !restoWaySiparisi && !digerKuryeSiparisi && satir.Contains("\"orderId\":");
                    if (!fiyuuSiparisi && !restoWaySiparisi && !digerKuryeSiparisi) continue;

                    string json = ExtractJson(satir);
                    if (json == null) continue;

                    string kuryeAdi = KuryeSatiriRegex.Match(satir).Groups[1].Value.Trim();

                    if (restoWaySiparisi)
                        RestoWaySiparis(json, saat, kuryeAdi);
                    else if (digerKuryeSiparisi)
                        DigerKuryeSiparis(json, saat, kuryeAdi);
                    else
                        KuryeSiparis(json, saat, kuryeAdi);
                }
                else if (satir.StartsWith("Ingenico Gelen Veri :")) // TsmLclSrv: Ingenico ÖKC'den gelen fiş
                {
                    string json = ExtractJson(satir);
                    if (json == null) continue;

                    LogTSM(json, saat);
                }
            }

            SiparisAdetGoster();
            HataRaporuOlustur();
            KolonuBoslugaYay(datagridrcguard, "not");

           



        }
        // SİPARİŞ ADET: özet tablodaki siparişler satış kanalına göre sayılır (TSM fişlerinde kanal olmadığı için sadece toplam yazılır)
        private void SiparisAdetGoster()
        {
            datagridsiparisadet.Rows.Clear();
            datagridsiparisadet.Columns.Clear();
            datagridsiparisadet.Columns.Add("kanal", "Satış Kanalı");
            datagridsiparisadet.Columns.Add("adet", "Sipariş Adedi");
            datagridsiparisadet.Columns["kanal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var satirlar = datagridrcguard.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList();

            if (datagridrcguard.Columns.Contains("kanal"))
            {
                var kanallar = satirlar
                    .GroupBy(r => (r.Cells["kanal"].Value?.ToString() ?? "").Trim(), StringComparer.CurrentCultureIgnoreCase)
                    .OrderByDescending(g => g.Count());

                foreach (var kanal in kanallar)
                    datagridsiparisadet.Rows.Add(kanal.Key == "" ? "(Belirtilmemiş)" : kanal.Key, kanal.Count());
            }

            int toplamSatir = datagridsiparisadet.Rows.Add("TOPLAM", satirlar.Count);
            datagridsiparisadet.Rows[toplamSatir].DefaultCellStyle.Font = new Font(datagridsiparisadet.Font, FontStyle.Bold);
            VurguRengi(datagridsiparisadet.Rows[toplamSatir], AcikSari);
        }

        // Dikkat çekilmesi gereken satırlar: sarı = tekrar eden / toplam, kırmızı = log bitene kadar düzelmeyen sorun
        private static readonly Color AcikSari = Color.FromArgb(255, 242, 204);
        private static readonly Color AcikKirmizi = Color.FromArgb(248, 203, 203);

        private static Color? HataVurgusu(string olay, string sure)
        {
            if ((sure ?? "").Contains("devam ediyor")) return AcikKirmizi;   // "… log sonuna kadar düzelmedi"
            if ((olay ?? "").Contains(" kez")) return AcikSari;              // "… – 4464 kez" gibi tekrar eden hata dönemi
            return null;
        }

        // Seçiliyken de renk korunsun diye seçim rengi aynı tonun koyusu yapılır (yazı siyah kalır)
        private static void VurguRengi(DataGridViewRow satir, Color? renk)
        {
            if (renk == null) return;

            var r = renk.Value;
            satir.DefaultCellStyle.BackColor = r;
            satir.DefaultCellStyle.SelectionBackColor = Color.FromArgb(r.R * 4 / 5, r.G * 4 / 5, r.B * 4 / 5);
            satir.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        string ExtractJson(string line)
        {
            int startBracket = line.IndexOf('[');
            int startBrace = line.IndexOf('{');

            // Önce [ ile başlayan JSON'u kontrol et
            if (startBracket >= 0 && (startBracket < startBrace || startBrace < 0))
            {
                int depth = 0;
                for (int i = startBracket; i < line.Length; i++)
                {
                    if (line[i] == '[') depth++;
                    else if (line[i] == ']') depth--;

                    if (depth == 0)
                        return line.Substring(startBracket, i - startBracket + 1);
                }
            }

            // { ile başlayan JSON
            if (startBrace >= 0)
            {
                int depth = 0;
                for (int i = startBrace; i < line.Length; i++)
                {
                    if (line[i] == '{') depth++;
                    else if (line[i] == '}') depth--;

                    if (depth == 0)
                        return line.Substring(startBrace, i - startBrace + 1);
                }
            }

            return null;
        }







        //datagridrcguard taki seçilen satırın datagridrcguarddetay a yazılması
        private void datagridrcguard_SelectionChanged(object sender, EventArgs e)
        {
            if (datagridrcguard.SelectedRows.Count == 0) return; //hiç satır seçilmediyse çık

            if (!datagridrcguard.Columns.Contains("id")) return;

            // Sipariş no "id" kolonunda; detaylar siparisDetaylari sözlüğünden gösterilir
            string secilenSiparisNo = datagridrcguard.SelectedRows[0].Cells["id"].Value?.ToString();
            if (string.IsNullOrEmpty(secilenSiparisNo)) return;

            if (tsmFisleri.TryGetValue(secilenSiparisNo, out var tsmFis))
                TsmDetayGoster(tsmFis);
            else
                SiparisDetayGoster(secilenSiparisNo);
        }

        




        private class HataDonemi
        {
            public DateTime Ilk;
            public DateTime Son;
            public int Adet;
        }

        private static string SureMetni(TimeSpan fark)
        {
            return fark.TotalHours >= 1
                ? $"{(int)fark.TotalHours} sa {fark.Minutes} dk {fark.Seconds} sn"
                : $"{(int)fark.TotalMinutes} dk {fark.Seconds} sn";
        }

        // "HTTP/1.1 503 Service Unavailable" (RestoPOS logları) ya da ".NET: The remote server returned an error: (404) Not Found."
        private static readonly Regex HttpHataRegex = new Regex(
            @"HTTP/\d(?:\.\d)?\s+(?<kod>\d{3})(?<metin>[A-Za-z ]*)|returned an error:\s*\((?<kod>\d{3})\)(?<metin>[A-Za-z ]*)");

        private static string HttpHataAciklamasi(int kod)
        {
            switch (kod)
            {
                case 400: return "geçersiz istek";
                case 401: return "erişim hatası";
                case 403: return "erişim yasak";
                case 404: return "adres bulunamadı";
                case 500: return "sunucu hatası";
                case 503: return "sunucu şu an kullanılamıyor";
                default: return kod >= 500 ? "sunucu hatası" : "istek hatası";
            }
        }

        // Kanal satırın başındaki kısım ("Trendyol : https://..."); satır doğrudan adresle başlıyorsa adresin sunucu adı
        private static string HttpKanali(string satir)
        {
            string bas = satir.Split(new[] { " : " }, 2, StringSplitOptions.None)[0].Trim();
            if (bas.StartsWith("http", StringComparison.OrdinalIgnoreCase) && Uri.TryCreate(bas, UriKind.Absolute, out var adres))
                return adres.Host;
            return bas;
        }

        // RCGuard'ın sık tekrar eden hata satırları; null ise satır bu türden değildir
        private static string TekrarlayanRcguardHatasi(string satir)
        {
            if (!satir.StartsWith("Adisyon Kayit Hata")) return null;

            if (satir.Contains("closed dataset"))
                return "Adisyon kaydedilemedi (veritabanı tablosu kapalı)";

            if (satir.Contains("Access violation"))
                return "Adisyon kaydında program hatası (Access violation)";

            return "Adisyon kaydedilemedi";
        }

        // LOG YAZDIR'a basınca siparişlerden sonra seçili log dosyasının hata raporu da çıkarılır (ayrı buton yok)
        private void HataRaporuOlustur()
        {

            string klasorYolu = LogKlasoru();

            if (klasorYolu == null)
            {
                MessageBox.Show(LogKlasoruYokMesaji);
                return;
            }



            string secilen = SeciliKaynakDosyasi();

            if (string.IsNullOrEmpty(secilen))
            {
                MessageBox.Show("Lütfen bir kaynak seçin.");
                return; //metodu durdur
            }

            string dosyaTarihi = tarihsec.Value.ToString("yyyyMMdd");
            string dosyaYolu = Path.Combine(klasorYolu, $"{secilen}_{dosyaTarihi}.txt"); //dosyaYolu = C:\RestoPOS\MY\LOG\RcGuard_20251105.txt

            if (!File.Exists(dosyaYolu))
            {
                MessageBox.Show("Seçilen tarihe ait log bulunamadı.");
                return;
            }

            datagridhataraporu.Rows.Clear();
            dataGridToplamHatalarıGoster.Rows.Clear();

            // Seçili kaynağın (RCGuard, RestoSepet, ...) tüm hataları tek HATA RAPORU tablosunda, toplamları HATA SAYISI tablosunda
            datagridhataraporu.RowHeadersVisible = false;
            dataGridToplamHatalarıGoster.RowHeadersVisible = false;
            //datagridhataraporu.RowHeadersVisible = false;

            if (datagridhataraporu.Columns.Count == 0)
            {
                datagridhataraporu.Columns.Add("Olay", "Olay");
                datagridhataraporu.Columns.Add("Zaman", "Olayın Gerçekleştiği Zaman");
                datagridhataraporu.Columns.Add("Süre", "Süre");
                KolonlariTabloyaYay(datagridhataraporu, 3, 1.5f, 1.5f);
                //datagridhataraporu.Columns.Add("Sayı", "Hata Sayısı");
            }

            string[] satirlar = LogDosyasiniOku(dosyaYolu); //okur, her satırı bir string dizisine koyar.

            DateTime? sonDurma = null; //Servisin durduğu zamanı saklamak için. Başlangıçta null.
            int hataSayisi = 0; //databasedeki Toplam hata sayısını saymak için.
            int hatasayisi2 = 0; //servisteki Toplam hata sayısını saymak için.
            int hatasayisi3 = 0; //Bilinen böyle bir ana bilgisayar yok hatasın ı saymak için.
            int hataSayisi4 = 0; //Istek Hata : A connection attempt failed because the connected party did not properly respond after a period of time hatasını saymak için.

            // RestoSepet Hata Sayıları
            int rsVersionErrorCount = 0;
            int rsCourierErrorCount = 0;
            int rsRestartCount = 0;



            //database hatası hesaplaması ıcın değişken tanımlama
            DateTime? ilkDatabaseKopma = null;
            DateTime? sonDatabaseKopma = null;



         



                // İnternet kopması (No such host is known)
                bool hostHataAktif = false;
                DateTime? hostHataBaslangic = null;
                bool httpsSeen = false;
                bool gelenVeriSeen = false;
                int successPairCount = 0;

                // Server ulaşılamadı (A connection attempt failed)
                bool baglantiHataAktif = false;
                DateTime? baglantiHataBaslangic = null;
                bool httpsSeen2 = false;
                bool gelenVeriSeen2 = false;
                int successPairCount2 = 0;




            // RCGuard raporu satırları önce toplanır, sonra zamana göre sıralanıp tabloya yazılır
            // (tekrar eden hatalar dönem bitince eklendiği için doğrudan yazılırsa sıra bozulur)
            var raporSatirlari = new List<Tuple<DateTime, string, string>>();
            void RaporaEkle(DateTime zaman, string olay, string sure) => raporSatirlari.Add(Tuple.Create(zaman, olay, sure));

            // Sık tekrar eden hatalar (373 kez "closed dataset" gibi) dönem olarak tek satırda gösterilir
            var acikDonemler = new Dictionary<string, HataDonemi>();
            var hataSayilari = new Dictionary<string, int>();
            DateTime? sonLogZamani = null;

            void DonemiKapat(string olay)
            {
                if (!acikDonemler.TryGetValue(olay, out var d)) return;
                acikDonemler.Remove(olay);

                string sure = d.Adet == 1 ? "" : $"{SureMetni(d.Son - d.Ilk)} (son: {d.Son:HH:mm:ss})";
                RaporaEkle(d.Ilk, d.Adet == 1 ? olay : $"{olay} – {d.Adet} kez", sure);
            }

            // Hata satırını sayar; aynı hata 5 dakikadan kısa aralıkla tekrar ediyorsa açık döneme eklenir, değilse yeni dönem başlar
            void DonemeEkle(string olay, DateTime zaman)
            {
                hataSayilari[olay] = hataSayilari.TryGetValue(olay, out var adet) ? adet + 1 : 1;
                if (zaman == DateTime.MinValue) return;

                if (acikDonemler.TryGetValue(olay, out var donem) && (zaman - donem.Son).TotalMinutes > 5)
                    DonemiKapat(olay);

                if (acikDonemler.TryGetValue(olay, out donem))
                {
                    donem.Son = zaman;
                    donem.Adet++;
                }
                else
                {
                    acikDonemler[olay] = new HataDonemi { Ilk = zaman, Son = zaman, Adet = 1 };
                }
            }

            bool restoSepetLogu = secilen.StartsWith("RestoSepet", StringComparison.OrdinalIgnoreCase);
            bool kuryeLogu = secilen.Equals("my_kurye", StringComparison.OrdinalIgnoreCase);
            bool okcLogu = secilen.Equals("TsmLclSrv", StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < satirlar.Length; i++) // satırlarda dolaşma
            {
                string satir = satirlar[i];

                // Bazı satırlarda zaman satır sonunda değil bir sonraki satırda ("Malformed string : 30.09.2026 06:27:37")
                DateTime SatirZamani()
                {
                    DateTime t = ExtractDate(satir);
                    return t == DateTime.MinValue && i + 1 < satirlar.Length ? ExtractDate(satirlar[i + 1]) : t;
                }

                // Getir entegrasyonu kapandı (Trendyol/Uber'e geçildi); Getir geçen satırlar rapora alınmaz
                if (restoSepetLogu && satir.Contains("Getir")) continue;

                // -----------------------------------------------------
                // TÜM LOGLAR: veritabanı hatası
                // -----------------------------------------------------
                if (satir.Contains("SQL error code = -303"))
                {
                    DonemeEkle("Database hatası (SQL error code = -303)", SatirZamani());
                    continue;
                }

                // -----------------------------------------------------
                // TÜM LOGLAR: HTTP hata kodları (4xx / 5xx)
                // "Trendyol : https://... : Hata§HTTP/1.1 503 Service Unavailable - ..." → "Trendyol sunucu şu an kullanılamıyor (503 Service Unavailable)"
                // 2xx başarılı, 3xx yönlendirme olduğu için rapora alınmaz
                // -----------------------------------------------------
                var http = HttpHataRegex.Match(satir);
                if (http.Success)
                {
                    int kod = int.Parse(http.Groups["kod"].Value);
                    if (kod >= 400)
                    {
                        string metin = http.Groups["metin"].Value.Trim();
                        string olay = $"{HttpKanali(satir)} {(restoSepetLogu ? "RestoSepet " : "")}{HttpHataAciklamasi(kod)} ({kod}{(metin == "" ? "" : " " + metin)})";
                        DonemeEkle(olay, SatirZamani());
                        continue;
                    }
                }

                // -----------------------------------------------------
                // KURYE KURALLARI (my_kurye)
                // -----------------------------------------------------
                if (kuryeLogu)
                {
                    if (satir.Contains("Daha Önce Gönderilmiş"))
                    {
                        DonemeEkle("Sipariş Daha Önce Gönderilmiş", SatirZamani());
                        continue;
                    }

                    // "Sipariş Güncellemesi Hata! Cannot open file "...my_kurye_20260930.txt". Dosya başka bir işlem tarafından kullanıldığından ..."
                    // Log dosyası başka bir programda açık; o programı kapatmak yeterli
                    if (satir.Contains("Sipariş Güncellemesi Hata!") && satir.Contains("Cannot open file"))
                    {
                        DonemeEkle("Sipariş güncellenemedi (log dosyası kullanımda)", SatirZamani());
                        continue;
                    }

                    // "SipId: o477-2640-yh7c ... No'lu DiğerKurye Siparişi Bulunamadı!" → hangi sipariş olduğu da yazılır
                    if (satir.Contains("Siparişi Bulunamadı"))
                    {
                        var sipId = Regex.Match(satir, @"SipId:\s*(\S+)");
                        DonemeEkle(sipId.Success ? $"Sipariş Bulunamadı ({sipId.Groups[1].Value})" : "Sipariş Bulunamadı", SatirZamani());
                        continue;
                    }
                }

                // -----------------------------------------------------
                // ÖKC KURALLARI (TsmLclSrv)
                // -----------------------------------------------------
                if (okcLogu)
                {
                    if (satir.Contains("DataBase Hata : unavailable database"))
                    {
                        DonemeEkle("ÖKC servisi veritabanına bağlanamadı", SatirZamani());
                        continue;
                    }

                    // "Ingenico Giden Veri MessageType:1, AdisyonNo:25, Result : {"ResultCode":2,"ResultMessage":"Adisyon bulunamadı"}"
                    // ResultCode 0 başarılı; diğerleri Ingenico'nun kendi mesajıyla yazılır
                    var sonuc = Regex.Match(satir, @"^Ingenico Giden Veri .*?AdisyonNo:\s*(\d+).*?Result : (\{.*\})");
                    if (sonuc.Success)
                    {
                        try
                        {
                            var cevap = JObject.Parse(sonuc.Groups[2].Value);
                            int kod = cevap.Value<int?>("ResultCode") ?? 0;
                            if (kod != 0)
                            {
                                string mesaj = cevap.Value<string>("ResultMessage");
                                if (string.IsNullOrWhiteSpace(mesaj)) mesaj = $"Hata kodu {kod}";
                                DonemeEkle($"ÖKC: {mesaj.Trim()} (Adisyon No {sonuc.Groups[1].Value})", SatirZamani());
                            }
                        }
                        catch (JsonException)
                        {
                            // Cevap JSON değilse atlanır
                        }
                        continue;
                    }
                }

                // -----------------------------------------------------
                // RESTOSEPET KURALLARI
                // -----------------------------------------------------
                if (restoSepetLogu)
                {
                    // Trendyol satırlarında zaman bir sonraki satırda " : 27.09.2026 12:13:47" olarak geliyor
                    DateTime rsTarih = ExtractDate(satir);
                    if (rsTarih == DateTime.MinValue && i + 1 < satirlar.Length && satirlar[i + 1].TrimStart().StartsWith(":"))
                        rsTarih = ExtractDate(satirlar[i + 1]);
                    if (rsTarih != DateTime.MinValue)
                        sonLogZamani = rsTarih;

                    // "DataPath : C:\...", "D:\..." ya da "192.168.1.5:D:\..." → RestoSepet açılışı
                    if (satir.StartsWith("DataPath :"))
                    {
                        rsRestartCount++;
                        RaporaEkle(rsTarih, "Restosepet kapanıp açılmış", "");
                        continue;
                    }

                    // "Trendyol Otomatik Ürün Güncelleştirme İşleminde Hata : Access violation ..." → kanal adı satırın başındaki kısım
                    if (satir.Contains("Otomatik Ürün Güncelleştirme İşleminde Hata"))
                    {
                        string kanal = satir.Split(new[] { " Otomatik Ürün" }, 2, StringSplitOptions.None)[0].Trim();
                        DonemeEkle($"{kanal} ürün güncellemesi yapılamadı", rsTarih);
                        continue;
                    }

                    // "Çoklu Istek Hata : Access violation at address ... in module 'RestoSepet.exe'" → program iç hatası, genelde yeniden başlatma gerekir
                    if (satir.Contains("Istek Hata") && satir.Contains("Access violation"))
                    {
                        DonemeEkle("RestoSepet program hatası (Access violation)", rsTarih);
                        continue;
                    }

                    // "Istek Hata : '' is not a valid integer value" → platformdan gelen veride boş sayı alanı
                    if (satir.Contains("Istek Hata") && satir.Contains("is not a valid integer value"))
                    {
                        DonemeEkle("RestoSepet veri hatası (geçersiz sayı)", rsTarih);
                        continue;
                    }
                }

                // -----------------------------------------------------
                // Uygulama Versiyon Kontrol Hata Tespiti (Çok Satırlı ve Esnek Tarih Desteği)
                // -----------------------------------------------------
                if (satir.Contains("Uygulama Versiyon Kontrol Hata"))
                {
                    rsVersionErrorCount++;
                    
                    // Önce mevcut satırda tarih araması yapalım
                    var dateMatch = Regex.Match(satir, @"(\d{1,2}[./]\d{1,2}[./]\d{4}\s\d{2}:\d{2}:\d{2})");
                    string hataZamani = "";
                    
                    if (dateMatch.Success)
                    {
                        hataZamani = dateMatch.Groups[1].Value;
                    }
                    else
                    {
                        // Eğer bu satırda tarih yoksa, bir sonraki satıra "ileriye bak" (peek) yapalım
                        if (i + 1 < satirlar.Length)
                        {
                            string sonrakiSatir = satirlar[i + 1];
                            var nextLineDateMatch = Regex.Match(sonrakiSatir, @"(\d{1,2}[./]\d{1,2}[./]\d{4}\s\d{2}:\d{2}:\d{2})");
                            if (nextLineDateMatch.Success)
                            {
                                hataZamani = nextLineDateMatch.Groups[1].Value;
                                // Not: i++ yapmıyoruz çünkü sonraki satırda başka bir hata veya bilgi de olabilir 
                                // (RestoSepet loglarında genellikle sonraki satır sadece mesajın devamı + tarihtir)
                            }
                        }
                    }

                    RaporaEkle(ExtractDate(hataZamani), "Uygulama Versiyon Kontrol Hata", "");
                    continue; 
                }

                DateTime tarih = ExtractDate(satir);
                if (tarih == DateTime.MinValue)
                {
                    // Diğer özel log tipleri için (Kurye vb) tarih araması
                    if (satir.Contains("Kurye Dll Donen : Hata : Bilinen böyle bir ana bilgisayar yok."))
                    {
                        rsCourierErrorCount++;
                        var match = Regex.Match(satir, @"(\d{1,2}[./]\d{1,2}[./]\d{4}\s\d{2}:\d{2}:\d{2})");
                        string kuryeZaman = match.Success ? match.Groups[1].Value : "";
                        RaporaEkle(ExtractDate(kuryeZaman), "Kurye sunucusuna ulaşılamadı", "");
                    }

                    continue;
                }

                // Normal akış (Tarih başarıyla ayıklandıysa)
                sonLogZamani = tarih;

                // -----------------------------------------------------
                // Kurye Sunucu Hatası
                // -----------------------------------------------------
                if (satir.Contains("Kurye Dll Donen : Hata : Bilinen böyle bir ana bilgisayar yok."))
                {
                    rsCourierErrorCount++;
                    RaporaEkle(tarih, "Kurye sunucusuna ulaşılamadı", "");
                }




             




                // -----------------------------------------------------
                // 2) CONNECTION FAILED (HATA BAŞLANGICI)
                // -----------------------------------------------------
                // -----------------------------------------------------
                // 1) ADİSYON KAYIT HATALARI (dönem olarak gruplanır)
                // -----------------------------------------------------
                string tekrarlayanHata = TekrarlayanRcguardHatasi(satir);
                if (tekrarlayanHata != null)
                {
                    DonemeEkle(tekrarlayanHata, tarih);
                    continue;
                }

                bool baglantiError =
                    satir.Contains("Istek Hata : A connection attempt failed") ||
                    satir.Contains("connected host has failed to respond") ||
                    satir.Contains("Istek Hata : An established connection was aborted");
                
                if (baglantiError)
                {
                    if (!baglantiHataAktif)
                    {
                        hataSayisi4++;

                        baglantiHataAktif = true;
                        baglantiHataBaslangic = tarih;

                        RaporaEkle(tarih, "Servera Ulaşılamadı – Bağlantı Hatası Başladı", "");
                    }

                    httpsSeen2 = false;
                    gelenVeriSeen2 = false;
                    successPairCount2 = 0;

                    continue;
                }


                // BAŞARI SATIRLARI
                if (baglantiHataAktif)
                {
                    if (satir.StartsWith("https://"))
                    {
                        httpsSeen2 = true;
                        continue;
                    }

                    if (satir.Contains("Gelen Veri : []") || satir.Contains("Gelen Veri : [{"))
                    {
                        gelenVeriSeen2 = true;
                    }

                    // HTTPS → Gelen Veri çifti yakalandı
                    if (httpsSeen2 && gelenVeriSeen2)
                    {
                        successPairCount2++;

                        httpsSeen2 = false;
                        gelenVeriSeen2 = false;
                    }

                    // 2 çift → hata bitti
                    if (successPairCount2 >= 2)
                    {
                        TimeSpan fark = tarih - baglantiHataBaslangic.Value;
                        string sure = SureMetni(fark);

                        RaporaEkle(tarih, "Servera Ulaşıldı – Bağlantı Hatası Bitti", sure);

                        baglantiHataAktif = false;
                        baglantiHataBaslangic = null;
                        successPairCount2 = 0;
                        httpsSeen2 = false;
                        gelenVeriSeen2 = false;
                    }
                }




                // -----------------------------------------------------
                // 3) NO SUCH HOST (DNS HATASI)
                // -----------------------------------------------------

                bool hostError =
                 satir.Contains("Istek Hata : No such host is known") ||
                 satir.Contains("Istek Hata : Bilinen böyle bir ana bilgisayar yok");
                
                if (hostError)
                {
                    

                    if (!hostHataAktif)
                    {
                        hatasayisi3++;

                        hostHataAktif = true;
                        hostHataBaslangic = tarih;

                        RaporaEkle(tarih, "Internetinizin Bağlantısı Koptu", "");
                    }

                    // hata gelince başarı algılayıcıları sıfırlanır
                    httpsSeen = false;
                    gelenVeriSeen = false;
                    successPairCount = 0;

                    continue;
                }

                // BAŞARI SATIRLARI
                if (hostHataAktif)
                {
                    if (satir.StartsWith("https://"))
                    {
                        httpsSeen = true;
                        continue;
                    }

                    if (satir.Contains("Gelen Veri : []") || satir.Contains("Gelen Veri : [{"))
                    {
                        gelenVeriSeen = true;
                    }

                    // HTTPS → Gelen Veri çifti tamamlandığında
                    if (httpsSeen && gelenVeriSeen)
                    {
                        successPairCount++;

                        httpsSeen = false;
                        gelenVeriSeen = false;
                    }

                    // 2 çift olunca düzelmiş kabul et
                    if (successPairCount >= 2)
                    {
                        TimeSpan fark = tarih - hostHataBaslangic.Value;
                        string sure = SureMetni(fark);

                        RaporaEkle(tarih, "Internetiniz Düzeldi", sure);

                        // reset
                        hostHataAktif = false;
                        hostHataBaslangic = null;
                        successPairCount = 0;
                        httpsSeen = false;
                        gelenVeriSeen = false;
                    }
                }







                // -----------------------------------------------------
                // 5) DataBase Bağlantısı Koptu
                // -----------------------------------------------------
                if (satir.Contains("DataBase Bağlantısı Koptu! (Disconnect)") ||
                    satir.Contains("DataBase Baglantisi Koptu! (Disconnect)"))

                {
                    hataSayisi++;
                    sonDatabaseKopma = tarih;

                    RaporaEkle(tarih, "DataBase Koptu!", "");
                    continue;
                }

                if (satir.Contains("DataBase Bağlandı. (Connect)") ||
                    satir.Contains("DataBase Baglandi. (Connect)"))

                {
                    DateTime baslamazamani = tarih;
                    string sure = "";


                    if (sonDatabaseKopma != null)
                    {
                        TimeSpan fark = baslamazamani - sonDatabaseKopma.Value;
                        sure = SureMetni(fark);
                    }

                    RaporaEkle(baslamazamani, "Database Bağlandı", sure);

                    sonDatabaseKopma = null;
                    continue;
                }






                // -----------------------------------------------------
                // 6) SERVİS DURDU
                // -----------------------------------------------------
                if (satir.Contains("RcGuard Servis Durdu.(Destroy)") ||
                        satir.Contains("RcGuard Servis Kapandı.(ShutDown)"))
                {
                    hatasayisi2++;
                    sonDurma = tarih; //Servisin durduğu zaman kaydediliyor.eğer sonra servis tekrar başlarsa, bu zaman ile karşılaştırılıp ne kadar süre durduğu hesaplanacak.

                    RaporaEkle(tarih, "Bilgisayar kapandı ya da restocell durdur yapıldı", "");

                    continue;
                }

                // -----------------------------------------------------
                // 7) SERVİS BAŞLADI
                // -----------------------------------------------------
                if (satir.Contains("RcGuard Servis Başladı.(Start)") ||
                    satir.Contains("RcGuard Servis Baþladý.(Start)") ||
                    satir.Contains("RcGuard Servis Basladi.(Start)"))
                {
                    DateTime baslamaZamani = tarih; //Servisin başladığı an kaydediliyor.
                    string sure = "";

                    if (sonDurma != null) //daha önce bir durma zamanı kaydedildiyse 
                    {
                        TimeSpan fark = baslamaZamani - sonDurma.Value; //başlama zamanı ile durma zamanı arasındaki fark hesaplanıyor.
                        sure = SureMetni(fark); //fark dakika ve saniye cinsinden formatlanıyor.
                    }

                    RaporaEkle(baslamaZamani, "Bilgisayar açıldı ya da restocell başlat yapıldı", sure);

                    sonDurma = null;
                    continue;
                }
            }



            // Log okuma tamamlandıktan sonra

            foreach (string olay in acikDonemler.Keys.ToList())
                DonemiKapat(olay);

            // Log bitene kadar düzelmeyen hatalar (zaman olarak logdaki son satırın zamanı yazılır)
            if (sonLogZamani is DateTime logSonu)
            {
                if (baglantiHataAktif && baglantiHataBaslangic != null)
                    RaporaEkle(logSonu, "Servera Ulaşılamadı – log sonuna kadar düzelmedi", $"{SureMetni(logSonu - baglantiHataBaslangic.Value)} (devam ediyor)");
                if (hostHataAktif && hostHataBaslangic != null)
                    RaporaEkle(logSonu, "Internet bağlantısı log sonuna kadar düzelmedi", $"{SureMetni(logSonu - hostHataBaslangic.Value)} (devam ediyor)");
                if (sonDatabaseKopma != null)
                    RaporaEkle(logSonu, "DataBase log sonuna kadar bağlanmadı", $"{SureMetni(logSonu - sonDatabaseKopma.Value)} (devam ediyor)");
                if (sonDurma != null)
                    RaporaEkle(logSonu, "Servis log sonuna kadar başlatılmadı", $"{SureMetni(logSonu - sonDurma.Value)} (devam ediyor)");
            }

            if (raporSatirlari.Count == 0)
                datagridhataraporu.Rows.Add("Hata bulunamadı", "", "");

            foreach (var r in raporSatirlari.OrderBy(r => r.Item1))
            {
                int satir = datagridhataraporu.Rows.Add(r.Item2, r.Item1 == DateTime.MinValue ? "" : r.Item1.ToString("dd.MM.yyyy HH:mm:ss"), r.Item3);
                VurguRengi(datagridhataraporu.Rows[satir], HataVurgusu(r.Item2, r.Item3));
            }

        





            dataGridToplamHatalarıGoster.Columns.Clear();
            dataGridToplamHatalarıGoster.Rows.Clear();

            dataGridToplamHatalarıGoster.Columns.Add("Olay", "Olay");
            dataGridToplamHatalarıGoster.Columns.Add("Sayı", "Hata Sayısı");
            KolonlariTabloyaYay(dataGridToplamHatalarıGoster, 4, 1);

            if (restoSepetLogu)
            {
                // RestoSepet Toplamları
                dataGridToplamHatalarıGoster.Rows.Add("RestoSepet Versiyon Kontrol Hata Sayısı:", rsVersionErrorCount);
                dataGridToplamHatalarıGoster.Rows.Add("Kurye Sunucusuna Ulaşılamadı Sayısı:", rsCourierErrorCount);
                dataGridToplamHatalarıGoster.Rows.Add("RestoSepet Yeniden Başlatılma Sayısı:", rsRestartCount);

                foreach (var h in hataSayilari)
                    dataGridToplamHatalarıGoster.Rows.Add(h.Key, h.Value);
            }
            else if (secilen.Equals("RCGuard", StringComparison.OrdinalIgnoreCase))
            {
                dataGridToplamHatalarıGoster.Rows.Add("Toplam 'DataBase Bağlantısı Koptu' hatası sayısı:", hataSayisi);
                dataGridToplamHatalarıGoster.Rows.Add("Toplam 'Bilgisayar Kapandı ya da Restocell Durduruldu' hatası sayısı:", hatasayisi2);
                dataGridToplamHatalarıGoster.Rows.Add("Internet Hatası sayısı", hatasayisi3);
                dataGridToplamHatalarıGoster.Rows.Add("Servera Ulaşılamadı – Bağlantı Hatası sayısı", hataSayisi4);

                foreach (var h in hataSayilari)
                    dataGridToplamHatalarıGoster.Rows.Add(h.Key, h.Value);
            }
            else
            {
                // Kurye ve ÖKC loglarında RCGuard'a özel sayılar yanıltıcı olurdu; sadece bulunan hatalar sayılır
                foreach (var h in hataSayilari)
                    dataGridToplamHatalarıGoster.Rows.Add(h.Key, h.Value);

                if (hataSayilari.Count == 0)
                    dataGridToplamHatalarıGoster.Rows.Add("Hata bulunamadı", 0);
            }

        }



        private DateTime ExtractDate(string satir)  //Yalnızca satırın sonunda bulunan tarih-saatleri kabul eder
        {
            if (string.IsNullOrWhiteSpace(satir))
                return DateTime.MinValue;

            // Sadece satır sonundaki tarihi yakalar
            var match = Regex.Match(satir, @"(\d{1,2}[./]\d{1,2}[./]\d{4}\s\d{2}:\d{2}:\d{2})\s*$");

            if (!match.Success)  //Başarısızsa çık
                return DateTime.MinValue;

            string tarihStr = match.Groups[1].Value;

            string[] formats =
            {
                "dd.MM.yyyy HH:mm:ss",
                "d.M.yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "d/M/yyyy HH:mm:ss"
            };

            if (DateTime.TryParseExact(tarihStr, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime tarih))  // parse et
            {
                return tarih;
            }

            return DateTime.MinValue;
        }




        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

        }

        private void datagridrcguarddetay_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void datagridrcguard_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridToplamHatalarıGoster_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    }

    





