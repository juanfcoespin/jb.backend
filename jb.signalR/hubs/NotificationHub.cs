using jb.signalR.msg;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace jb.signalR.hubs
{
    public class NotificationHub:Hub
    {
        // Diccionario para asociar clientes con su ConnectionId
        private static ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            // Recibir UserId desde el cliente (Ionic) y asociarlo con su ConnectionId
            string userId = Context.GetHttpContext().Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedClients[userId] = connectionId;
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            var userId = ConnectedClients.FirstOrDefault(x => x.Value == connectionId).Key;

            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedClients.TryRemove(userId, out _);
            }

            return base.OnDisconnectedAsync(exception);
        }

        // Método para que la API envíe mensajes a un usuario específico
        public async Task SendNotificationToUser(string userId, CommunicationMsg me)
        {
            if (ConnectedClients.TryGetValue(userId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", me);
            }
        }
        public static string GetConnectionId(string userId)
        {
            return ConnectedClients.TryGetValue(userId, out var connectionId) ? connectionId : null;
        }
    }
}
