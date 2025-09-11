namespace stajApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Gerçekte hash'lenmeli
        public string Email { get; set; }

        // Web tarafındaki kullanıcının verdiği siparişler
        public ICollection<Order> Orders { get; set; }
    }
}
