using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Logokuma24062025
{
    internal class yemeksepeti
    {
        public class yemeksepetilog
        {

            
            //public HashSet<string> islenmisJsonlar = new HashSet<string>();
            public platformRestaurant platformRestaurant { get; set; }
            public delivery delivery { get; set; }
            public localInfo localInfo { get; set; }
            public customer customer { get; set; }
            public payment payment { get; set; } 
            public List<products> products { get; set; }
            public List<discounts> discounts { get; set; }
            public price price { get; set; }
        }

        public class platformRestaurant
        {
            public string id { get; set; }
            
        }
        public class delivery
        {
            public address address { get; set; }      
        }

        public class address
        {
            public string street { get; set; }
            public string number { get; set; }
            public string deliveryMainArea { get; set; }
            public string city { get; set; }
           
        }

        public class localInfo
        {
            public string platform { get; set; }
        }
        public class customer
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string mobilePhone { get; set; } 
        }

        public class payment
        {
            public string type { get; set; }            
        }
        public class products
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public List<selectedToppings> selectedToppings { get; set; }
        }

        public class selectedToppings
        {
            public string name { get; set; }
            public int quantity { get; set; }
        }
        public class discounts
        {
            public string name { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public string amount { get; set; }
        }

        public class price 
        { 
           public string grandTotal { get; set; }
        }

    }
}
