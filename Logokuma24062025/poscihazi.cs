using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logokuma24062025
{
    internal class poscihazi  // internal olarak Dış bir projeden bu sınıfa erişilemez yanlzıca aynı projedeki dosyalardan erişilebilir
    {
        public class IngenicoData      // publıc herkes erısır
        {
            public int AdisyonNo   { get; set; } // get=değişken çağrıldıgında calısmaya baslayan method set= değişken atandığında calısmaya baslayan method
            public Receipt Receipt { get; set; } //dizi
            public string SerialNo { get; set; } //receipt içindeki veri


            //public HeaderInfo Header_Info { get; set; }
        }

        public class Receipt  //json objectinin Receipt kısmı
        {
            public List<object> DiscountList { get; set; }
            public int EkuNo { get; set; }
            public Invoice Invoice { get; set; }
            public double PaidAmount { get; set; }
            public List<Payment> PaymentList { get; set; } //PAYMENTLIST ARRAY dır
            public int ReceiptNo { get; set; }
            public int TicketType { get; set; }
            public string TransDateTime { get; set; }
            public int ZNo { get; set; }
        }

        public class Invoice
        {
            public string InvoiceDate { get; set; }
            public string InvoiceNo { get; set; }
            public int InvoiceType { get; set; }
            public string TCKN_VKN { get; set; }
        }

        public class Payment
        {
            public int BankBKMID { get; set; }
            public double PaymentAmount { get; set; }
            public int PaymentCurrency { get; set; }
            public string PaymentDateTime { get; set; }
            public string PaymentDesc { get; set; }
            public string PaymentInfo { get; set; }
            public int PaymentType { get; set; }
            public long? PaymentTypeEx { get; set; }
            public string Stannumber { get; set; }
            public string Batchnumber { get; set; }
            public object Merchantid { get; set; }
            public object Referencenumber { get; set; }
            public object Authorizationcode { get; set; }
            public object Paymentsubtype { get; set; }
            public object Numberofinstallment { get; set; }
            public List<object> subPayment { get; set; }
        }

        //public class HeaderInfo
        //{
        //    public string Password { get; set; }
        //    public string TrackId { get; set; }
        //    public string UserName { get; set; }
        //}

    }
}
