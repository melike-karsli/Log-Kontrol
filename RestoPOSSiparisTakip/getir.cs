using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoPOSSiparisTakip
{
    internal class getir
    {
        public class getirlog
        {
            public string id { get; set; }
            public string confirmationId { get; set; }
            public int status { get; set; }
            public client client { get; set; }           
            public List<products> products { get; set; } //Bu property bir product türünden nesnelerden oluşan liste tutar Property’nin adı. JSON’dan gelen “products” alanına karşılık gelir
            public paymentmethodtext paymentMethodText { get; set; }

        }
        public class products
        {
            public name name { get; set; }
            public List<optionCategories> optionCategories { get; set; }
            public double totalPrice { get; set; }
        }
        public class name
        {
            public string tr { get; set; }
        }
        public class optionCategories
        {
            public List<options> options { get; set; }
        }
        public class options
        {
            public name name { get; set; }
            public double price { get; set; }
        }
        public class client
        {
            public string id { get; set; }
            public string name { get; set; }

            public string clientPhoneNumber { get; set; }
            public location location { get; set; }  // ✅ tek obje
            //public List<location> location { get; set; }
           
        }
        public class location
        {
            
            public string lat { get; set; }
            public string lon { get; set; }
        }

        public class paymentmethodtext
        {
            public string tr { get; set; }
            public string en { get; set; }
        }

        //public class restaurant
        //{
        //    public int paymentMethod { get; set; }

        //}


    }
}
