using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logokuma24062025
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
            public int totalPrice { get; set; }
            public int primaryDiscountedPriceText { get; set; }
            public string customerFullName { get; set; }
            public string address { get; set; }
            public string paymentType { get; set; }
            public string deliveryProvider { get; set; }
            public List<products> products { get; set; }
        }
        public class products
        {
            public string name { get; set; }
            public int amount { get; set; }
            public int price { get; set; }
            public List<options> options { get; set; }
        }

        public class options
        {
            public string headerName { get; set; }
            public string itemNames { get; set; }
        }

        
    }
}
