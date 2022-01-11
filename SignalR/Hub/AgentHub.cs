using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MyChat;
using SignalR.Models;
using SignalR.Services;

namespace SignalR.Hub
{

    [Authorize]
    public class AgentHub:Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IHubContext<ChatHub> _chatHub;

        public AgentHub(IChatRoomService chatRoomService,IHubContext<ChatHub> chatHub)
        {
            _chatRoomService = chatRoomService;
            _chatHub = chatHub;
        }

        public override async Task OnConnectedAsync()
        {

            await Clients.Caller.SendAsync("ActiveRooms", await _chatRoomService.GetAllRooms());
            await base.OnConnectedAsync();
        }

        public async Task SendAgentMessage(Guid roomId, string text)
        {
            var message = new ChatMessage
            {
                SendAt = DateTimeOffset.UtcNow,
                SenderName = Context.User.Identity.Name,
                TextMessage = text
            };
            await _chatRoomService.AddMessage(roomId, message);

            await _chatHub.Clients.Group(roomId.ToString())
                .SendAsync("ReceiveMessage", message.SenderName, message.SendAt, message.TextMessage);
        }

        public async Task LoadHistory(Guid roomId)
        {
            var history = await _chatRoomService.GetMessageHistory(roomId);
            await Clients.Caller.SendAsync("ReceiveMessages", history);
        }

    }
}
