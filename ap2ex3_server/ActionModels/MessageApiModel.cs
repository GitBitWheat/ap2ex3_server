using ap2ex3_server.Models;

namespace ap2ex3_server.ActionModels
{
    public class MessageApiModel
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public DateTime Created { get; set; }

        public bool Sent { get; set; }

        public MessageApiModel(Message message)
        {
            Id = message.Id;
            Content = message.Content;
            Created = message.Created;
            Sent = message.Sent;
        }
    }
}
