

using Microsoft.AspNetCore.SignalR;
using stajApi.Models;
using System.Collections.Concurrent;

namespace stajApi.Hubs
{
    public class MessageHub : Hub
    {
        // Tüm bağlı kullanıcılar: <KullanıcıId, ConnectionId>
        public static ConcurrentDictionary<int, string> ConnectedUsers = new();

        // Mesaj gönderimi
        public async Task SendPrivateMessage(ChatMessage message)
        {
            var receiverUserId = message.ReceiverId.ToString();
            await Clients.User(receiverUserId).SendAsync("ReceiveMessage", message);
        }

        public override Task OnConnectedAsync()
        {
            var userIdStr = Context.User?.FindFirst("Id")?.Value;

            if (int.TryParse(userIdStr, out int userId))
            {
                ConnectedUsers[userId] = Context.ConnectionId!;
                Console.WriteLine($"✅ Bağlandı: {userId} (ConnId: {Context.ConnectionId})");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.User?.FindFirst("Id")?.Value;

            if (int.TryParse(userIdStr, out int userId))
            {
                ConnectedUsers.TryRemove(userId, out _);
                Console.WriteLine($"❌ Ayrıldı: {userId}");
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
