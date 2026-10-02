using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoPOSSiparisTakip
{
    internal class kurye
    {
        // my_kurye logundaki "<Kurye adı> Json :{..."orderId"...}" satırı: kuryeye gönderilen sipariş (ürün listesi yok).
        // Kurye adı müşteriye göre değişiyor ("Fiyuu Json :", "Diğer Kurye Json :"), JSON yapısı aynı.
        public class kuryesiparis
        {
            public string storeId { get; set; }
            public string orderId { get; set; }
            public string salesChannelSId { get; set; }   // "YemekSepeti", ...
            public bool isTestOrder { get; set; }
            public customer customer { get; set; }
            public address address { get; set; }
            public payment payment { get; set; }
        }

        public class customer
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string customerPhone { get; set; }
        }

        public class address
        {
            public string fullAddress { get; set; }
            public string addressDirection { get; set; }   // adres tarifi + müşteri notu
            public string city { get; set; }
            public string county { get; set; }
        }

        public class payment
        {
            public string paymentTypeSId { get; set; }     // "cash", ...
            public long totalPrice { get; set; }           // kuruş cinsinden (71740 = 717,40 TL)
        }

        // "Diğer Kurye Json :{..."orderId"..."nameSurname"...}" satırı: tüm alanlar düz gelir (ürün listesi yok).
        // Sadece merchantId/branchId içeren satırlar sipariş değildir.
        public class digerkuryesiparis
        {
            public string orderId { get; set; }
            public string salesChannel { get; set; }       // "YemekSepeti", "Trendyol", "RestoPOS"
            public string nameSurname { get; set; }
            public string phoneNumber { get; set; }
            public string address { get; set; }
            public string addressDetail { get; set; }      // "Esentepe Şişli/İstanbul Tarif: 49 | Test Sip; ..."
            public string paymentMethod { get; set; }      // "cash", "online-credit-card", "offline-metropol"
            public decimal totalAmount { get; set; }       // TL cinsinden (315)
        }

        // "RestoWay Json :{"RestoOrderId":...}" satırı: RestoWay kuryesine gönderilen sipariş (ürün listesi yok).
        // Sadece MarketOrderNo içeren satırlar durum sorgusu olduğu için okunmaz.
        public class restowaysiparis
        {
            public long RestoOrderId { get; set; }
            public int MarketId { get; set; }              // 1 = Yemeksepeti, 3 = Trendyol
            public string MarketOrderNo { get; set; }      // platformun sipariş numarası
            public decimal Amount { get; set; }            // TL cinsinden (477.5)
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string OrderNote { get; set; }
            public string PaymentMethod { get; set; }      // "Nakit", "Online Ödeme", ...
            public string Address { get; set; }
            public string AddressDescription { get; set; } // adres tarifi + müşteri notu
            public string MobilePhone { get; set; }
        }
    }
}
