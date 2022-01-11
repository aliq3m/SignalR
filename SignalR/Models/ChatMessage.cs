namespace SignalR.Models
{
    public class ChatMessage
    {
        public string    SenderName { get; set; }
        public string    TextMessage { get; set; }
        public System.DateTimeOffset SendAt { get; set; }
    }
}
