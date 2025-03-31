using jb.signalR.hubs;
using jb.signalR.msg;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace jb.signalR.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController:ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] CommunicationMsg me)
        {
            if (string.IsNullOrEmpty(me.userId) || string.IsNullOrEmpty(me.message))
            {
                return BadRequest(new { message = "UserId y Message son requeridos." });
            }

            // Llamar al método del hub para enviar la notificación
            var connectionId = NotificationHub.GetConnectionId(me.userId);
            if (connectionId != null) {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", me);
                return Ok(new { message = "Notificación enviada correctamente." });
            }
            return BadRequest(new { message = "No se ha podido establecer el connection Id para el usuario "+me.userId });

        }
    }
}
