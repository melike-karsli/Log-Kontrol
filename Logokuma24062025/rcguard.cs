using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Logokuma24062025.yemeksepeti;

namespace Logokuma24062025
{
    internal class rcguard
    {
        public class rcguardlog
        {

            //public HashSet<string> islenmisJsonlar = new HashSet<string>();
            public string orderSource { get; set; }
            public string orderid { get; set; }
            public string orderTypeText { get; set; }
            public string orderDate { get; set; }
            public client client { get; set; }
            public List<menus> menus { get; set; }
            public List<discount> discount { get; set; }
            public List<payments> payments { get; set; }
            public List<products> products { get; set; }

        }

        public class client
        {
            public string id { get; set; }
            public string name { get; set; }

        }

        public class menus
        {
            public string name { get; set; }
            public string quantity { get; set; }
            public string price { get; set; }
            public List<products> products { get; set; }
        }

        public class products // entity framework ile içindeki ürünler toplatılacak
        {
            public string name { get; set; }
            public string quantity { get; set; }
            public string price { get; set; }
        }

        public class payments
        {
            public string paymentMethodText { get; set; }
            public string paymentAmount { get; set; }
        }

        public class discount  //dolu olan bır satıştan kontrol et
        {
            public string description { get; set; }
            public string amount { get; set; }
        }
    }
}
