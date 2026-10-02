using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logokuma24062025
{
    internal class rcguard
    {
        // RCGuard logundaki "Gelen Veri : [...]" satırı: her eleman bir sipariş
        public class rcguardlog
        {
            public string orderSource { get; set; }
            public string orderid { get; set; }
            public string orderTypeText { get; set; }   // "Gel Al", "Adrese Teslim"
            public string orderDate { get; set; }
            public client client { get; set; }
            public List<menus> menus { get; set; }
            public List<discount> discount { get; set; }
            public List<payments> payments { get; set; }
            public List<products> products { get; set; }
            public string clientNote { get; set; }      // müşteri notu bu metnin içinde "Sipariş Notu : ..." olarak geliyor
            public string orderNote { get; set; }
            public decimal totalAmount { get; set; }
        }

        public class client
        {
            public string id { get; set; }
            public string name { get; set; }
            public string clientPhoneNumber { get; set; }
        }

        public class menus
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public decimal price { get; set; }
            public string note { get; set; }
            public List<products> products { get; set; }   // ilk eleman genelde menünün kendisi
        }

        public class products
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public decimal price { get; set; }
            public string note { get; set; }
            public List<options> options { get; set; }
        }

        public class options
        {
            public string name { get; set; }
            public int quantity { get; set; }
        }

        public class payments
        {
            public string paymentMethodText { get; set; }
            public string paymentAmount { get; set; }
        }

        public class discount
        {
            public string description { get; set; }
            public decimal amount { get; set; }        // negatif gelir (örn. -45)
        }
    }
}
