namespace staj.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }  // Teslimat adresi
        public string BillingAddress { get; set; }   // Faturalandırma adresi

        public string Password { get; set; }

        // Mobil tarafındaki müşterinin siparişleri
        public ICollection<Order> Orders { get; set; }
      
    }
}
