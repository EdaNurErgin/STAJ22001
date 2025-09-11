using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using stajApi.Data;
using stajApi.Hubs;
using stajApi.Models;

namespace stajApi.Controllers
{
    /// <summary>
    /// mesaj gonderme ile ilgili işlemleri içerir
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MessageApiController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public MessageApiController(IHubContext<MessageHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        // 🔸 Mesaj gönderme ve DB’ye kaydetme
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            message.Timestamp = DateTime.Now;
            Console.WriteLine($"Gelen Mesaj: From {message.SenderId} ({message.SenderRole}) → {message.ReceiverId} ({message.ReceiverRole}) | İçerik: {message.Message}");

            // SenderName getir
            if (message.SenderRole == "User" && message.SenderId.HasValue)
            {
                var user = await _context.Users.FindAsync(message.SenderId.Value);
                message.SenderName = user?.Username;
            }
            else if (message.SenderRole == "Customer" && message.SenderId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(message.SenderId.Value);
                message.SenderName = customer?.FullName;
            }

            // ReceiverName getir
            if (message.ReceiverRole == "User" && message.ReceiverId.HasValue)
            {
                var user = await _context.Users.FindAsync(message.ReceiverId.Value);
                message.ReceiverName = user?.Username;
            }
            else if (message.ReceiverRole == "Customer" && message.ReceiverId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(message.ReceiverId.Value);
                message.ReceiverName = customer?.FullName;
            }

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    //        await _hubContext.Clients.User(message.ReceiverId?.ToString())
    //.SendAsync("ReceiveMessage", message);

            return Ok(new { status = "sent", message });
        }


        // 🔸 Mesaj geçmişini getir (iki kişi arasında)
        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(int senderId, int receiverId)
        {
            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId)
                )
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }
        // GET api/MessageApi/customers-with-messages
        [HttpGet("customers-with-messages")]
        public async Task<IActionResult> GetCustomersWhoSentMessagesToUser(int userId)
        {
            var customerIds = await _context.ChatMessages
                .Where(m => m.ReceiverId == userId && m.ReceiverRole == "User" && m.SenderRole == "Customer")
                .Select(m => m.SenderId)
                .Distinct()
                .ToListAsync();

            var customers = await _context.Customers
                .Where(c => customerIds.Contains(c.Id))
                .Select(c => new
                {
                    senderId = c.Id,
                    senderName = c.FullName
                })
                .ToListAsync();

            return Ok(customers);
        }


        [HttpGet("is-online/{id}")]
        public IActionResult IsUserOnline(int id)
        {
            bool isOnline = MessageHub.ConnectedUsers.ContainsKey(id);
            return Ok(new { isOnline });
        }


        [HttpDelete("delete-conversation")]
        public async Task<IActionResult> DeleteConversation(int senderId, int receiverId)
        {
            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId)
                )
                .ToListAsync();

            _context.ChatMessages.RemoveRange(messages);
            await _context.SaveChangesAsync();

            return Ok(new { deleted = true });
        }


    }
}
