
namespace stajApi.Models
{
    public class Order
    {

        public int Id { get; set; }

        // Web tarafındaki kullanıcı ile ilişki
        public int UserId { get; set; }
        public User? User { get; set; }

        // Mobil tarafındaki müşteri ile ilişki
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; }
        public bool IsCompleted { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
