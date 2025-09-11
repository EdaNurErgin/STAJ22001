using System.ComponentModel.DataAnnotations;

public class ChatMessage
{

    public int Id { get; set; }

    public int? SenderId { get; set; }           // User.Id veya Customer.Id
    public string? SenderRole { get; set; }      // "User" veya "Customer"

    public int? ReceiverId { get; set; }
    public string? ReceiverRole { get; set; }    // "User" veya "Customer"

    public string? SenderName { get; set; }
    public string? ReceiverName { get; set; }

    [Required(ErrorMessage = "Mesaj boş olamaz")]
    [MinLength(1, ErrorMessage = "Mesaj en az 1 karakter olmalıdır")]
    public string? Message { get; set; }
    public DateTime? Timestamp { get; set; } = DateTime.Now;
}
