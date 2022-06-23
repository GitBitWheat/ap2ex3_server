using ap2ex3_server.Models;

namespace ap2ex3_server.ActionModels
{
    public class NewContactModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Server { get; set; }

        public Contact ConvertToContact()
        {
            return new Contact()
            {
                Username = Id,
                Name = Name,
                Server = Server
            };
        }
    }
}
