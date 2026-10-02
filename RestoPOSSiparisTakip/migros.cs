using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoPOSSiparisTakip
{
    internal class migros
    {
        public class migroslog
        {
            public List<data> data { get; set; }
            public bool success { get; set; }
        }

        public class data
        {
            public long storeId { get; set; }
            public List<pendingOrderDetails> pendingOrderDetailsDTOS { get; set; }
        }

        public class pendingOrderDetails
        {
            public long id { get; set; }
            public string phoneNumber { get; set; }
            public long totalPrice { get; set; }        // kuruş cinsinden (60000 = 600,00 TL)
            public long discountedPrice { get; set; }   // indirimli tutar, kuruş cinsinden
            public string customerFullName { get; set; }
            public string address { get; set; }
            public string orderNote { get; set; }
            public string paymentType { get; set; }
            public string paymentTypeDescription { get; set; }
            public string deliveryProvider { get; set; }
            public List<products> products { get; set; }
        }
        public class products
        {
            public string name { get; set; }
            public int amount { get; set; }
            public long price { get; set; }
            public string note { get; set; }
            public List<options> options { get; set; }
        }

        public class options
        {
            public string headerName { get; set; }
            public string itemNames { get; set; }
        }

        
    }
}
