using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Logokuma24062025.trendyol;

namespace Logokuma24062025
{
    internal class trendyol
    {
        public class trendyollog

        {
            public int totalCount { get; set; }
            public int totalPages { get; set; }
            public List<content> content { get; set; }
        }



        public class content

        {
            public customer customer { get; set; }

            public payment payment { get; set; }

            public address address { get; set; }
            public string id { get; set; }
            public string orderCode { get; set; }
            public string deliveryType { get; set; }
            public double totalPrice { get; set; }
            public string callCenterPhone { get; set; }
            public string customerNote { get; set; }
            public string description { get; set; }
           // public int totalSellerAmount { get; set; }
            public List<lines> lines { get; set; }
            public List<promotions> promotions { get; set; }
        }

        public class lines
        {
            public double unitSellingPrice { get; set; }
            public List<object> items { get; set; }   // her adet için bir eleman
            public List<modifierProducts> modifierProducts { get; set; }
            // public List<extraIngredients> extraIngredients { get; set; }
            //public List<removedIngredients> removedIngredients { get; set; }
            public string name { get; set; }

        }

        public class customer
        {
            public string id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }

        }

        public class payment
        {
            public string paymentType { get; set; }

        }

        public class address
        { 
           
            public string city { get; set; }
            public string district { get; set; }
            public string neighborhood { get; set; }
            public string apartmentNumber { get; set; }
            public string floor { get; set; }
            public string doorNumber { get; set; }
            public string addressDescription { get; set; }
        }   
       
        public class promotions
        {
            public string description { get; set; }
            public double totalSellerAmount { get; set; }
        }


        public class modifierProducts
        {
            public string name { get; set; }
            public double price { get; set; }
            public List<modifierProducts> ModifierProducts { get; set; }  // 👈 İç içe tekrar kendi tipinden!


        }   
    }
}
