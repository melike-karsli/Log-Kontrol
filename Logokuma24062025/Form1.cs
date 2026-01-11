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



namespace Logokuma24062025
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DosyalariYukle();
        }


        private void DosyalariYukle()
        {


            string[] muhtemelKlasorler =
                {
                    @"C:\RestoPOS\MY\LOG\",
                    @"D:\RestoPOS\MY\LOG\"
                };

            // Bu değişken log klasörünü tutacak
            string klasorYolu = null;

            // C ve D içinde sırayla ara
            foreach (string yol in muhtemelKlasorler)
            {
                if (Directory.Exists(yol))
                {
                    klasorYolu = yol;
                    break;
                }
            }


            string[] dosyalar = Directory.GetFiles(klasorYolu, "*.txt"); //Eğer klasör varsa, .txt uzantılı tüm dosyalar dosyalar dizisine alınır.

            foreach (string dosyaYolu in dosyalar) //Her bir .txt dosyası için işlem yapılır.
            {
                string dosyaAdi = Path.GetFileName(dosyaYolu); //Dosyanın tam yolu içinden sadece adı alınır.Örn: Ankara_log1.txt

                string[] parcalar = dosyaAdi.Split('_'); //Dosya adı _ karakteriyle parçalanır

                if (parcalar.Length > 0)
                {
                    string ilkParca = parcalar[0]; //Parçalama sonucu en az bir parça varsa, ilk parça alınır 

                    if (!kaynak_combo.Items.Contains(ilkParca)) //Eğer bu ilk parça ComboBox'ta (yani kaynak_combo) yoksa, listeye eklenir. Böylece her kaynak adı bir kez görünür.
                    {
                        kaynak_combo.Items.Add(ilkParca);
                    }
                }
            }


        }

        string[] satirlar;
        string DosyaTarihi;

        private void kaynak_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string secilenAd = kaynak_combo.SelectedItem.ToString();

            // Önce C ve D sürücüsünü sırayla dene
            string[] muhtemelKlasorler =
            {
                @"C:\RestoPOS\MY\LOG\",
                @"D:\RestoPOS\MY\LOG\"
            };

            string klasorYolu = null;

            foreach (string yol in muhtemelKlasorler)
            {
                if (Directory.Exists(yol))
                {
                    klasorYolu = yol;
                    break;
                }
            }

            if (klasorYolu == null)
            {
                MessageBox.Show("LOG klasörü C: veya D: sürücüsünde bulunamadı.");
                return;
            }

            // 🔍 Seçilen kaynak adına göre dosyaları getir
            string[] dosyalar = Directory.GetFiles(klasorYolu, $"{secilenAd}_*.txt");

            // Eğer hiç dosya yoksa uyar
            if (dosyalar.Length == 0)
            {
                MessageBox.Show($"{secilenAd} için log dosyası bulunamadı.");
                return;
            }

            // Tarih seçimini al
            DosyaTarihi = tarihsec.Value.ToString("yyyyMMdd");


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


        //TSM
        private void LogTSM(string json)
        {


            if (datagridrcguarddetay.Columns.Count == 0)
            {
                datagridrcguarddetay.Columns.Add("AdisyonNo", "Adisyon No");
                datagridrcguarddetay.Columns.Add("SerialNo", "Serial No");
                datagridrcguarddetay.Columns.Add("EkuNo", "EKU No");
                datagridrcguarddetay.Columns.Add("ZNo", "Z No");
                datagridrcguarddetay.Columns.Add("ReceiptNo", "Receipt No");
                datagridrcguarddetay.Columns.Add("TransDateTime", "Tarih");
                datagridrcguarddetay.Columns.Add("PaymentTypeEx" + ":" + "PaymentAmount", "Odeme Bilgisi");
            }

            try
            {
                logokuclass.IngenicoData veri = JsonConvert.DeserializeObject<logokuclass.IngenicoData>(json);

                // Veriyi DataGridView'e ekle

                // Örnek: PaymentList içindeki tüm öğeleri yazdırma


                //string odemeler = "";
                string odemetipi = "";
                string banka = ""; // Banka kodunu tutacak değişken 
                string odemeText = ""; // Ödeme bilgilerini tutacak string

                if (veri?.Receipt?.PaymentList != null) //veri null değilse, Receipt null değilse,PaymentList null değilse, o zaman if bloğuna giriyor.

                {

                    for (int i = 0; i < veri.Receipt.PaymentList.Count; i++)  //PaymentList içinde kaç ödeme varsa (Count kadar), her birini i indeksiyle teker teker geziyor.
                                                                              //  Yani örnek olarak:PaymentList[0] → 1.ödeme PaymentList[1] → 2.ödeme
                    {

                        odemetipi = veri.Receipt?.PaymentList[i].PaymentTypeEx.ToString() ?? ""; // PaymentTypeEx değerini kullanarak ödeme tipini alır

                        banka = veri.Receipt?.PaymentList[i].BankBKMID.ToString() ?? ""; // Banka kodunu alır

                        // Merchantid artık object olduğu için güvenli şekilde stringe çeviriyoruz
                        string merchantId = veri.Receipt.PaymentList[i].Merchantid?.ToString() ?? "";

                        if (odemetipi == "1" || odemetipi == "512" || odemetipi == "2048")
                            odemeText += fnc_IngenicoOdemeTip(odemetipi);
                        else
                            odemeText += fnc_IngenicoBankaKod(banka);

                        odemeText += ":" + ((veri.Receipt?.PaymentList?[i]?.PaymentAmount ?? 0) / 100) + "$"; //PaymentAmount genelde kuruş cinsinden geldiği için /100 yapılıyor → TL’ye çevrilir. Null ise 0 alınır.  Sonuç "OdemeTipi:Banka:50$" gibi bir metne dönüşür.

                    }
                    odemeText = odemeText.TrimEnd('$'); // Sonundaki $ işaretini kaldırır

                    datagridrcguarddetay.Rows.Add

                    (
                        veri.AdisyonNo.ToString() ?? "",
                        veri.SerialNo.Trim(),
                        veri.Receipt.EkuNo,
                        veri.Receipt.ZNo,
                        veri.Receipt.ReceiptNo,
                        veri.Receipt.TransDateTime,
                        odemeText


                     );

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("JSON işleme hatası: " + ex.Message);
            }
        }

        //TRENDYOL
        private void RestoSepetTrendyol(string json)
        {
            if (datagridrcguard.Columns.Count == 0)
            {
                datagridrcguard.Rows.Clear();
                datagridrcguard.Columns.Clear();
                datagridrcguard.Columns.Add("kanal", "Satış kanalı");
                datagridrcguard.Columns.Add("id", "Sipariş No");
                datagridrcguard.Columns.Add("totalPrice", "Toplam Tutar");
                datagridrcguard.Columns.Add("totalSellerAmount", "Toplam İndirim");
                datagridrcguard.Columns.Add("callCenterPhone", "İletişim No");
                datagridrcguard.Columns.Add("Name", "Müşteri");
                datagridrcguard.Columns.Add("paymentType", "Ödeme Tipi");
                //   datagridrestosepet.Columns.Add("address", "Adres");
                datagridrcguard.Columns.Add("name", "Ana Ürün");
                datagridrcguard.Columns.Add("modifierNamesStr", "İçindekiler");
            }

            var veri1 = JsonConvert.DeserializeObject<logokurestosepetclass.trendyollog>(json); //json isimli stringi, sizin tanımladığınız trendyollog class’ına çeviriyor.

            if (veri1?.content == null) return; //Eğer veri1 boşsa veya content listesi yoksa, fonksiyon duruyor.


            foreach (var contentItem in veri1.content) //JSON içindeki content listesi üzerinde tek tek dönüyor. //contentItem oluşturduk listeyi buraya atadık
            {
                var lines = contentItem?.lines ?? new List<logokurestosepetclass.lines>();  //Her contentItem'ın lines adlı bir listesi var.
                                                                                            //Eğer lines null ise boş bir liste (new List<...>()) oluşturuyor.
                                                                                            //Böylece lines null olsa da foreach patlamıyor.

                foreach (var line in lines)
                {
                    // Modifier ürünler // entity framework kullanarak yazdık
                    var modifierNames = line?.modifierProducts? //line değişkenin varsa (null değilse) → onun içindeki modifierProducts listesini al.
                                            .Select(m => m?.name ?? "") //modifierProducts listesindeki her bir m için:Eğer m null değilse → m.name al.Eğer m ya da m.name null ise → boş string "" döndür.
                                            .ToList() ?? new List<string>(); // .ToList() ?? new List<string>()Select sonucu bir listeye(List<string>) çevriliyor.Eğer yukarıdaki işlemlerin tamamı null dönerse, boş bir liste(new List<string>()) oluştur.

                    string modifierNamesStr = string.Join(", ", modifierNames); //string modifierNamesStr = string.Join(", ", modifierNames); Listeyi alıp string’e çeviriyorsun.Örn: ["Peynir", "Zeytin", "Domates"] → "Peynir, Zeytin, Domates".Eğer liste boşsa → sonuç ""(boş string) olur.

                    // Promotions toplam
                    string totalSellerAmount = contentItem?.promotions?.FirstOrDefault()?.totalSellerAmount.ToString() ?? "0";



                    // aynı sipariş varsa ekleme

                    bool zatenVar = datagridrcguard.Rows
                  .Cast<DataGridViewRow>()
                  .Any(r => r.Cells["id"].Value?.ToString() == contentItem.id.ToString());

                    if (zatenVar) continue;




                    // Satırı DataGridView'e ekle
                    datagridrcguard.Rows.Add(


                        "Trendyol",  // Sabit değer

                        contentItem?.id ?? "",

                        (contentItem?.totalPrice != null ? contentItem.totalPrice.ToString("F0") : "0"),

                        totalSellerAmount,

                        contentItem?.callCenterPhone ?? "",

                        $"{contentItem?.customer?.firstName ?? ""} {contentItem?.customer?.lastName ?? ""} ",

                        contentItem?.payment?.paymentType ?? "",

                        line?.name ?? "",

                        modifierNamesStr
                    );
                }
            }
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



        //YEMEKSEPETİ
        private void Restosepetyemeksepeti(string json)
        {
            if (datagridrcguard.Columns.Count == 0)
            {
                datagridrcguard.Rows.Clear();
                datagridrcguard.Columns.Clear();

                datagridrcguard.Columns.Add("kanal", "Satış kanalı");
                datagridrcguard.Columns.Add("code", "Sipariş Kodu");
                datagridrcguard.Columns.Add("grandTotal", "Toplam Tutar");
                datagridrcguard.Columns.Add("discounts", "Toplam indirim");
                datagridrcguard.Columns.Add("mobilePhone", "Müşteri Telefon");
                datagridrcguard.Columns.Add("firstname", "Müşteri Adı");
                datagridrcguard.Columns.Add("paymentType", "Ödeme Tipi");
                datagridrcguard.Columns.Add("createdAt", "Oluşturulma Tarihi");
                datagridrcguard.Columns.Add("platform", "Platform");
                datagridrcguard.Columns.Add("products", "Ürünler");

            }
            var veri3 = JsonConvert.DeserializeObject<yemeksepeti.yemeksepetilog>(json);
            if (veri3?.products == null) return;

            foreach (var product in veri3.products)
            {
                // Ürün bilgilerini birleştir

                string urunBilgisi = string.Join(", ",
                veri3.products.Select(p => $"{p.name} (Adet: {p.quantity})"));

                string toppings = product.selectedToppings != null
                 ? string.Join(", ", product.selectedToppings.Select(t => t.name)) //Select(t => t.name) → sadece adlarını alır string.Join(", ", ...) → virgülle birleştirir
                 : "";

                string adres = veri3.delivery?.address != null
                ? $"{veri3.delivery.address.city ?? ""} {veri3.delivery.address.deliveryMainArea ?? ""} {veri3.delivery.address.street ?? ""} {veri3.delivery.address.number ?? ""}"
                   : "";



                // Satırı DataGridView'e ekle
                datagridrcguard.Rows.Add(

                    "Yemeksepeti",  // Sabit değer

                    veri3.platformRestaurant.id ?? "",

                    veri3.price?.grandTotal ?? "",

                    veri3.discounts.Count.ToString(), // Toplam indirim sayısı

                    veri3.customer?.mobilePhone ?? "",

                    $"{veri3.customer?.firstname ?? ""} {veri3.customer?.lastname ?? ""}",

                    veri3.payment?.type ?? "",

                    // adres,

                    urunBilgisi,

                    toppings



                );
            }
        }


        //MİGROS
        private void Restosepetmigros(string json)
        {
            if (datagridrcguard.Columns.Count == 0)
            {
                datagridrcguard.Rows.Clear();
                datagridrcguard.Columns.Clear();

                datagridrcguard.Columns.Add("kanal", "Satış kanalı");
                datagridrcguard.Columns.Add("id", "Sipariş No");
                datagridrcguard.Columns.Add("totalPrice", "Toplam Tutar");
                datagridrcguard.Columns.Add("discountedPrice", "Toplam İndirim");
                datagridrcguard.Columns.Add("phoneNumber", "İletişim No");
                datagridrcguard.Columns.Add("customerFullName", "Müşteri");
                datagridrcguard.Columns.Add("paymentType", "Ödeme Tipi");
                // datagridrestosepet.Columns.Add("address", "Adres");
                datagridrcguard.Columns.Add("name", "Ana Ürün");
                datagridrcguard.Columns.Add("options", "İçindekiler");
            }

            var veri4 = JsonConvert.DeserializeObject<migros.migroslog>(json);

            if (veri4?.data == null) return;

            // data içinde dön
            foreach (var store in veri4.data)
            {
                // pendingOrderDetailsDTOS içinde dön
                foreach (var order in store.pendingOrderDetailsDTOS)
                {
                    if (order.products == null) continue;


                    string urunbilgisi = string.Join(", ",

                        order.products.Select(p =>
                        p.options != null && p.options.Any()
                            ? string.Join("; ", p.options.Select(o => o.itemNames))
                            : ""
                        )

                    );


                    foreach (var product in order.products)
                    {
                        datagridrcguard.Rows.Add
                        (
                            "Migros",  // Sabit değer

                            order.id,

                            order.totalPrice,

                            order.primaryDiscountedPriceText,

                            order.phoneNumber,

                            order.customerFullName,

                            order.paymentType,

                            //order.address,

                            product?.name ?? "",

                            urunbilgisi
                        );
                    }
                }
            }
        }

        // Form seviyesinde (global)
        private List<rcguard.rcguardlog> veriListesi = new List<rcguard.rcguardlog>();









        private void Rcguard(string json)
        {
            // Kolonlar (sadece ilk seferde ekle)
            if (datagridrcguard.Columns.Count == 0)
            {
                datagridrcguard.Rows.Clear();
                datagridrcguard.Columns.Clear();
                datagridrcguard.Columns.Add("orderSource", "Satış Kanalı");
                datagridrcguard.Columns.Add("orderTypeText", "Satış Tipi");
                datagridrcguard.Columns.Add("orderid", "Sipariş No");
                datagridrcguard.Columns.Add("orderDate", "Sipariş Tarihi");
            }


            //JSON bir dizi (array) ise her elemanı tek tek işliyor. JSON tek bir obje ise onu işliyor. JSON geçersiz ya da beklenmeyen türde ise işlem yapmıyor.
            //Sonrasında her JSON objesini rcguard.rcguardlog modeline dönüştürüp AddIfNotExists() metoduna gönderiyor.

            if (string.IsNullOrWhiteSpace(json)) return;
            try
            {
                var token = JToken.Parse(json);

                if (token is JArray arr)
                {
                    foreach (var item in arr)
                    {
                        // JObject -> model'e dönüştür
                        var veri = item.ToObject<rcguard.rcguardlog>(); //Her eleman rcguardlog class'ına dönüştürülüyor.
                        AddIfNotExists(veri, item.ToString());
                    }
                }
                else if (token is JObject obj)
                {
                    var veri = obj.ToObject<rcguard.rcguardlog>(); //JSON tek obje ise ({...}) direkt model’e çeviriliyor.
                    AddIfNotExists(veri, obj.ToString());
                }
                else
                {
                    //JSON string, sayı, boolean vs. ise işlem yapma.
                    return;
                }
            }
            catch (JsonReaderException jex) // JSON bozuksa veya beklenmeyen hata olursa debug’a yazılıyor.
            {
                Debug.WriteLine("JSON parse error in Rcguard: " + jex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Rcguard unexpected error: " + ex.Message);
            }
        }

        private void AddIfNotExists(rcguard.rcguardlog veri, string rawJson) //AddIfNotExists() ile tekrarlayan veri eklemeyi engeller
        {
            if (veri == null) return;
            if (string.IsNullOrEmpty(veri.orderid)) return;  //orderid yoksa işlem yapılmaz.

            // Eğer zaten sözlükte varsa tekrar ekleme
            if (siparisJsonlari.ContainsKey(veri.orderid)) return;

            // Sözlüğe kaydet (detay için saklıyoruz)
            siparisJsonlari[veri.orderid] = rawJson;

            // DataGrid'de aynı orderid var mı kontrol et
            bool zatenVar = datagridrcguard.Rows
                .Cast<DataGridViewRow>()
                .Any(r => r.Cells["orderid"].Value?.ToString() == veri.orderid);

            if (zatenVar) return;

            // Satırı ekle
            datagridrcguard.Rows.Add(
                veri.orderSource ?? "",
                veri.orderTypeText ?? "",
                veri.orderid ?? "",
                veri.orderDate ?? ""
            );
        }


        private void Rcguarddetay(string json)
        {
            if (datagridrcguarddetay.Columns.Count == 0)
            {
                datagridrcguarddetay.Columns.Clear();
                datagridrcguarddetay.Columns.Add("client", "Müşteri Adı");
                datagridrcguarddetay.Columns.Add("menu", "Ürün Adı");
                datagridrcguarddetay.Columns.Add("product", "Ürün Detayı");
                datagridrcguarddetay.Columns.Add("price", "Ürün Tutarı");
                datagridrcguarddetay.Columns.Add("payments", "Toplam Tutar");
            }

            datagridrcguarddetay.Rows.Clear();

            List<rcguard.rcguardlog> siparisler;

            try
            {
                if (json.TrimStart().StartsWith("["))
                    siparisler = JsonConvert.DeserializeObject<List<rcguard.rcguardlog>>(json);
                else
                    siparisler = new List<rcguard.rcguardlog>
            {
                JsonConvert.DeserializeObject<rcguard.rcguardlog>(json)
            };
            }
            catch (Exception ex)
            {
                MessageBox.Show("JSON parse hatası: " + ex.Message);
                return;
            }

            foreach (var s in siparisler)
            {
                string toplamTutar = s.payments?.Count > 0 ? s.payments[0].paymentAmount : "";

                // 🔥 MENÜLERİ TEK TEK EKLEYELİM
                if (s.menus != null && s.menus.Count > 0)
                {
                    foreach (var menu in s.menus)  // ←←← **TÜM MENÜLER**
                    {

                        decimal fiyat = decimal.Parse(menu.price.ToString(), CultureInfo.InvariantCulture);
                        string menuAd = menu.name;
                        string urunler = "";

                        if (menu.products != null && menu.products.Count > 0)
                            urunler = string.Join(", ", menu.products.Select(p => p.name));

                        datagridrcguarddetay.Rows.Add(
                            s.client?.name ?? "",
                            menuAd,
                            urunler,
                            fiyat,
                            toplamTutar
                        );
                    }
                }

                // 🔥 Eğer menus yoksa products gösterilir
                else if (s.products != null && s.products.Count > 0)
                {
                    decimal fiyat = s.products.Sum(p =>
                        Convert.ToDecimal(p.price) * Convert.ToDecimal(p.quantity));

                    string urunler = string.Join(", ", s.products.Select(p => p.name));

                    datagridrcguarddetay.Rows.Add(
                        s.client?.name ?? "",
                        "",
                        urunler,
                        fiyat,
                        toplamTutar
                    );
                }
            }
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
            siparisJsonlari.Clear();



            // Kullanılabilecek klasör yolları
            string[] muhtemelKlasorler =
            {
                @"C:\RestoPOS\MY\LOG\",
                @"D:\RestoPOS\MY\LOG\"
            };

            string klasorYolu = null;

            // C ve D sürücüsünde klasörü ara
            foreach (string yol in muhtemelKlasorler)
            {
                if (Directory.Exists(yol))
                {
                    klasorYolu = yol;
                    break;
                }
            }

            if (klasorYolu == null)
            {
                MessageBox.Show("LOG klasörü C: veya D: sürücüsünde bulunamadı.");
                return;
            }







            string secilen = kaynak_combo.SelectedItem?.ToString();

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

            string[] satirlar = File.ReadAllLines(dosyaYolu, Encoding.GetEncoding("windows-1254"));

            foreach (string satir in satirlar)
            {
                string json = ExtractJson(satir);

                if (json == null) continue;

                if (json.Contains("kioskOrderNumber"))
                {
                    Rcguard(json);
                }



                var token = JToken.Parse(json);

                if (token is JArray arr)
                {
                    foreach (var item in arr)
                    {
                        string orderid = item["orderid"]?.ToString();
                        if (string.IsNullOrEmpty(orderid)) continue;
                        if (siparisJsonlari.ContainsKey(orderid)) continue;

                        string itemJson = item.ToString();
                        siparisJsonlari[orderid] = itemJson;

                        datagridrcguard.Rows.Add(orderid, "", itemJson);
                    }
                }



            }


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
        private Dictionary<string, string> siparisJsonlari = new Dictionary<string, string>();
        //sözlük tanımlandı.Key (anahtar) → orderid (her siparişin benzersiz kimliği) Value (değer) → o siparişin JSON formatındaki detay verisi 
        private void datagridrcguard_SelectionChanged(object sender, EventArgs e)
        {
            if (datagridrcguard.SelectedRows.Count == 0) return; //hiç satır seçilmediyse çık

            string secilenOrderId = datagridrcguard.SelectedRows[0].Cells["orderid"].Value?.ToString(); //seçilen satırdaki orderid yi al
            if (string.IsNullOrEmpty(secilenOrderId)) return; //orderid boşsa metot çıkış yapar

            if (siparisJsonlari.TryGetValue(secilenOrderId, out string detayJson))
            //sözlükten json verisini al-siparisJsonlari içinde anahtar olarak secilenOrderId var mı diye bakar-arsa, onun değerini detayJson değişkenine atar.-Yoksa hiçbir şey yapmaz (false döner).
            {
                // sadece burayı çağırıyoruz
                Rcguarddetay(detayJson);
            }
        }




        private void button3_Click(object sender, EventArgs e)
        {

            // Aranacak klasör yolları
            string[] muhtemelKlasorler =
            {
                @"C:\RestoPOS\MY\LOG\",
                @"D:\RestoPOS\MY\LOG\"
            };

            string klasorYolu = null;

            // Mevcut olanı bul
            foreach (string yol in muhtemelKlasorler)
            {
                if (Directory.Exists(yol))
                {
                    klasorYolu = yol;
                    break;
                }
            }

            if (klasorYolu == null)
            {
                MessageBox.Show("LOG klasörü C: veya D: sürücüsünde bulunamadı.");
                return;
            }



            string secilen = kaynak_combo.SelectedItem?.ToString();

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
            //datagridhataraporu.RowHeadersVisible = false;

            if (datagridhataraporu.Columns.Count == 0)
            {
                datagridhataraporu.Columns.Add("Olay", "Olay");
                datagridhataraporu.Columns.Add("Zaman", "Olayın Gerçekleştiği Zaman");
                datagridhataraporu.Columns.Add("Süre", "Durma-Başlama Süresi");
                //datagridhataraporu.Columns.Add("Sayı", "Hata Sayısı");
            }

            string[] satirlar = File.ReadAllLines(dosyaYolu, Encoding.GetEncoding("windows-1254")); //okur, her satırı bir string dizisine koyar.

            DateTime? sonDurma = null; //Servisin durduğu zamanı saklamak için. Başlangıçta null.
            int hataSayisi = 0; //databasedeki Toplam hata sayısını saymak için.
            int hatasayisi2 = 0; //servisteki Toplam hata sayısını saymak için.
            int hatasayisi3 = 0; //Bilinen böyle bir ana bilgisayar yok hatasını saymak için.
            int hataSayisi4 = 0; //Istek Hata : A connection attempt failed because the connected party did not properly respond after a period of time hatasını saymak için.


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




            foreach (string satir in satirlar) // satırlarda dolaşma
            {
                DateTime tarih = ExtractDate(satir);
                if (tarih == DateTime.MinValue)
                    continue;



                // -----------------------------------------------------
                // 1) HER ZAMAN EN ÖNCE HTTPS KONTROLÜ (HATA BİTTİ Mİ?)
                //// -----------------------------------------------------
                //if (baglantiHataAktif && satir.Contains("https://"))
                //{
                //    baglantiHataAktif = false;

                //    TimeSpan fark = tarih - baglantiHataBaslangic.Value;
                //    string sure = $"{(int)fark.TotalMinutes} dk {fark.Seconds} sn";

                //    datagridhataraporu.Rows.Add(
                //        "Servera Ulaşıldı – Bağlantı Hatası Düzeldi",
                //        tarih.ToString("dd.MM.yyyy HH:mm:ss"),
                //        sure
                //    );

                //    continue;   // başka kontrole gerek yok
                //}




                // -----------------------------------------------------
                // 2) CONNECTION FAILED (HATA BAŞLANGICI)
                // -----------------------------------------------------
                bool baglantiError =
                    satir.Contains("Istek Hata : A connection attempt failed") ||
                    satir.Contains("connected host has failed to respond");
                
                if (baglantiError)
                {
                    if (!baglantiHataAktif)
                    {
                        hataSayisi4++;

                        baglantiHataAktif = true;
                        baglantiHataBaslangic = tarih;

                        datagridhataraporu.Rows.Add(
                            "Servera Ulaşılamadı – Bağlantı Hatası Başladı",
                            tarih.ToString("dd.MM.yyyy HH:mm:ss"),
                            ""
                        );
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
                        string sure = $"{(int)fark.TotalMinutes} dk {fark.Seconds} sn";

                        datagridhataraporu.Rows.Add(
                            "Servera Ulaşıldı – Bağlantı Hatası Bitti",
                            tarih.ToString("dd.MM.yyyy HH:mm:ss"),
                            sure
                        );

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

                        datagridhataraporu.Rows.Add(
                            "Internetinizin Bağlantısı Koptu",
                            tarih.ToString("dd.MM.yyyy HH:mm:ss"),
                            ""
                        );
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
                        string sure = $"{(int)fark.TotalMinutes} dk {fark.Seconds} sn";

                        datagridhataraporu.Rows.Add(
                            "Internetiniz Düzeldi",
                            tarih.ToString("dd.MM.yyyy HH:mm:ss"),
                            sure
                        );

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

                    datagridhataraporu.Rows.Add("DataBase Koptu!", tarih.ToString("dd.MM.yyyy HH:mm:ss"), "");
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
                        sure = $"{(int)fark.TotalMinutes} dk {fark.Seconds} sn";
                    }

                    datagridhataraporu.Rows.Add(
                        "Database Bağlandı",
                        baslamazamani.ToString("dd.MM.yyyy HH:mm:ss"),
                        sure
                    );

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

                    datagridhataraporu.Rows.Add(
                            "Bilgisayar kapandı ya da restocell durdur yapıldı",
                            tarih.ToString("dd.MM.yyyy HH:mm:ss"),
                            ""
                        );

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
                        sure = $"{(int)fark.TotalMinutes} dk {fark.Seconds} sn"; //fark dakika ve saniye cinsinden formatlanıyor.
                    }

                    datagridhataraporu.Rows.Add(
                        "Bilgisayar açıldı ya da restocell başlat yapıldı",
                        baslamaZamani.ToString("dd.MM.yyyy HH:mm:ss"),
                        sure
                    );

                    sonDurma = null;
                    continue;
                }
            }



            // Log okuma tamamlandıktan sonra

        





            dataGridToplamHatalarıGoster.Columns.Clear();
            dataGridToplamHatalarıGoster.Rows.Clear();

            dataGridToplamHatalarıGoster.Columns.Add("Olay", "Olay");
            dataGridToplamHatalarıGoster.Columns.Add("Sayı", "Hata Sayısı");

            dataGridToplamHatalarıGoster.Rows.Add("Toplam 'DataBase Bağlantısı Koptu' hatası sayısı:", hataSayisi);
            dataGridToplamHatalarıGoster.Rows.Add("Toplam 'Bilgisayar Kapandı ya da Restocell Durduruldu' hatası sayısı:", hatasayisi2);
            dataGridToplamHatalarıGoster.Rows.Add("Internet Hatası sayısı", hatasayisi3);
            dataGridToplamHatalarıGoster.Rows.Add("Servera Ulaşılamadı – Bağlantı Hatası sayısı", hataSayisi4);

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




        ////hataraporu detay
        //private void button3_Click_1(object sender, EventArgs e)
        //{
        //    string klasorYolu = @"D:\RestoPOS\MY\LOG\";
        //    string secilen = kaynak_combo.SelectedItem?.ToString();

        //    if (string.IsNullOrEmpty(secilen))
        //    {
        //        MessageBox.Show("Lütfen bir kaynak seçin.");
        //        return; //metodu durdur
        //    }

        //    string dosyaTarihi = tarihsec.Value.ToString("yyyyMMdd");
        //    string dosyaYolu = Path.Combine(klasorYolu, $"{secilen}_{dosyaTarihi}.txt"); //dosyaYolu = D:\RestoPOS\MY\LOG\RcGuard_20251105.txt

        //    if (!File.Exists(dosyaYolu))
        //    {
        //        MessageBox.Show("Seçilen tarihe ait log bulunamadı.");
        //        return;
        //    }

        //    datagridkontrol.Rows.Clear();
        //    datagridkontrol.RowHeadersVisible = false;

        //    if (datagridkontrol.Columns.Count == 0)
        //    {
        //        datagridkontrol.Columns.Add("Olay", "Olay");
        //        datagridkontrol.Columns.Add("Zaman", "Zaman");
               

        //    }


        //    string[] satirlar = File.ReadAllLines(dosyaYolu, Encoding.GetEncoding("windows-1254"));  //Log dosyasını oku (File.ReadAllLines)
        //    DateTime? oncekiZaman = null;

        //    foreach (string satir in satirlar)
        //    {
        //        DateTime zaman = ExtractDate(satir); //Her satırdan zamanı çıkar (ExtractDate)

        //        // Tarih bulunamadıysa bu satırı atla
        //        if (zaman == DateTime.MinValue)
        //            continue;

        //        if (oncekiZaman != null)
        //        {
        //            TimeSpan fark = zaman - oncekiZaman.Value; //Önceki satırın zamanıyla farkını bul

        //            // 🔸 Eğer iki log arası 15 saniyeden fazla ise
        //            if (fark.TotalSeconds > 15)
        //            {
        //                // O anki satırın tam metnini grid’e ekle
        //                datagridkontrol.Rows.Add(
        //                    satir.Trim(),                                  // log içeriği
        //                    zaman.ToString("dd.MM.yyyy HH:mm:ss")         // zamanı
                                           
        //                );
        //            }
        //        }

        //        oncekiZaman = zaman; //O anki zamanı önceki zaman olarak kaydet
        //    }

        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

      
     }
    }

    





